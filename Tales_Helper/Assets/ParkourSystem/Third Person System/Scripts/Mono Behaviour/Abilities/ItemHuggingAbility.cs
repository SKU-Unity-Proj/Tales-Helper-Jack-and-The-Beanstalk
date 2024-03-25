using DiasGames.Abilities;
using DiasGames.Components;
using DiasGames.Debugging;
using TMPro;
using UnityEngine;

namespace DiasGames.Abilities
{
    [DisallowMultipleComponent]
    public class ItemHuggingAbility : AbstractAbility
    {
        private IMover _mover = null;

        [SerializeField] private float speed = 3f;

        [SerializeField] private float radius = 1f;
        public Transform targetPos; // ������ �Ȱ� ���� ��ġ
        public GameObject pickItem = null; // �ֿ� ������ ���
        public bool haveItem = false; // ������ ��� �ִ��� ���� üũ
        private Rigidbody itemRigid; 

        private void Awake()
        {
            _mover = GetComponent<IMover>();
        }

        public override bool ReadyToRun() // ���� ����
        {
            return _action.pickUp;
        }

        public override void OnStartAbility() // ����� �� ȣ��
        {
            CheckItem();

            if(haveItem) // �������� �������� �ִϸ��̼� ����
            {
                SetAnimationState("ItemLift", 0.2f);
                //Debug.Log("LiftAnim");

                Invoke("StartIdle", 2f);
            }
        }


        public override void UpdateAbility() // ���� �߿� ��� ȣ��
        {
            _mover.Move(_action.move, speed);

            HuggingItem();

            if(_action.pickUp) // EŰ�� �ٽ� ������ �����Ƽ �ʱ�ȭ
                StopAbility();

            if (!haveItem) // �������� ������ �����Ƽ �ʱ�ȭ
                StopAbility();
        }

        public override void OnStopAbility() // ���⶧ ȣ��
        {
            if (pickItem != null) // ���� �������� ���� �ʱ�ȭ
            {
                pickItem.transform.position = targetPos.transform.position + targetPos.transform.forward * 0.5f;
                itemRigid.velocity = Vector3.zero;
                itemRigid.angularVelocity = Vector3.zero;

                pickItem.transform.parent = null;

                pickItem = null;
                haveItem = false;

                SetLayerPriority(1, 0); // ��ü �ִϸ��̼� �켱���� ���߱� (�ִϸ��̼� ����)
            }
        }





        private void StartIdle() // Hugging_Idle �ִϸ��̼� ����
        {
            SetAnimationState("ItemHugging_Idle", 0.25f, 1);
        }

        private void CheckItem() // �ٴڿ� �������� �ֿ� �� �ִ��� �Ǻ�
        {
            if (!haveItem) // �������� ������ ���� ������
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, radius, 1 << 9);

                if (colliders != null)
                {
                    foreach (Collider collider in colliders)
                    {
                        haveItem = true;
                        pickItem = collider.gameObject;
                        itemRigid = pickItem.GetComponent<Rigidbody>();
                    }
                }
                else
                {
                    pickItem = null;
                    haveItem = false;
                }
            }
        }

        void HuggingItem() // �������� Ÿ�� ��ġ �ڽ����� ������
        {
            if (haveItem && pickItem != null)
            {
                pickItem.transform.SetParent(targetPos);
                pickItem.transform.localPosition = Vector3.zero;
                pickItem.transform.localRotation = Quaternion.Euler(new Vector3(16f, -95f, 0f));
            }
        }
    }
}

// �������� ���������� �� �Ʒ��� �հ� ������
// �ݴ� �ִϸ��̼��� �������϶� ��������
// �ִϸ��̼� ����ü �и�