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
    [SerializeField] private Transform chairTransform; // ������ Transform

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

        #region �������
        var chaseRoot = BTRoot.Add(new BTNode_Condition("Can Chase",
            () =>
            {
                if (DroppedObject.Instance.CheckSpecialObjectCondition())
                {
                    StartCoroutine(StartChaseAfterDelay(2f));

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
                // ������ �ɾ� �ִ� ���, ���� ������ ����
                if (anim.GetBool("SittingToWalk"))
                {
                    Agent.StandUpChase();
                    restCondition.ResetCondition();
                }
                if (restCondition.IsStandingUp())
                {
                    Agent.MoveToRun(Chase_CurrentTarget.transform.position);
                }

                if (Sensors.IsPlayerFullyDetected() == true) // IsPlayerFullyDetected�� ���� �޼ҵ��, ���� ���� �ʿ�
                {
                    Agent.AttackToPlayer(Chase_CurrentTarget.gameObject); // �÷��̾� ���� ����
                    return BehaviourTree.ENodeStatus.Succeeded; // ���� �� ���¸� �������� �����Ͽ� ���� �ൿ���� �Ѿ �� �ֵ��� ��
                }

                // ���� ����
                Agent.MoveToRun(Chase_CurrentTarget.transform.position);

                return BehaviourTree.ENodeStatus.InProgress;
            },
            () =>
            {
                if (Sensors.IsPlayerFullyDetected() == true) // IsPlayerFullyDetected�� ���� �޼ҵ��, ���� ���� �ʿ�
                {
                    Agent.AttackToPlayer(Chase_CurrentTarget.gameObject); // �÷��̾� ���� ����
                    return BehaviourTree.ENodeStatus.Succeeded; // ���� �� ���¸� �������� �����Ͽ� ���� �ൿ���� �Ѿ �� �ֵ��� ��
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
               // �ɴ� �׼��� ���۵Ǹ�
               if (!anim.GetBool("SittingToWalk"))
                {
                   Agent.SitAtPosition(chairTransform.position, chairTransform.rotation);
                   return BehaviourTree.ENodeStatus.InProgress; // �׼� ���� ��
                }

               if (Sensors.ActiveTargets.Count == 0 && restCondition.CheckCondition())
                {
                   // ������ üũ�Ͽ� ����� �����ٸ� Succeeded ��ȯ
                   return BehaviourTree.ENodeStatus.Succeeded;
                }

                // ���� ������ �� InProgress ����
                return BehaviourTree.ENodeStatus.InProgress;
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
                // ������ �ɾ� �ִ� ���, ���� ������ ����
                if (anim.GetBool("SittingToWalk"))
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