using UnityEngine;
using DiasGames.Components;
using DiasGames.Climbing;

namespace DiasGames.Abilities
{
    public class ClimbLadderAbility : AbstractAbility
    {
        [Header("Overlap")]
        [SerializeField] private LayerMask ladderMask; // ��ٸ��� ��ġ���� �����ϱ� ���� ���̾� ����ũ
        [SerializeField] private Transform grabReference; // ��ٸ��� ���� ��ġ
        [SerializeField] private float overlapRange = 1f; // ��ٸ� ���� ����
        [Header("Animation")]
        [SerializeField] private string climbLadderAnimState = "Ladder"; // ��ٸ� Ÿ�� �ִϸ��̼� ����
        [SerializeField] private string climbUpAnimState = "Climb.Climb up"; // ��ٸ��� �ö󰡴� �ִϸ��̼� ����
        [SerializeField] private string ladderAnimFloat = "Vertical"; // ��ٸ� Ÿ�� �ִϸ��̼ǿ� ���޵Ǵ� ���� �Է�
        [Header("Movement")]
        [SerializeField] private float climbSpeed = 1.2f; // ��ٸ��� Ÿ�� �ӵ�
        [SerializeField] private float charOffset = 0.3f; // ĳ���Ϳ� ��ٸ� ������ �Ÿ�
        [SerializeField] private float smoothnessTime = 0.12f; // ��ٸ��� �ε巯�� �̵��� ���� �ð�

        private IMover _mover;
        private ICapsule _capsule;

        private Ladder _currentLadder;
        private Ladder _blockedLadder;
        private float _blockedTime;

        private bool _stopDown;
        private bool _stopUp;
        private bool _climbingUp;

        // ��ٸ��� �ε巯�� ��ġ�� ȸ���� �����ϱ� ���� �Ű� ����
        private Vector3 _startPosition, _targetPosition;
        private Quaternion _startRotation, _targetRotation;
        private float _step;
        private float _weight;

        private void Awake()
        {
            _mover = GetComponent<IMover>();
            _capsule = GetComponent<ICapsule>();
        }

        public override bool ReadyToRun()
        {
            return !_mover.IsGrounded() && FoundLadder(); // ���� ���� �ʰ� ��ٸ��� ã�� ���
        }

        public override void OnStartAbility()
        {
            _animator.CrossFadeInFixedTime(climbLadderAnimState, 0.1f); // ��ٸ� Ÿ�� �ִϸ��̼� ���
            _mover.DisableGravity(); // �߷� ��Ȱ��ȭ

            // ��ġ ����
            _weight = 0;
            _step = 1 / smoothnessTime;
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _targetPosition = GetCharPosition();
            _targetRotation = GetCharRotation();

            // ���� ����
            _stopDown = false;
            _stopUp = false;
            _climbingUp = false;
        }

        public override void UpdateAbility()
        {
            // �ɷ��� ���۵ǰ� ĳ������ ��ġ�� ȸ���� �����ؾ� �ϴ� ���
            if (!Mathf.Approximately(_weight, 1f))
            {
                UpdatePositionOnLadder();
                return;
            }

            // ��ٸ��� �ö󰡴� ���� ��ܿ� �����ϸ� �ִϸ��̼� ���� ���
            if (_climbingUp)
            {
                if (_animator.IsInTransition(0)) return;

                var state = _animator.GetCurrentAnimatorStateInfo(0);
                var normalizedTime = Mathf.Repeat(state.normalizedTime, 1f);
                if (normalizedTime > 0.95f)
                    StopAbility();

                return;
            }

            // ���� �Է�
            float vertical = _action.move.y;

            // ���� �Ѱ� Ȯ��
            CheckVerticalLimits();

            // �Ʒ��� �Ѱ迡 �����ϸ� �Ʒ������� �̵� ����
            if (_stopDown && vertical < 0) vertical = 0;

            // ��� �Ѱ迡 ������ ���
            if (_stopUp && vertical > 0)
            {
                // ��ٸ��� �� ���� �ö� �� �ִ� ���
                if (_currentLadder.CanClimbTop)
                {
                    _animator.CrossFadeInFixedTime(climbUpAnimState, 0.1f); // �ö󰡴� �ִϸ��̼� ���
                    _mover.ApplyRootMotion(Vector3.one);
                    _capsule.DisableCollision(); // �ݶ��̴� �浹 ��Ȱ��ȭ
                    _climbingUp = true;
                    return;
                }

                // ��ٸ� �� ���� ������ �� �ö󰡴� ���� ����
                vertical = 0;
            }

            // ��ٸ� Ÿ�� �ִϸ��̼ǿ� ���� �Է� ����
            _animator.SetFloat(ladderAnimFloat, vertical, 0.1f, Time.deltaTime);
            // ĳ���͸� ���Ʒ��� �̵�
            _mover.Move(Vector3.up * vertical * climbSpeed);

            // �Ʒ��� �̵� ���̰� ���� ������ ��� �ɷ� ����
            if (_action.move.y < 0f && _mover.IsGrounded())
                StopAbility();

            // ��ٸ����� ��������
            if (_action.drop)
            {
                StopAbility();
                BlockLadder(); // ��ٸ� ���
            }
        }

        public override void OnStopAbility()
        {
            _mover.EnableGravity(); // �߷� Ȱ��ȭ
            _capsule.EnableCollision(); // �ݶ��̴� �浹 Ȱ��ȭ
            _mover.StopRootMotion(); // ��Ʈ ��� ����
        }

        /// <summary>
        /// �ε巯�� ��ȯ�� ����Ͽ� ��ġ�� ȸ�� ����
        /// </summary>
        private void UpdatePositionOnLadder()
        {
            _weight = Mathf.MoveTowards(_weight, 1f, _step * Time.deltaTime);
            _mover.SetPosition(Vector3.Lerp(_startPosition, _targetPosition, _weight));
            transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, _weight);
        }

        /// <summary>
        /// ��ٸ����� ������ �� ���� ��ٸ��� ����մϴ�.
        /// </summary>
        private void BlockLadder()
        {
            _blockedLadder = _currentLadder;
            _blockedTime = Time.time;
        }

        /// <summary>
        /// ��ٸ��� ���� �Ѱ踦 Ȯ���Ͽ� ��ٸ��� �Ѿ� �ö󰡰ų� �������� ���� �����մϴ�.
        /// </summary>
        private void CheckVerticalLimits()
        {
            if (transform.position.y < _currentLadder.BottomLimit.position.y)
                _stopDown = true;
            else if (transform.position.y > _currentLadder.BottomLimit.position.y + 0.15f)
                _stopDown = false;

            if (transform.position.y + _capsule.GetCapsuleHeight() > _currentLadder.TopLimit.position.y)
                _stopUp = true;
            else if (transform.position.y + _capsule.GetCapsuleHeight() < _currentLadder.TopLimit.position.y - 0.15f)
                _stopUp = false;
        }

        /// <summary>
        /// ��ٸ��� ã���ϴ�.
        /// </summary>
        private bool FoundLadder()
        {
            var overlaps = Physics.OverlapSphere(grabReference.position, overlapRange, ladderMask, QueryTriggerInteraction.Collide);

            // ��� �浹ü Ȯ��
            foreach (var coll in overlaps)
            {
                if (coll.TryGetComponent(out Ladder ladder))
                {
                    if (ladder == _blockedLadder && Time.time - _blockedTime < 2f)
                        continue;

                    if (CanGrab(ladder))
                    {
                        _currentLadder = ladder;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// ĳ���Ͱ� ���� ��ġ�� ȸ������ ��ٸ��� ���� �� �ִ��� Ȯ���մϴ�.
        /// </summary>
        private bool CanGrab(Ladder ladder)
        {
            // ĳ���Ͱ� ��ٸ��� �ٶ󺸰� �ִ��� Ȯ��
            if (Vector3.Dot(transform.forward, -ladder.PositionAndDirection.forward) < -0.1f) return false;

            // �ϴ� Ȯ��
            if (transform.position.y < ladder.BottomLimit.position.y - 0.15f) return false;

            // ��� Ȯ��
            if (transform.position.y + _capsule.GetCapsuleHeight() > ladder.TopLimit.position.y + 0.15f) return false;

            return true;
        }

        /// <summary>
        /// �� ��ٸ����� ĳ������ ��ǥ ��ġ�� �����ɴϴ�.
        /// </summary>
        private Vector3 GetCharPosition()
        {
            Vector3 position = _currentLadder.PositionAndDirection.position + _currentLadder.PositionAndDirection.forward * charOffset;
            position.y = transform.position.y;

            if (position.y < _currentLadder.BottomLimit.position.y)
                position.y = _currentLadder.BottomLimit.position.y;

            float height = _capsule.GetCapsuleHeight();
            if (position.y + height > _currentLadder.TopLimit.position.y)
                position.y = _currentLadder.TopLimit.position.y - height;

            return position;
        }

        /// <summary>
        /// �� ��ٸ����� ĳ������ ��ǥ ȸ���� �����ɴϴ�.
        /// </summary>
        private Quaternion GetCharRotation()
        {
            return Quaternion.LookRotation(-_currentLadder.PositionAndDirection.forward);
        }
    }
}