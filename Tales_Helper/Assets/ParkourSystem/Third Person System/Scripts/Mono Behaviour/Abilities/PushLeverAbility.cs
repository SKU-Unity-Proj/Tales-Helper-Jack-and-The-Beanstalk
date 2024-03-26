using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;
using DiasGames.IK;

namespace DiasGames.Abilities
{
    public class PushLeverAbility : AbstractAbility
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float positionSmoothnessTime = 0.12f;
        [Header("Animation")]
        [SerializeField] private string pushLeverAnimationState = "Push Lever";


        // components
        private IMover _mover;
        private IKScheduler _ikScheduler;
        private Transform _camera;

        // interaction components
        private IPushable _pushable;
        private List<Collider> _triggeredObjs = new List<Collider>();

        // private internal vars
        // to positioning
        private bool _isMatchingTarget;
        private float _step;

        private Vector3 _lastPosition;

        private void Awake()
        {
            _mover = GetComponent<IMover>();
            _ikScheduler = GetComponent<IKScheduler>();

            _camera = Camera.main.transform;
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
                if (trigger.TryGetComponent(out _pushable))
                    return true;
            }

            return false;
        }

        public override void OnStartAbility()
        {
            _pushable.StartPush();
            _mover.StopMovement();
            SetAnimationState(pushLeverAnimationState);

            _step = Vector3.Distance(transform.position, _pushable.GetTarget().position) / positionSmoothnessTime;
            _isMatchingTarget = true;
        }

        public override bool ReadyToRun()
        {
            if (Time.time - StopTime < 0.1f) return false;

            return _action.interact && IsDraggable();
        }

        public override void UpdateAbility()
        {
            HandleIK();
            UpdateTransform();

            if (_isMatchingTarget) return;

            if (_action.interact)
                StopAbility();

            Vector3 targetPos = _pushable.GetTarget().position;
            targetPos.y = transform.position.y;

            //_mover.SetPosition(targetPos);
            transform.rotation = _pushable.GetTarget().rotation;

            // calculate input for realtive move
            Vector3 cameraFwd = Vector3.Scale(_camera.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 relativeMove = _action.move.x * _camera.transform.right + _action.move.y * cameraFwd;
            relativeMove.Normalize();

        }

        public override void OnStopAbility()
        {
            _pushable.StopPush();

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
            if (_pushable != null && _ikScheduler != null)
            {
                // left hand
                Transform lhEffector = _pushable.GetLeftHandTarget();
                if (lhEffector != null)
                {
                    IKPass leftHandPass = new IKPass(lhEffector.position,
                        lhEffector.rotation,
                        AvatarIKGoal.LeftHand,
                        1, 1);

                    _ikScheduler.ApplyIK(leftHandPass);
                }

                // right hand
                Transform rhEffector = _pushable.GetRightHandTarget();
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
            if (!_isMatchingTarget || _pushable == null) return;

            Vector3 targetPos = _pushable.GetTarget().position;
            targetPos.y = transform.position.y;

           // _mover.SetPosition(Vector3.MoveTowards(transform.position, targetPos, _step * Time.deltaTime));
            transform.rotation = Quaternion.Lerp(transform.rotation, _pushable.GetTarget().rotation, positionSmoothnessTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                _isMatchingTarget = false;
                //_mover.SetPosition(targetPos);
                transform.rotation = _pushable.GetTarget().rotation;
            }
        }
    }
}