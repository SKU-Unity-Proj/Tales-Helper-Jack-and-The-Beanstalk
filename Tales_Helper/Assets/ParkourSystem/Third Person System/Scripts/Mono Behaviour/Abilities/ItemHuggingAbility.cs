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
        public Transform targetPos; // 아이템 안고 있을 위치
        public GameObject pickItem = null; // 주운 아이템 등록
        public bool haveItem = false; // 아이템 들고 있는지 상태 체크
        private Rigidbody itemRigid; 

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

            if(haveItem) // 아이템이 있을때만 애니메이션 실행
            {
                SetAnimationState("ItemLift", 0.2f);
                //Debug.Log("LiftAnim");

                Invoke("StartIdle", 2f);
            }
        }


        public override void UpdateAbility() // 실행 중에 계속 호출
        {
            _mover.Move(_action.move, speed);

            HuggingItem();

            if(_action.pickUp) // E키를 다시 누르면 어빌리티 초기화
                StopAbility();

            if (!haveItem) // 아이템이 없으면 어빌리티 초기화
                StopAbility();
        }

        public override void OnStopAbility() // 멈출때 호출
        {
            if (pickItem != null) // 물건 내려놓고 상태 초기화
            {
                pickItem.transform.position = targetPos.transform.position + targetPos.transform.forward * 0.5f;
                itemRigid.velocity = Vector3.zero;
                itemRigid.angularVelocity = Vector3.zero;

                pickItem.transform.parent = null;

                pickItem = null;
                haveItem = false;

                SetLayerPriority(1, 0); // 상체 애니메이션 우선순위 낮추기 (애니메이션 끄기)
            }
        }





        private void StartIdle() // Hugging_Idle 애니메이션 실행
        {
            SetAnimationState("ItemHugging_Idle", 0.25f, 1);
        }

        private void CheckItem() // 바닥에 아이템을 주울 수 있는지 판별
        {
            if (!haveItem) // 아이템을 가지고 있지 않을때
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

        void HuggingItem() // 아이템을 타겟 위치 자식으로 보내기
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

// 아이템을 내려놓으면 땅 아래로 뚫고 내려감
// 줍는 애니메이션이 실행중일때 움직여짐
// 애니메이션 상하체 분리