using UnityEngine;

namespace DiasGames.Puzzle
{
    public class LeverTrigger : MonoBehaviour, IPushable
    {
        [SerializeField] private Transform targetCharacterTransform = null;
        [SerializeField] private Transform rightIK = null;
        [SerializeField] private Transform leftIK = null;
        [SerializeField] private Transform rightIKTarget = null; // �ִϸ��̼� ����� ������ Ÿ��
        [SerializeField] private Transform leftIKTarget = null;  // �ִϸ��̼� ����� ���� Ÿ��
        [Space]
        [SerializeField] private Collider characterRefCollider;
        public Transform playerTarget;

        private PushableObject _block = null;

        private void Awake()
        {
            _block = GetComponentInParent<PushableObject>();
            characterRefCollider.enabled = false;
        }

        private void Update()
        {
            // IK Ÿ���� �ִϸ��̼� ��� ��Ī��Ŵ
            if (rightIKTarget != null && leftIKTarget != null)
            {
                rightIK.position = rightIKTarget.position;
                rightIK.rotation = rightIKTarget.rotation;

                leftIK.position = leftIKTarget.position;
                leftIK.rotation = leftIKTarget.rotation;
            }
        }

        public void StartPush()
        {
            _block.EnablePhysics();
            characterRefCollider.enabled = true;
        }

        public void StopPush()
        {
            _block.DisablePhysics();
            characterRefCollider.enabled = false;
        }

        public void SetPushState(bool pushing)
        {
            if (pushing)
                _block.EnablePhysics();
            else
                _block.DisablePhysics();
        }


        #region IHandIk & ICharacterTargetPos

        public Transform GetLeftHandTarget()
        {
            return leftIK;
        }

        public Transform GetRightHandTarget()
        {
            return rightIK;
        }

        public Transform GetTarget()
        {
            playerTarget.position = targetCharacterTransform.position;
            return targetCharacterTransform;
        }

        #endregion
    }
}