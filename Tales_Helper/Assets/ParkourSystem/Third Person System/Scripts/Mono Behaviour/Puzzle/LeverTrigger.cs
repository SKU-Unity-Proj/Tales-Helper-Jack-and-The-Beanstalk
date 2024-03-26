using UnityEngine;

namespace DiasGames.Puzzle
{
    public class LeverTrigger : MonoBehaviour, IPushable
    {
        [SerializeField] private Transform targetCharacterTransform = null;
        [SerializeField] private Transform rightIK = null;
        [SerializeField] private Transform leftIK = null;
        [SerializeField] private Transform rightIKTarget = null; // 애니메이션 대상의 오른쪽 타겟
        [SerializeField] private Transform leftIKTarget = null;  // 애니메이션 대상의 왼쪽 타겟
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
            // IK 타겟을 애니메이션 대상에 매칭시킴
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