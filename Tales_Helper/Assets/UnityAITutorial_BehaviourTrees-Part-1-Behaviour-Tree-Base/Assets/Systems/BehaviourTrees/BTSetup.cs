using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(BehaviourTree))]
public class BTSetup : MonoBehaviour
{


    [Header("Wander Settings")]
    [SerializeField] float Wander_Range = 10f;

    [Header("Chase Settings")]
    [SerializeField] float Chase_MinAwarenessToChase = 1.5f;
    [SerializeField] float Chase_AwarenessToStopChase = 1f;

    DetectableTarget Chase_CurrentTarget;

    protected BehaviourTree LinkedBT;
    protected CharacterAgent Agent;
    protected AwarenessSystem Sensors;
    protected Animator anim;

    //의자관련
    [SerializeField] Transform interactionPosition; // 의자와 상호작용할 위치
    [SerializeField] Transform chairDirection; // 의자가 바라보는 방향

    private int wanderCount = 0; // Wander 행동 카운터
    private const int MaxWanderCount = 5; // 최대 Wander 횟수

    void Awake()
    {
        anim = GetComponent<Animator>();
        Agent = GetComponent<CharacterAgent>();
        LinkedBT = GetComponent<BehaviourTree>();
        Sensors = GetComponent<AwarenessSystem>();

        var BTRoot = LinkedBT.RootNode.Add<BTNode_Selector>("Base Logic");

        var chaseRoot = BTRoot.Add(new BTNode_Condition("Can Chase",
            () =>
            {
                // no targets
                if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
                    return false;

                if (Chase_CurrentTarget != null)
                {
                    // check if the current is still sensed
                    foreach (var candidate in Sensors.ActiveTargets.Values)
                    {
                        if (candidate.Detectable == Chase_CurrentTarget &&
                            candidate.Awareness >= Chase_AwarenessToStopChase)
                        {
                            return true;
                        }
                    }

                    // clear our current target
                    Chase_CurrentTarget = null;
                }

                // acquire a new target if possible
                float highestAwareness = Chase_MinAwarenessToChase;
                foreach (var candidate in Sensors.ActiveTargets.Values)
                {
                    // found a target to acquire
                    if (candidate.Awareness >= highestAwareness)
                    {
                        Chase_CurrentTarget = candidate.Detectable;
                        highestAwareness = candidate.Awareness;
                    }
                }

                return Chase_CurrentTarget != null;
            }));
        chaseRoot.Add<BTNode_Action>("Chase Target",
            () =>
            {

                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            });

        var wanderRoot = BTRoot.Add<BTNode_Sequence>("Wander");
        wanderRoot.Add<BTNode_Action>("Perform Wander",
            () =>
            {
                Vector3 location = Agent.PickLocationInRange(Wander_Range);
                anim.SetBool("Run", false);
                Agent.MoveTo(location);
                // 목적지에 도착했거나, 아직 목적지가 설정되지 않았다면 새로운 위치로 이동
                if (Agent.AtDestination)
                { 
                    wanderCount++;
                    Debug.Log($"Wander Count: {wanderCount}");
                }
    

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return Agent.AtDestination ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });


        // SitOnChair 노드
        var sitOnChairSequence = BTRoot.Add<BTNode_Sequence>("Sit on Chair");

        // SitOnChair 조건 노드
        sitOnChairSequence.Add(new BTNode_Condition("Can Sit",
            () => wanderCount >= MaxWanderCount)); // Wander 횟수 확인

        // "Interaction Position"으로 이동하는 액션 노드
        sitOnChairSequence.Add<BTNode_Action>("Move to Interaction Position",
            () =>
            {
                Agent.MoveToInteractionPositionAndSit(interactionPosition, chairDirection);
                Debug.Log("Move to chair"); // 디버그 로그 추가
                return BehaviourTree.ENodeStatus.Succeeded;
            });
    }
}
