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
    [SerializeField] private float TimeToRest = 0.05f; // Ž�� �� �޽ı����� �ð�
    [SerializeField] private Transform chairTransform; // ������ Transform

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
                // Ÿ���� ���� ���
                // 'ActiveTargets' ����Ʈ�� null�̰ų� ��������� ������ �������� ����
                if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
                    return false;

                if (Chase_CurrentTarget != null)
                {
                    // ���� Ÿ���� ������ �����ǰ� �ִ��� Ȯ��
                    // ���� Ÿ���� ���� ��Ͽ� �ְ�, �������� ������ ���� ���� �̻��̸� ����
                    foreach (var candidate in Sensors.ActiveTargets.Values)
                    {
                        if (candidate.Detectable == Chase_CurrentTarget &&
                            candidate.Awareness >= Chase_AwarenessToStopChase)
                        {
                            return true;
                        }
                    }

                    // ���� Ÿ���� �� �̻� �������� ������ null�� ����
                    Chase_CurrentTarget = null;
                }

                // ������ ���ο� Ÿ���� ã��
                // ������ ���� �߿��� ���� �������� ���� ����� ���ο� ���� ������� ����
                float highestAwareness = Chase_MinAwarenessToChase;
                foreach (var candidate in Sensors.ActiveTargets.Values)
                {
                    if (candidate.Awareness >= highestAwareness)
                    {
                        Chase_CurrentTarget = candidate.Detectable;
                        highestAwareness = candidate.Awareness;
                    }
                }

                // ���ο� ���� ����� �ִ��� ���θ� ��ȯ
                // ���� ����� ������ true, ������ false�� ��ȯ.
                return Chase_CurrentTarget != null;
            }));
        chaseRoot.Add<BTNode_Action>("Chase Target",
            () =>
            {
                // ������ �ɾ� �ִ� ���, ���� ������ ����
                if (anim.GetBool("Sitting"))
                {
                    Agent.StandUp();
                    restCondition.ResetCondition();
                }
                if (restCondition.IsStandingUp())
                {
                    Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                }

                // ���� ����
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

        // ������ ��ü ���� ���θ� �Ǻ��ϴ� ���� ���
        var droppedObjectCondition = BTRoot.Add(new BTNode_Condition("Check Dropped Objects",
           () =>
           {
               return droppedObjectDetector.DroppedObjects.Count > 0;
           }));

        // ������ ��ü�� ���� ��ȣ�ۿ��� ó���ϴ� �׼� ���
        droppedObjectCondition.Add<BTNode_Action>("Interact With Dropped Object",
            () =>
            {
                if (droppedObjectDetector.DroppedObjects.Count > 0)
                {
                    // ������ ��ü �� �ϳ��� ��ġ�� �̵��ϰ� ��ȣ�ۿ�
                    Agent.SearchingObject(droppedObjectDetector.DroppedObjects[0].transform.position);

                    // ��ȣ�ۿ� �� ��ü ��Ͽ��� ����
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