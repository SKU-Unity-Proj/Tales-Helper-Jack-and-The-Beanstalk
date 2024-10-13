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
    [SerializeField] private Transform chairTransform; // ���� Transform
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

    protected bool hasInteracted = false; // ��ȣ�ۿ� ���θ� Ȯ���ϴ� ����

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

        #region �������
        var chaseRoot = BTRoot.Add(new BTNode_Condition("Can Chase",
            () =>
            {
                if (DroppedObject.Instance.CheckSpecialObjectCondition())
                {
                    Chase_CurrentTarget = BasicManager.Instance.PlayerTarget;

                    return true; // Coroutine�� �Ϸ�� ������ ��ٸ�
                }

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

                // Chase_CurrentTarget null üũ
                if (Chase_CurrentTarget == null)
                {
                    Debug.LogError("Chase_CurrentTarget is null. Cannot chase.");
                    return BehaviourTree.ENodeStatus.Failed;  // ������ ���� ��ȯ
                }

                // Agent null üũ
                if (giant == null)
                {
                    Debug.LogError("NavMeshAgent is null.");
                    return BehaviourTree.ENodeStatus.Failed;  // ������ ���� ��ȯ
                }

                // ������ �ɾ� �ִ� ���, ���� ������ ����
                if (anim.GetBool("Sitting"))
                {
                    Agent.StandUpChase();
                    restCondition.ResetCondition();
                }
                if (restCondition.IsStandingUp())
                {
                    Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                }

                // Ÿ���� ���� ���� ����
                if (Chase_CurrentTarget != null)
                {
                    // NavMesh ��� ���
                    NavMeshPath path = new NavMeshPath();
                    if (giant.CalculatePath(Chase_CurrentTarget.transform.position, path))
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            // ��ΰ� ��ȿ�ϸ� �̵� ����
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


                if (Sensors.IsPlayerFullyDetected() == true) // IsPlayerFullyDetected�� ���� �޼ҵ��, ���� ���� �ʿ�
                {
                    Agent.AttackToPlayer(Chase_CurrentTarget.gameObject); // �÷��̾� ���� ����

                    if (Sensors.IsPlayerMissingDetected() == true)
                    {
                        Agent.MissingPlayer(Chase_CurrentTarget.transform.position);

                        return BehaviourTree.ENodeStatus.Succeeded;
                    }

                    return BehaviourTree.ENodeStatus.InProgress; 
                }
             

                // ���� ����
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

        #region �ɱ���
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

        #region Ž�����
        // ������ ��ü ���� ���θ� �Ǻ��ϴ� ���� ���
        var droppedObjectCondition = BTRoot.Add(new BTNode_Condition("Check Dropped Objects",
           () =>
           {
               bool conditionMet = DroppedObject.Instance.CheckCondition();
               Debug.Log($"Searching Condition node evaluated: {conditionMet}");
               return conditionMet;
           }));

        // ������ ��ü�� ���� ��ȣ�ۿ��� ó���ϴ� �׼� ���
        droppedObjectCondition.Add<BTNode_Action>("Interact With Dropped Object",
            () =>
            {
                if (hasInteracted)
                {
                    // �̹� ��ȣ�ۿ��� �Ϸ�Ǿ����Ƿ� �� �̻� ó������ ����
                    return BehaviourTree.ENodeStatus.Succeeded;
                }

                /*
                // Ž�� �߿� Ư���� ������ �����Ǹ� ���� ���·� ��ȯ
                if (DroppedObject.Instance.CheckSpecialObjectCondition())
                {
                    // ������ ����� �����ϰ� Ž�� ��带 ��������
                    Chase_CurrentTarget = BasicManager.Instance.PlayerTarget;
                    Debug.Log("Ž�� ��忡�� ���� ���·� ��ȯ��.");
                    return BehaviourTree.ENodeStatus.Succeeded; // Ž���� �Ϸ��ϰ� ��带 ��������
                }
                */

                // ������ �ɾ� �ִ� ���, ���� ������ ����
                if (anim.GetBool("Sitting"))
                {
                    Agent.StandUpSearch(); // Agent�� ���ִ� ���·� ����
                    restCondition.ResetCondition(); // �޽� ���� �ʱ�ȭ
                }

                // ���� Ž�� ���� ������ ��ü�� �ִ��� Ȯ���ϰ�, ������ ���ο� ��ü�� Ž�� ����
                if (!Agent.IsSearching() && DroppedObject.Instance.GetDroppedObjectsCount() > 0)
                {
                    LinkedAI.OnSearching(); // Ž�� ���·� ����
                    Agent.SearchingObject(); // ������ ��ü�� ��ȣ�ۿ� ����
                    return BehaviourTree.ENodeStatus.InProgress; // ��ȣ�ۿ��� ���� ������ ��Ÿ��
                }

                // Ž���� �Ϸ�Ǿ����� Ȯ��
                if (Agent.IsSearching())
                {
                    hasInteracted = true; // ��ȣ�ۿ� �Ϸ� �÷��� ����
                    // ��� ������ ��ü���� ��ȣ�ۿ��� �Ϸ�Ǹ� ���� ���� ��ȯ
                    return BehaviourTree.ENodeStatus.Succeeded;
                }


                // ���� Ž�� ���̶�� InProgress ���� ����
                return BehaviourTree.ENodeStatus.InProgress;
            });
        #endregion

        #region �������
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

    // 2�� ������ �� ������ �����ϴ� Coroutine
    IEnumerator StartChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 2�� ���
        Chase_CurrentTarget = BasicManager.Instance.PlayerTarget;
        // ���⼭ ������ �����ϴ� ������ �����ϰų�, ���� ���� ������ true�� ����
    }

}