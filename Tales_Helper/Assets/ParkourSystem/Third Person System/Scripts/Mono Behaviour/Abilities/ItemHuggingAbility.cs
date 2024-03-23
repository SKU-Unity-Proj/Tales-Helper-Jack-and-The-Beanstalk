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
        public Transform targetPos; // 아이템 안고 있을 위치
        public GameObject pickItem = null; // 주운 아이템 등록
        private bool haveItem = false; // 아이템 들고 있는지 상태 체크

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
            SetAnimationState("ItemLift", 0.2f);
            //Debug.Log("LiftAnim");
            
            Invoke("StartIdle", 2f);

            CheckItem();
        }


        public override void UpdateAbility() // 실행 중에 계속 호출
        {
            _mover.Move(_action.move, speed);

            HuggingItem();

            if (_action.pickUp && haveItem && pickItem != null) // 아이템을 들고 있을때 E키
            {
                pickItem.transform.localPosition = new Vector3(0f,0f,0.3f);
                pickItem.transform.SetParent(null);
                pickItem = null;

                SetLayerPriority(1, 0); // 상체 애니메이션 우선순위 낮추기 (애니메이션 끄기)
            }
        }





        private void StartIdle() // Hugging_Idle 애니메이션 실행
        {
            SetAnimationState("ItemHugging_Idle", 0.25f, 1);
        }

        private void CheckItem() // 바닥에 아이템을 주울 수 있는지 판별
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