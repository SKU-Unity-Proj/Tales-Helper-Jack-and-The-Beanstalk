using UnityEngine;
using DiasGames.Components;

namespace DiasGames.Abilities
{
    public class JumpInBed : MonoBehaviour
    {
        private bool onBed;
        private bool onBedPrevState; // ���� �������� onBed ���¸� ������ ����
        private bool canJumpCombo = false;
        private int jumpCombo = 0;

        private float GroundedOffset = -0.2f;
        private float GroundedRadius = 0.28f;
        public LayerMask BedLayers;

        public AirControlAbility airControlAbility;

        private float timer = 0f; // ������ ������ �� ����� �ð�

        private void Update()
        {
            CheckBed();
            CheckBedCondition();
            JumpComboLogic();
        }

        private void CheckBed()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            onBed = Physics.CheckSphere(spherePosition, GroundedRadius, BedLayers, QueryTriggerInteraction.Ignore);
        }

        private void CheckBedCondition()
        {
            // ���� �����Ӱ� ���� �������� onBed ���°� �ٸ���, ���� �����ӿ��� onBed�� true�� ��
            if (onBed != onBedPrevState && onBed)
            {
                // ����� ���� 0.5�� ���� ����
                if (timer < 0.5f)
                {
                    canJumpCombo = true;

                    timer += Time.deltaTime; // �ð� ����
                }
                else
                {
                    timer = 0f; // �ð� �ʱ�ȭ
                    canJumpCombo = false;
                }
            }

            onBedPrevState = onBed; // ���� �������� onBed ���¸� ���� ���������� ����
        }

        private void JumpComboLogic()
        {
            if (canJumpCombo)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpCombo++;

                    //jumpCombo�� ���� ���� ���� ���
                    switch (jumpCombo)
                    {
                        case 1:
                            Debug.Log("ù ��° ����! �⺻ ���̷� �����մϴ�.");
                            airControlAbility.jumpHeight = 1.5f;
                            break;
                        case 2:
                            Debug.Log("�� ��° ����! ���̰� ������ ������ �մϴ�.");
                            airControlAbility.jumpHeight = 3f;
                            break;
                        case 3:
                            Debug.Log("�� ��° ����! �� ���� �����մϴ�.");
                            airControlAbility.jumpHeight = 4f;
                            break;
                        default:
                            Debug.Log("������ �ִ� ���� ���̿� �����߽��ϴ�!");

                            break;
                    }
                }
            }
        }
    }
}
