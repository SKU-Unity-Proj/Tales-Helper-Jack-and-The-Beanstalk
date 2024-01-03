using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(BehaviourTree))]
public class BTSetup : MonoBehaviour
{


    [Header("Wander and Rest Settings")]
    [SerializeField] private float Wander_Range = 10f;
    [SerializeField] private float TimeToRest = 0.05f; // Ž�� �� �޽ı����� �ð�
    [SerializeField] private Transform chairTransform; // ������ Transform

    private float wanderTimer = 0f; // Ž�� Ÿ�̸�
    private float debugTimer = 0f; // ����� Ÿ�̸�

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
        // "Rest Condition" ����� ��带 BTRoot�� �߰�
        var restRoot = BTRoot.Add(new BTNode_Condition("Rest Condition",
            () => restCondition.CheckCondition()));
        
        // "Rest on Chair" �׼� ��带 BTRoot�� �߰�
        restRoot.Add<BTNode_Action>("Rest on Chair",
            () =>
            {
            // ������ �����Ǹ� �ɱ� ���� ����
            Agent.SitAtPosition(chairTransform.position, chairTransform.rotation);
                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
            // �ɴ� �ִϸ��̼��� ����ǰ� �ִ��� Ȯ��
            return anim.GetBool("Sitting") ? BehaviourTree.ENodeStatus.Succeeded : BehaviourTree.ENodeStatus.InProgress;
            });

    }


}