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
    [SerializeField] private Transform chairTransform; // 의자의 Transform

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

    void Awake()
    {
        anim = GetComponent<Animator>();
        Agent = GetComponent<CharacterAgent>();
        LinkedBT = GetComponent<BehaviourTree>();
        LinkedAI = GetComponent<EnemyAI>();
        Sensors = GetComponent<AwarenessSystem>();
        restCondition = GetComponent<RestCondition>();

        var BTRoot = LinkedBT.RootNode.Add<BTNode_Selector>("Base Logic");

        #region 추적노드
        var chaseRoot = BTRoot.Add(new BTNode_Condition("Can Chase",
            () =>
            {
                if (DroppedObject.Instance.CheckSpecialObjectCondition())
                {
                    StartCoroutine(StartChaseAfterDelay(2f));

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
                // 거인이 앉아 있는 경우, 먼저 일으켜 세움
                if (anim.GetBool("SittingToWalk"))
                {
                    Agent.StandUpChase();
                    restCondition.ResetCondition();
                }
                if (restCondition.IsStandingUp())
                {
                    Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                }

                if (Sensors.IsPlayerFullyDetected() == true) // IsPlayerFullyDetected는 예시 메소드로, 실제 구현 필요
                {
                    Agent.AttackToPlayer(Chase_CurrentTarget.gameObject); // 플레이어 공격 실행
                    return BehaviourTree.ENodeStatus.Succeeded; // 공격 후 상태를 성공으로 변경하여 다음 행동으로 넘어갈 수 있도록 함
                }

                // 추적 로직
                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                if (Sensors.IsPlayerFullyDetected() == true) // IsPlayerFullyDetected는 예시 메소드로, 실제 구현 필요
                {
                    Agent.AttackToPlayer(Chase_CurrentTarget.gameObject); // 플레이어 공격 실행
                    return BehaviourTree.ENodeStatus.Succeeded; // 공격 후 상태를 성공으로 변경하여 다음 행동으로 넘어갈 수 있도록 함
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
               // 앉는 액션이 시작되면
               if (!anim.GetBool("SittingToWalk"))
                {
                   Agent.SitAtPosition(chairTransform.position, chairTransform.rotation);
                   return BehaviourTree.ENodeStatus.InProgress; // 액션 진행 중
                }

               if (Sensors.ActiveTargets.Count == 0 && restCondition.CheckCondition())
                {
                   // 조건을 체크하여 충분히 쉬었다면 Succeeded 반환
                   return BehaviourTree.ENodeStatus.Succeeded;
                }

                // 조건 미충족 시 InProgress 유지
                return BehaviourTree.ENodeStatus.InProgress;
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
                // 거인이 앉아 있는 경우, 먼저 일으켜 세움
                if (anim.GetBool("SittingToWalk"))
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