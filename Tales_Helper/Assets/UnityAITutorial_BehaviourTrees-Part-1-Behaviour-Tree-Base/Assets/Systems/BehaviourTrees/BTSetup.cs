using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(BehaviourTree))]
public class BTSetup : MonoBehaviour
{


    [Header("Wander and Rest Settings")]
    [SerializeField] private float Wander_Range = 10f;
    [SerializeField] private float TimeToRest = 0.05f; // 탐색 후 휴식까지의 시간
    [SerializeField] private Transform chairTransform; // 의자의 Transform

    private float wanderTimer = 0f; // 탐색 타이머
    private float debugTimer = 0f; // 디버그 타이머

    [Header("Chase Settings")]
    [SerializeField] float Chase_MinAwarenessToChase = 1.5f;
    [SerializeField] float Chase_AwarenessToStopChase = 1f;

    DetectableTarget Chase_CurrentTarget;

    protected BehaviourTree LinkedBT;
    protected CharacterAgent Agent;
    protected AwarenessSystem Sensors;
    protected RestCondition restCondition;
    protected Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        Agent = GetComponent<CharacterAgent>();
        LinkedBT = GetComponent<BehaviourTree>();
        Sensors = GetComponent<AwarenessSystem>();
        restCondition = GetComponent<RestCondition>();


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

                

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return Agent.AtDestination ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });
        // "Rest Condition" 컨디션 노드를 BTRoot에 추가
        var restRoot = BTRoot.Add(new BTNode_Condition("Rest Condition",
            () => restCondition.CheckCondition()));
        
        // "Rest on Chair" 액션 노드를 BTRoot에 추가
        restRoot.Add<BTNode_Action>("Rest on Chair",
            () =>
            {
            // 조건이 충족되면 앉기 로직 실행
            Agent.SitAtPosition(chairTransform.position, chairTransform.rotation);
                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
            // 앉는 애니메이션이 실행되고 있는지 확인
            return anim.GetBool("Sitting") ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });

    }


}