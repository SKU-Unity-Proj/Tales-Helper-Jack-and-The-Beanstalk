using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BehaviourTree))]
public class BTSetup : MonoBehaviour
{


    [Header("Wander and Rest Settings")]
    [SerializeField] private float Wander_Range = 10f;
    [SerializeField] private Transform chairTransform; // 의자 Transform
    [SerializeField] private Transform doorPos;

    [Header("Chase Settings")]
    [SerializeField] float Chase_MinAwarenessToChase = 1.5f;
    [SerializeField] float Chase_AwarenessToStopChase = 1f;

    DetectableTarget Chase_CurrentTarget;

    protected EnemyAI LinkedAI;
    protected BehaviourTree LinkedBT;
    protected CharacterAgent Agent;
    protected AwarenessSystem Sensors;
    protected RestCondition restCondition;
    protected Animator anim;
    protected NavMeshAgent giant;

    protected bool hasInteracted = false; // 상호작용 여부를 확인하는 변수

    void Awake()
    {
        anim = GetComponent<Animator>();
        Agent = GetComponent<CharacterAgent>();
        LinkedBT = GetComponent<BehaviourTree>();
        LinkedAI = GetComponent<EnemyAI>();
        Sensors = GetComponent<AwarenessSystem>();
        restCondition = GetComponent<RestCondition>();
        giant = GetComponent<NavMeshAgent>();
        Chase_CurrentTarget = GetComponent<DetectableTarget>();

        var BTRoot = LinkedBT.RootNode.Add<BTNode_Selector>("Base Logic");

        #region 추적노드
        var chaseRoot = BTRoot.Add(new BTNode_Condition("Can Chase",
            () =>
            {
                if (DroppedObject.Instance.CheckSpecialObjectCondition())
                {
                    Chase_CurrentTarget = BasicManager.Instance.PlayerTarget;

                    return true; // Coroutine이 완료될 때까지 기다림
                }

                // 타겟이 없는 경우
                // 'ActiveTargets' 리스트가 null이거나 비어있으면 추적을 시작하지 않음
                if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
                    return false;

                if (Chase_CurrentTarget != null)
                {
                    // 현재 타겟이 여전히 감지되고 있는지 확인
                    // 현재 타겟이 감지 목록에 있고, 인지도가 추적을 멈출 수준 이상이면 추적
                    foreach (var candidate in Sensors.ActiveTargets.Values)
                    {
                        if (candidate.Detectable == Chase_CurrentTarget &&
                            candidate.Awareness >= Chase_AwarenessToStopChase)
                        {
                            return true;
                        }
                    }

                    // 현재 타겟이 더 이상 감지되지 않으면 null로 설정
                    Chase_CurrentTarget = null;
                }

                // 가능한 새로운 타겟을 찾음
                // 감지된 대상들 중에서 가장 인지도가 높은 대상을 새로운 추적 대상으로 선택
                float highestAwareness = Chase_MinAwarenessToChase;
                foreach (var candidate in Sensors.ActiveTargets.Values)
                {
                    if (candidate.Awareness >= highestAwareness)
                    {
                        Chase_CurrentTarget = candidate.Detectable;
                        highestAwareness = candidate.Awareness;
                    }
                }

                // 새로운 추적 대상이 있는지 여부를 반환
                // 추적 대상이 있으면 true, 없으면 false를 반환.
                return Chase_CurrentTarget != null;
            }));
        chaseRoot.Add<BTNode_Action>("Chase Target",
            () =>
            {

                // Chase_CurrentTarget null 체크
                if (Chase_CurrentTarget == null)
                {
                    Debug.LogError("Chase_CurrentTarget is null. Cannot chase.");
                    return BehaviourTree.ENodeStatus.Failed;  // 적절한 상태 반환
                }

                // Agent null 체크
                if (giant == null)
                {
                    Debug.LogError("NavMeshAgent is null.");
                    return BehaviourTree.ENodeStatus.Failed;  // 적절한 상태 반환
                }

                // 거인이 앉아 있는 경우, 먼저 일으켜 세움
                if (anim.GetBool("Sitting"))
                {
                    Agent.StandUpChase();
                    restCondition.ResetCondition();
                }
                if (restCondition.IsStandingUp())
                {
                    Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                }

                // 타겟이 있을 때만 추적
                if (Chase_CurrentTarget != null)
                {
                    // NavMesh 경로 계산
                    NavMeshPath path = new NavMeshPath();
                    if (giant.CalculatePath(Chase_CurrentTarget.transform.position, path))
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            // 경로가 유효하면 이동 시작
                            Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                            Debug.Log("Chasing target: " + Chase_CurrentTarget.transform.name);
                        }
                        else
                        {
                            Debug.LogError("Path to target is invalid.");
                            return BehaviourTree.ENodeStatus.Failed;
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to calculate path to target.");
                        return BehaviourTree.ENodeStatus.Failed;
                    }
                }


                if (Sensors.IsPlayerFullyDetected() == true) // IsPlayerFullyDetected는 예시 메소드로, 실제 구현 필요
                {
                    Agent.AttackToPlayer(Chase_CurrentTarget.gameObject); // 플레이어 공격 실행

                    if (Sensors.IsPlayerMissingDetected() == true)
                    {
                        Agent.MissingPlayer(Chase_CurrentTarget.transform.position);

                        return BehaviourTree.ENodeStatus.Succeeded;
                    }

                    return BehaviourTree.ENodeStatus.InProgress; 
                }
             

                // 추적 로직
                //Agent.MoveToRun(Chase_CurrentTarget.transform.position);
               // Debug.Log(Chase_CurrentTarget.transform.name);
                //Debug.Log("Chasing player at position: " + Chase_CurrentTarget.transform.position);

                if (giant.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    Debug.LogError("Invalid path. NavMeshAgent cannot find a valid path.");
                }

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {

                if (Agent.IsKnockingDoor)
                {
                    Agent.StartKnockingDoor(doorPos.position, doorPos.rotation);

                    return BehaviourTree.ENodeStatus.InProgress;
                }

                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            });
        #endregion

        #region 앉기노드
        var restSequence = BTRoot.Add(new BTNode_Condition("Rest Condition",
           () =>
           {
               bool conditionMet = restCondition.CheckCondition();
               //Debug.Log($"Rest Condition node evaluated: {conditionMet}");
               return conditionMet;

           }));
        restSequence.Add<BTNode_Action>("Rest on Chair",
            () =>
            {
                //Debug.Log("Rest on Chair action started.");
                Agent.SitAtPosition(chairTransform.position, chairTransform.rotation);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                if (Sensors.ActiveTargets != null || Sensors.ActiveTargets.Count != 0)
                {
                    return BehaviourTree.ENodeStatus.Succeeded;

                }

                if (restCondition.CheckCondition())
                {
                    return BehaviourTree.ENodeStatus.Succeeded;
                }

                if (Agent.IsKnockingDoor)
                {
                    return BehaviourTree.ENodeStatus.Succeeded;
                }

                return anim.GetBool("Sitting") ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });
        #endregion

        #region 탐색노드
        // 떨어진 물체 감지 여부를 판별하는 조건 노드
        var droppedObjectCondition = BTRoot.Add(new BTNode_Condition("Check Dropped Objects",
           () =>
           {
               bool conditionMet = DroppedObject.Instance.CheckCondition();
               Debug.Log($"Searching Condition node evaluated: {conditionMet}");
               return conditionMet;
           }));

        // 떨어진 물체에 대한 상호작용을 처리하는 액션 노드
        droppedObjectCondition.Add<BTNode_Action>("Interact With Dropped Object",
            () =>
            {
                if (hasInteracted)
                {
                    // 이미 상호작용이 완료되었으므로 더 이상 처리하지 않음
                    return BehaviourTree.ENodeStatus.Succeeded;
                }

                /*
                // 탐색 중에 특별한 조건이 만족되면 추적 상태로 전환
                if (DroppedObject.Instance.CheckSpecialObjectCondition())
                {
                    // 추적할 대상을 설정하고 탐색 노드를 빠져나감
                    Chase_CurrentTarget = BasicManager.Instance.PlayerTarget;
                    Debug.Log("탐색 노드에서 추적 상태로 전환됨.");
                    return BehaviourTree.ENodeStatus.Succeeded; // 탐색을 완료하고 노드를 빠져나감
                }
                */

                // 거인이 앉아 있는 경우, 먼저 일으켜 세움
                if (anim.GetBool("Sitting"))
                {
                    Agent.StandUpSearch(); // Agent가 서있는 상태로 변경
                    restCondition.ResetCondition(); // 휴식 상태 초기화
                }

                // 현재 탐색 중인 떨어진 물체가 있는지 확인하고, 없으면 새로운 물체를 탐색 시작
                if (!Agent.IsSearching() && DroppedObject.Instance.GetDroppedObjectsCount() > 0)
                {
                    LinkedAI.OnSearching(); // 탐색 상태로 변경
                    Agent.SearchingObject(); // 떨어진 물체와 상호작용 시작
                    return BehaviourTree.ENodeStatus.InProgress; // 상호작용이 진행 중임을 나타냄
                }

                // 탐색이 완료되었는지 확인
                if (Agent.IsSearching())
                {
                    hasInteracted = true; // 상호작용 완료 플래그 설정
                    // 모든 떨어진 물체와의 상호작용이 완료되면 성공 상태 반환
                    return BehaviourTree.ENodeStatus.Succeeded;
                }


                // 아직 탐색 중이라면 InProgress 상태 유지
                return BehaviourTree.ENodeStatus.InProgress;
            });
        #endregion

        #region 순찰노드
        var wanderRoot = BTRoot.Add<BTNode_Sequence>("Wander");
        wanderRoot.Add<BTNode_Action>("Perform Wander",
            () =>
            {
                Vector3 location = Agent.PickLocationInRange(Wander_Range);
                anim.SetBool("Run", false);
                Agent.MoveTo(location);

                restCondition.UpdateTimer(Time.deltaTime);

                Debug.Log("wander");
                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                if (restCondition.CheckCondition())
                    return BehaviourTree.ENodeStatus.Succeeded;

                return Agent.AtDestination ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });
        #endregion


    }

    // 2초 딜레이 후 추적을 시작하는 Coroutine
    IEnumerator StartChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 2초 대기
        Chase_CurrentTarget = BasicManager.Instance.PlayerTarget;
        // 여기서 추적을 시작하는 로직을 실행하거나, 추적 시작 조건을 true로 설정
    }

}