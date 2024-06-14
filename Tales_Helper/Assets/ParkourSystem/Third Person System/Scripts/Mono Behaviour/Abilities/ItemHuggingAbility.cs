using DiasGames.Abilities;
using DiasGames.Components;
using UnityEngine.SceneManagement; // Add this line
using System.Collections;
using DiasGames.Debugging;
using TMPro;
using UnityEngine;

//ItemHugPos Position(0, -0.037, 0.336)

namespace DiasGames.Abilities
{
    [DisallowMultipleComponent]
    public class ItemHuggingAbility : AbstractAbility
    {
        private IMover _mover = null;

        [SerializeField] private float speed = 3f; // �������� ��� ������ �ӵ�

        [SerializeField] private float radius = 1f; // �������� �ִ��� üũ�ϴ� ���� ũ��
        public Transform targetPos; // ������ �Ȱ� ���� ��ġ
        public GameObject pickItem = null; // �ֿ� ������ ���
        public bool haveItem = false; // ������ ��� �ִ��� ���� üũ
        public bool liftingWait = false; // ������ ��� ��ٸ��� ���� ������ ����
        private Rigidbody itemRigid; // �������� ������ٵ�

        // ���� �Ű�����
        [Header("Jump parameters")]
        [SerializeField] private float jumpHeight = 1.2f; // ���� ����
        [SerializeField] private float speedOnAir = 6f; // ���߿����� �ӵ�
        [SerializeField] private float airControl = 0.5f; // ���� ����

        [SerializeField] private Transform originBottle;

        private bool isJump = false;
        private float _startSpeed;
        private Vector2 _startInput;

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

            if(haveItem)
            {
                _mover.StopMovement(); // velocity 0

                SetAnimationState("ItemLift", 0.2f);

                Invoke("StartIdle", 2f);
            }
        }


        public override void UpdateAbility() // ���� �߿� ��� ȣ��
        {
            if(liftingWait)
            {
                _mover.Move(_action.move, speed);

                HuggingItem();
                //originBottle.gameObject.GetComponent<MeshRenderer>().enabled = true;
                //originBottle.gameObject.GetComponent<MeshCollider>().enabled = true;
            }

            if (pickItem == null)
            {
                StopAbility();
                return;
            }

            // If the item's tag is "duck", start the coroutine to switch scenes
            if (pickItem.CompareTag("duck"))
            {
                StartCoroutine(SwitchToEndingScene());
            }

            if (_action.pickUp) // EŰ�� �ٽ� ������ �����Ƽ ����
            {
                StopAbility();
                return;
            }
                
            if (!pickItem.activeSelf) // �������� ������ �����Ƽ ����
            {
                StopAbility();
                return ;
            }
                
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("ItemLift")) // �������� �ִϸ��̼��� IsName�̸�
                if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)  // �ִϸ��̼��� ������� �Ϸ� �Ǿ�����
                    liftingWait = true;
            
            if (_mover.IsGrounded())
            {
                if (isJump && _mover.GetVelocity().y < -3f) // ���� �� ����
                {
                    SetAnimationState("Grounded", 0.25f);
                    isJump = false;
                }
                    
                if (_action.jump)
                    PerformJump(); // ���� ����
            } 
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
                liftingWait = false;

                SetLayerPriority(1, 0); // ��ü �ִϸ��̼� �켱���� ���߱� (�ִϸ��̼� ����)

                //SetAnimationState("ThrowItem", 0.5f, 0);
                SetAnimationState("Grounded", 0.3f, 1);
            }
        }



        /// <summary>
        /// �����Ƽ ��. �Ϲ� �Լ���
        /// </summary>

        private void StartIdle()
        {
            SetAnimationState("ItemHugging_Idle", 0.3f, 1);
            SetLayerPriority(1, 1); // ��ü �ִϸ��̼� �켱���� ���̱� (�ִϸ��̼� �ѱ�)
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

                
            }
        }

        private void PerformJump()
        {
            Vector3 velocity = _mover.GetVelocity(); // ���� �ӵ� ��������
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * _mover.GetGravity()); // ������ �ʿ��� �ӵ� ���

            _mover.SetVelocity(velocity); // �ӵ� ����
            _animator.CrossFadeInFixedTime("Air.Jump", 0.0f); // ���� �ִϸ��̼� ���
            _startSpeed = speedOnAir; // ���� �ӵ� ����

            if (_startInput.magnitude > 0.1f)
                _startInput.Normalize(); // �Է� ����ȭ

            isJump = true; //���� ������ �˸�
        }

        private IEnumerator SwitchToEndingScene()
        {
            yield return new WaitForSeconds(3f); // 3�� ���
            SceneManager.LoadScene("EndingAnimation"); // EndingAnimation ������ ��ȯ
        }
    }
}

// �������� ���������� �� �Ʒ��� �հ� ������
// �ݴ� �ִϸ��̼��� �������϶� ��������
// �ִϸ��̼� ����ü �и�