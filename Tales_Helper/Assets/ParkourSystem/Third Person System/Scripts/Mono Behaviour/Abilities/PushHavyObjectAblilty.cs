using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;
using DiasGames.IK;
using UnityEngine.Events;

namespace DiasGames.Abilities
{
    public class PushHavyObjectAbility : AbstractAbility
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float positionSmoothnessTime = 0.12f;
        [Header("Animation")]
        [SerializeField] private string pushAnimationBlendState = "Push Havy Blend";
        [SerializeField] private string horizontalFloatParam = "Horizontal";
        [SerializeField] private string verticalFloatParam = "Vertical";

        [SerializeField] private float rotationSpeed = 50f; // ȸ�� �ӵ�

        private Transform mirrorTransform; // �̷��� Transform

        private IMover _mover;
        private IKScheduler _ikScheduler;
        private Transform _camera;

        private IHDraggable _draggable;
        private List<Collider> _triggeredObjs = new List<Collider>();

        //sound
        [SerializeField] private List<int> _boxDragSound = new List<int>();
        [SerializeField] private Transform boxsoundPos;

        private bool _isMatchingTarget;
        private float _step;

        private int _animHorizontalID;
        private int _animVerticalID;
        private int _animMotionSpeedID;

        private Vector3 _lastPosition;

        public UnityEvent onPushStart;

        private bool _isDragging;
        private float _soundCooldown = 0.5f; // �Ҹ� ��� ������ ������ �� �ִ� ����
        private float _lastSoundTime;

        private float oldDist, maxDist = 0; // �̵��Ÿ� üũ.
        private float lastStepTime = 0f; // ������ ���ڱ� �Ҹ� �ð�
        private float stepInterval = 0.5f; // �ּ� ���ڱ� �Ҹ� ����
        private int index;

        private void Awake()
        {
            _mover = GetComponent<IMover>();
            _ikScheduler = GetComponent<IKScheduler>();

            _camera = Camera.main.transform;

            // assign animations ids
            _animHorizontalID = Animator.StringToHash(horizontalFloatParam);
            _animVerticalID = Animator.StringToHash(verticalFloatParam);
            _animMotionSpeedID = Animator.StringToHash("Motion Speed");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsAbilityRunning || Time.time - StopTime < 0.1f) return;

            _triggeredObjs.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsAbilityRunning || Time.time - StopTime < 0.1f) return;

            if (_triggeredObjs.Contains(other))
                _triggeredObjs.Remove(other);
        }

        private bool IsDraggable()
        {
            if (_triggeredObjs.Count == 0) return false;

            foreach (var trigger in _triggeredObjs)
            {
                if (trigger.TryGetComponent(out _draggable))
                {
                    // �̷� ������Ʈ�� �θ� Ȯ��
                    if (trigger.transform.parent != null && trigger.transform.parent.CompareTag("Mirro"))
                    {
                        mirrorTransform = trigger.transform.parent; // �̷� Transform ����
                    }
                    return true;
                }
            }

            return false;
        }

        public override void OnStartAbility()
        {
            _draggable.HStartDrag();
            _mover.StopMovement();
            SetAnimationState(pushAnimationBlendState);

            _step = Vector3.Distance(transform.position, _draggable.GetTarget().position) / positionSmoothnessTime;
            _isMatchingTarget = true;

            Debug.Log("name :" + _draggable);
            Debug.Log("name :" + _draggable.ToString());

            if (_draggable.ToString() == "Grab Trigger (DiasGames.Puzzle.HavyTrigger)")
                onPushStart.Invoke();
        }

        public override bool ReadyToRun()
        {
            if (!_mover.IsGrounded() || Time.time - StopTime < 0.1f) return false;

            return _action.interact && IsDraggable();
        }

        public override void UpdateAbility()
        {
            // HandleIK();
            UpdateTransform();

            Debug.Log(mirrorTransform.name);

            if (_isMatchingTarget) return;

            // E Ű�� �̷� ȸ��
            if (Input.GetButton("PickUp") && mirrorTransform != null)
            {
                RotateMirror(true); // �ð� ����
            }
            else if (Input.GetKey(KeyCode.Q) && mirrorTransform != null)
            {
                RotateMirror(false); // �ݽð� ����
            }

            if (_action.interact)
                StopAbility();

            Vector3 targetPos = _draggable.GetTarget().position;
            targetPos.y = transform.position.y;

            _mover.SetPosition(targetPos);
            transform.rotation = _draggable.GetTarget().rotation;

            // calculate input for relative move
            Vector3 cameraFwd = Vector3.Scale(_camera.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 relativeMove = _action.move.x * _camera.transform.right + _action.move.y * cameraFwd;
            relativeMove.Normalize();

            // send move input to drag object
            _draggable.HMove(relativeMove * speed);

            float currentSpeed = (_lastPosition - transform.position).magnitude / Time.deltaTime;
            bool hasMoved = currentSpeed > 0.1f;

            _animator.SetFloat(_animMotionSpeedID, hasMoved ? currentSpeed / speed : 1, 0.1f, Time.deltaTime);

            float hor = Vector3.Dot(transform.right, relativeMove);
            float ver = Vector3.Dot(transform.forward, relativeMove);

            // update animator
            _animator.SetFloat(_animHorizontalID, hor, 0.1f, Time.deltaTime);
            _animator.SetFloat(_animVerticalID, ver, 0.1f, Time.deltaTime);

            // Play sound if player has moved
            if (mirrorTransform == null &&  hasMoved && Time.time - _lastSoundTime > _soundCooldown)
            {
                PlayFootStep();
            }

            _lastPosition = transform.position;
        }

        public override void OnStopAbility()
        {
            _draggable.HStopDrag();

            // reset vars
            _isMatchingTarget = false;

            // stop IK
            if (_ikScheduler != null)
            {
                _ikScheduler.StopIK(AvatarIKGoal.LeftHand);
                _ikScheduler.StopIK(AvatarIKGoal.RightHand);
            }
        }

        private void HandleIK()
        {
            if (_draggable != null && _ikScheduler != null)
            {
                // left hand
                Transform lhEffector = _draggable.GetLeftHandTarget();
                if (lhEffector != null)
                {
                    IKPass leftHandPass = new IKPass(lhEffector.position,
                        lhEffector.rotation,
                        AvatarIKGoal.LeftHand,
                        1, 1);

                    _ikScheduler.ApplyIK(leftHandPass);
                }

                // right hand
                Transform rhEffector = _draggable.GetRightHandTarget();
                if (rhEffector != null)
                {
                    IKPass rightHandPass = new IKPass(rhEffector.position,
                        rhEffector.rotation,
                        AvatarIKGoal.RightHand,
                        1, 1);

                    _ikScheduler.ApplyIK(rightHandPass);
                }
            }
        }

        private void UpdateTransform()
        {
            if (!_isMatchingTarget || _draggable == null) return;

            Vector3 targetPos = _draggable.GetTarget().position;
            targetPos.y = transform.position.y;

            _mover.SetPosition(Vector3.MoveTowards(transform.position, targetPos, _step * Time.deltaTime));
            transform.rotation = Quaternion.Lerp(transform.rotation, _draggable.GetTarget().rotation, positionSmoothnessTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                _isMatchingTarget = false;
                _mover.SetPosition(targetPos);
                transform.rotation = _draggable.GetTarget().rotation;
            }
        }

        public void StopAbilityFunction()
        {
            StopAbility();
        }

        private void InitializeSoundList()
        {
            _boxDragSound.Add((int)SoundList.metalDrag1);
            _boxDragSound.Add((int)SoundList.metalDrag2);
            _boxDragSound.Add((int)SoundList.metalDrag3);
        }

        private void RotateMirror(bool isClockwise)
        {
            if (mirrorTransform == null) return;

            // �̷� ȸ��
            float direction = isClockwise ? 1f : -1f;
            mirrorTransform.Rotate(0, direction * rotationSpeed * Time.deltaTime, 0, Space.World);

            // ĳ���� ȸ���� �ε巴�� ó��
            //RotateCharacterTowardsMirrorSmoothly();
        }

        private void PlayFootStep()
        {
            if (_boxDragSound.Count <= 3)
            {
                InitializeSoundList();
            }

            if (_boxDragSound.Count > 3)
            {
                _boxDragSound.RemoveAt(0); // ù ��° ��� ����
            }


            if (oldDist < maxDist || Time.time - lastStepTime < stepInterval)
            {
                return;
            }
            oldDist = maxDist = 0;
            lastStepTime = Time.time; // ������ ���ڱ� �Ҹ� �ð� ������Ʈ
            int oldIndex = index;
            while (oldIndex == index)
            {
                index = Random.Range(0, _boxDragSound.Count);
            }
            SoundManager.Instance.PlayOneShotEffect((int)_boxDragSound[index], boxsoundPos.position, 2f);
        }
    }
}
