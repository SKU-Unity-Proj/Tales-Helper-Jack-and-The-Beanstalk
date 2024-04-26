using DiasGames.Components;
using NPOI.SS.Formula.Functions;
using UnityEngine;

namespace DiasGames.Abilities
{
    public class PickItemAbility : AbstractAbility
    {
        private IMover _mover = null;

        public GameObject pickItem = null; // 주운 아이템 등록
        public GameObject pickItem_light = null; // 주운 아이템 라이트 등록
        public Transform targetPos; // 아이템 안고 있을 위치
        public float radius = 1f; // 아이템이 있는지 체크하는 원의 크기

        public GameObject lightControl;


        private void Awake()
        {
            _mover = GetComponent<IMover>();
        }

        public override bool ReadyToRun() // 실행 조건
        {
            return _action.pickUp;
        }

        public override void OnStartAbility() // 실행될 때 호출
        {
            CheckItem();

            if (pickItem != null)
            {
                _mover.StopMovement();
                SetAnimationState("PickItem", 0.2f);
            }
        }


        public override void UpdateAbility() // 실행 중에 계속 호출
        {
            if (_action.pickUp) // E키를 다시 누르면 어빌리티 중지
                StopAbility();
        }

        public override void OnStopAbility() // 멈출때 호출
        {
            lightControl.SetActive(false);
        }


        /// <summary>
        /// 어빌리티 끝. 일반 함수들
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

