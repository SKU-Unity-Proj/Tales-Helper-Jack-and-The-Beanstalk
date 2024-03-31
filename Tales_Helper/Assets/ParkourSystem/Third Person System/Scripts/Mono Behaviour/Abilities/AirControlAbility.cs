using UnityEngine;
using UnityEngine.Events;
using DiasGames.Components;

namespace DiasGames.Abilities
{
    public class AirControlAbility : AbstractAbility
    {
        // �ִϸ��̼� ����
        [Header("Animation State")]
        [SerializeField] private string animJumpState = "Air.Jump"; // ���� �ִϸ��̼� ����
        [SerializeField] private string animFallState = "Air.Falling"; // ���� �ִϸ��̼� ����
        [SerializeField] private string animHardLandState = "Air.Hard Land"; // ���ϰ� �����ϴ� �ִϸ��̼� ����

        // ���� �Ű�����
        [Header("Jump parameters")]
        [SerializeField] private float jumpHeight = 1.2f; // ���� ����
        [SerializeField] private float speedOnAir = 6f; // ���߿����� �ӵ�
        [SerializeField] private float airControl = 0.5f; // ���� ����

        // ����
        [Header("Landing")]
        [SerializeField] private float heightForHardLand = 3f; // ���ϰ� �����ϱ� ���� ����
        [SerializeField] private float heightForKillOnLand = 7f; // ������ ���� ��� ����

        // ���� ȿ��
        [Header("Sound FX")]
        [SerializeField] private AudioClip jumpEffort; // ���� ȿ����
        [SerializeField] private AudioClip hardLandClip; // ���ϰ� �����ϴ� ȿ����

        // �̺�Ʈ
        [Header("Event")]
        [SerializeField] private UnityEvent OnLanded = null; // ���� �̺�Ʈ

        private IMover _mover = null;
        private IDamage _damage;
        private CharacterAudioPlayer _audioPlayer;

        private float _startSpeed;
        private Vector2 _startInput;

        private Vector2 _inputVel;
        private float _angleVel;

        private float _targetRotation;
        private Transform _camera;

        // ���� ���� ����
        private float _highestPosition = 0;
        private bool _hardLanding = false;

        private void Awake()
        {
            _mover = GetComponent<IMover>(); // �̵��� ������Ʈ ��������
            _damage = GetComponent<IDamage>(); // ������ ������Ʈ ��������
            _audioPlayer = GetComponent<CharacterAudioPlayer>(); // ĳ���� ����� �÷��̾� ��������
            _camera = Camera.main.transform; // ���� ī�޶��� Ʈ������ ��������
        }

        public override bool ReadyToRun()
        {
            return !_mover.IsGrounded() || _action.jump; // ���� ���� �ʰų� ���� �׼��� �ߵ��Ǿ��� �� ���� �غ� Ȯ��
        }

        public override void OnStartAbility()
        {
            _startInput = _action.move; // �ʱ� �Է°� ����
            _targetRotation = _camera.eulerAngles.y; // Ÿ�� ȸ���� ����

            if (_action.jump && _mover.IsGrounded()) // ���� �׼��� �ߵ��ǰ� ���� ���� ��
                PerformJump(); // ���� ����
            else
            {
                SetAnimationState(animFallState, 0.25f); // ���� �ִϸ��̼� ���·� ����
                _startSpeed = Vector3.Scale(_mover.GetVelocity(), new Vector3(1, 0, 1)).magnitude; // ���� �ӵ� ����

                _startInput.x = Vector3.Dot(_camera.right, transform.forward); // ī�޶� �����ʰ� ���� ������ ������ ����
                _startInput.y = Vector3.Dot(Vector3.Scale(_camera.forward, new Vector3(1, 0, 1)), transform.forward); // ī�޶� ���� ���Ϳ� ���� ������ ������ ����

                if (_startSpeed > 3.5f)
                    _startSpeed = speedOnAir; // ���� �ӵ��� �����Ǿ� ������ ���߿����� �ӵ��� ����
            }

            _highestPosition = transform.position.y; // �ְ� ���� ����
            _hardLanding = false; // ���ϰ� ���� ���� ����
        }

        public override void UpdateAbility()
        {
            if (_hardLanding)
            {
                // ��Ʈ ��� ����
                _mover.ApplyRootMotion(Vector3.one, false);

                // �ִϸ��̼��� �Ϸ�� ������ ���
                if (HasFinishedAnimation(animHardLandState))
                    StopAbility();

                return;
            }

            if (_mover.IsGrounded()) // ���� ���� ��
            {
                if (_highestPosition - transform.position.y >= heightForHardLand) // �ְ� ���̿��� ���ϰ� �����ϴ� ���
                {
                    _hardLanding = true; // ���ϰ� ���� ���·� ����
                    SetAnimationState(animHardLandState, 0.02f); // ���ϰ� ���� �ִϸ��̼� ���·� ����

                    // �̺�Ʈ ȣ��
                    OnLanded.Invoke();

                    // ���� ���
                    if (_audioPlayer)
                        _audioPlayer.PlayVoice(hardLandClip);

                    // ������ �߻�
                    if (_damage != null)
                    {
                        // ������ ���
                        float currentHeight = _highestPosition - transform.position.y - heightForHardLand;
                        float ratio = currentHeight / (heightForKillOnLand - heightForHardLand);

                        _damage.Damage((int)(500 * ratio));
                    }

                    return;
                }

                StopAbility(); // �ɷ� ����
            }

            if (transform.position.y > _highestPosition) // ���� ���̰� �ְ� ���̺��� ���� ��
                _highestPosition = transform.position.y; // �ְ� ���� ������Ʈ

            _startInput = Vector2.SmoothDamp(_startInput, _action.move, ref _inputVel, airControl); // �Է°� ������
            _mover.Move(_startInput, _startSpeed, false); // �̵� ����

            RotateCharacter(); // ĳ���� ȸ��
        }

        private void RotateCharacter()
        {
            // ����: Vector2�� != �����ڴ� �ٻ�ġ�� ����ϹǷ� �ε� �Ҽ��� ������ �ΰ�
            // ���� �ʰ� magnitude ���꺸�� ������
            // ������ �Է��� �ִ� ��� ĳ���͸� ȸ��
            if (_action.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(_startInput.x, _startInput.y) * Mathf.Rad2Deg + _camera.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _angleVel, airControl);

                // �Է� ���⿡ ���� ī�޶� ��ġ�� �������� ȸ��
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        public override void OnStopAbility()
        {
            base.OnStopAbility(); // �θ� Ŭ������ �ɷ� ���� �޼��� ȣ��

            // ���� �ְ� ���ϰ� �������� �ʾ����� ���� �ӵ��� -3f ������ ���
            if (_mover.IsGrounded() && !_hardLanding && _mover.GetVelocity().y < -3f)
                OnLanded.Invoke(); // ���� �̺�Ʈ ȣ��

            _hardLanding = false; // ���ϰ� ���� ���� �ʱ�ȭ
            _highestPosition = 0; // �ְ� ���� �ʱ�ȭ
            _mover.StopRootMotion(); // ��Ʈ ��� ����
        }

        /// <summary>
        /// ������ �����Ͽ� ������ٵ� ���� �߰��մϴ�.
        /// </summary>
        private void PerformJump()
        {
            Vector3 velocity = _mover.GetVelocity(); // ���� �ӵ� ��������
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * _mover.GetGravity()); // ������ �ʿ��� �ӵ� ���

            _mover.SetVelocity(velocity); // �ӵ� ����
            _animator.CrossFadeInFixedTime(animJumpState, 0.0f); // ���� �ִϸ��̼� ���
            _startSpeed = speedOnAir; // ���� �ӵ� ����

            if (_startInput.magnitude > 0.1f)
                _startInput.Normalize(); // �Է� ����ȭ

            if (_audioPlayer)
                _audioPlayer.PlayVoice(jumpEffort); // ���� ȿ���� ���
        }

        private void HardLand()
        {

        }
    }
}