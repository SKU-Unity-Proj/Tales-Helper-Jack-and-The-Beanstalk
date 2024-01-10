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
    [SerializeField] private float TimeToRest = 0.05f; // 탐색 후 휴식까지의 시간
    [SerializeField] private Transform chairTransform; // 의자의 Transform

    [Header("Chase Settings")]
    [SerializeField] float Chase_MinAwarenessToChase = 1.5f;
    [SerializeField] float Chase_AwarenessToStopChase = 1f;

    DetectableTarget Chase_CurrentTarget;

    protected BehaviourTree LinkedBT;
    protected CharacterAgent Agent;
    protected AwarenessSystem Sensors;
    protected RestCondition restCondition;
    protected Animator anim;
    protected NavMeshAgent giant;
    protected DroppedObjectDetector droppedObjectDetector;

    void Awake()
    {
        anim = GetComponent<Animator>();
        Agent = GetComponent<CharacterAgent>();
        LinkedBT = GetComponent<BehaviourTree>();
        Sensors = GetComponent<AwarenessSystem>();
        restCondition = GetComponent<RestCondition>();
        droppedObjectDetector = GetComponent<DroppedObjectDetector>();

        var BTRoot = LinkedBT.RootNode.Add<BTNode_Selector>("Base Logic");

        var chaseRoot = BTRoot.Add(new BTNode_Condition("Can Chase",
            () =>
            {
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
                if (anim.GetBool("Sitting"))
                {
                    Agent.StandUp();
                    restCondition.ResetCondition();
                }
                if (restCondition.IsStandingUp())
                {
                    Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                }

                // 추적 로직
                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            });

        var restSequence = BTRoot.Add(new BTNode_Condition("Rest Condition",
           () =>
           {
               bool conditionMet = restCondition.CheckCondition();
               Debug.Log($"Rest Condition node evaluated: {conditionMet}");
               return conditionMet;

           }));
        restSequence.Add<BTNode_Action>("Rest on Chair",
            () =>
            {
                Debug.Log("Rest on Chair action started.");
                Agent.SitAtPosition(chairTransform.position, chairTransform.rotation);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                if (Sensors.ActiveTargets != null || Sensors.ActiveTargets.Count == 0)
                {
                    return BehaviourTree.ENodeStatus.Succeeded;

                }

                return anim.GetBool("Sitting") ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });

        // 떨어진 물체 감지 여부를 판별하는 조건 노드
        var droppedObjectCondition = BTRoot.Add(new BTNode_Condition("Check Dropped Objects",
           () =>
           {
               return droppedObjectDetector.DroppedObjects.Count > 0;
           }));

        // 떨어진 물체에 대한 상호작용을 처리하는 액션 노드
        droppedObjectCondition.Add<BTNode_Action>("Interact With Dropped Object",
            () =>
            {
                if (droppedObjectDetector.DroppedObjects.Count > 0)
                {
                    // 떨어진 물체 중 하나의 위치로 이동하고 상호작용
                    Agent.SearchingObject(droppedObjectDetector.DroppedObjects[0].transform.position);

                    // 상호작용 후 물체 목록에서 제거
                    droppedObjectDetector.DroppedObjects.RemoveAt(0);
                }
                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return BehaviourTree.ENodeStatus.Succeeded;
            });

        var wanderRoot = BTRoot.Add<BTNode_Sequence>("Wander");
        wanderRoot.Add<BTNode_Action>("Perform Wander",
            () =>
            {
                Vector3 location = Agent.PickLocationInRange(Wander_Range);
                anim.SetBool("Run", false);
                Agent.MoveTo(location);

                restCondition.UpdateTimer(Time.deltaTime);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                if (restCondition.CheckCondition())
                    return BehaviourTree.ENodeStatus.Succeeded;

                return Agent.AtDestination ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });
        
       
        
    }

}