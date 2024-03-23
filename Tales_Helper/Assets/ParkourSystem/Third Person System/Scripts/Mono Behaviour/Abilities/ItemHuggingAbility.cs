using DiasGames.Abilities;
using DiasGames.Components;
using DiasGames.Debugging;
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
        private bool haveItem = false; // ������ ��� �ִ��� ���� üũ

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
            SetAnimationState("ItemLift", 0.2f);
            //Debug.Log("LiftAnim");
            
            Invoke("StartIdle", 2f);

            CheckItem();
        }


        public override void UpdateAbility() // ���� �߿� ��� ȣ��
        {
            _mover.Move(_action.move, speed);

            HuggingItem();

            if (_action.pickUp && haveItem && pickItem != null) // �������� ��� ������ EŰ
            {
                pickItem.transform.localPosition = new Vector3(0f,0f,0.3f);
                pickItem.transform.SetParent(null);
                pickItem = null;

                SetLayerPriority(1, 0); // ��ü �ִϸ��̼� �켱���� ���߱� (�ִϸ��̼� ����)
            }
        }





        private void StartIdle() // Hugging_Idle �ִϸ��̼� ����
        {
            SetAnimationState("ItemHugging_Idle", 0.25f, 1);
        }

        private void CheckItem() // �ٴڿ� �������� �ֿ� �� �ִ��� �Ǻ�
        {
            if (!haveItem)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, radius, 1 << 9);

                if (colliders.Length > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        Debug.Log("Overlap detected with: " + collider.name);

                        haveItem = true;
                        pickItem = collider.gameObject;
                    }
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