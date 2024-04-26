using DiasGames.Components;
using NPOI.SS.Formula.Functions;
using UnityEngine;

namespace DiasGames.Abilities
{
    public class PickItemAbility : AbstractAbility
    {
        private IMover _mover = null;

        public GameObject pickItem = null; // �ֿ� ������ ���
        public GameObject pickItem_light = null; // �ֿ� ������ ����Ʈ ���
        public Transform targetPos; // ������ �Ȱ� ���� ��ġ
        public float radius = 1f; // �������� �ִ��� üũ�ϴ� ���� ũ��

        public GameObject lightControl;


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

            if (pickItem != null)
            {
                _mover.StopMovement();
                SetAnimationState("PickItem", 0.2f);
            }
        }


        public override void UpdateAbility() // ���� �߿� ��� ȣ��
        {
            if (_action.pickUp) // EŰ�� �ٽ� ������ �����Ƽ ����
                StopAbility();
        }

        public override void OnStopAbility() // ���⶧ ȣ��
        {
            lightControl.SetActive(false);
        }


        /// <summary>
        /// �����Ƽ ��. �Ϲ� �Լ���
        /// </summary>

        void CheckItem()
        {
            Collider collider = Physics.OverlapSphere(transform.position + transform.forward, radius, 1 << 9)[0];

            if (collider != null)
            {
                pickItem = collider.gameObject;
                pickItem_light = collider.transform.GetChild(1).gameObject;
                pickItem_light.SetActive(false);
            }
            else
            {
                pickItem = null;
            }
        }
    }
}

