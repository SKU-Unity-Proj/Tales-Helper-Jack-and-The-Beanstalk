using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;
using DiasGames.IK;
using System.Collections;

namespace DiasGames.Abilities
{
    public class PushLeverAbility : AbstractAbility
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float positionSmoothnessTime = 0.12f;
        [Header("Animation")]
        [SerializeField] private string pushLeverAnimationState = "Push Lever";

        [SerializeField] private Transform leverHandle;
        [SerializeField] private Transform rightIK = null;
        [SerializeField] private Transform leftIK = null;
        [SerializeField] private Transform rightIKTarget = null; // 애니메이션 대상의 오른쪽 타겟
        [SerializeField] private Transform leftIKTarget = null;  // 애니메이션 대상의 왼쪽 타겟
        [SerializeField] private Transform headTransform = null;  // 헤드 트렌스폼

        // components
        private IMover _mover;
        private IKScheduler _ikScheduler;
        private Animator animator;

        // interaction components
        private IPushable _pushable;
        private List<Collider> _triggeredObjs = new List<Collider>();

        // private internal vars
        // to positioning
        private bool _isMatchingTarget;

        private void Awake()
        {
            _mover = GetComponent<IMover>();
            _ikScheduler = GetComponent<IKScheduler>();
            animator = GetComponent<Animator>();

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

            _isMatchingTarget = true;

            StartCoroutine(CheckAnimationProgress(pushLeverAnimationState, 1f));
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

            StartCoroutine(LeverHandleDelay(0.1f)); // 0.1초 지연

        }

        private IEnumerator CheckAnimationProgress(string stateName, float progressThreshold)
        {
            // 애니메이터의 현재 상태가 목표 상태와 일치하는지 확인합니다.
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                yield return null; // 다음 프레임까지 기다립니다.
            }

            // 현재 상태의 진행도를 체크합니다.
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.normalizedTime < progressThreshold)
            {
                yield return null; // 다음 프레임까지 기다립니다.
                                   // 상태가 변경될 수 있으므로, 매 루프마다 상태 정보를 업데이트합니다.
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            // 애니메이션이 5/7 정도 실행된 시점에서 실행할 로직을 여기에 추가합니다.
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

        // 지연 후에 레버 핸들을 IK 타겟에 연결하는 Coroutine입니다.
        private IEnumerator LeverHandleDelay(float delay)
        {
            // 지연 시간만큼 대기합니다.
            yield return new WaitForSeconds(delay);

            // 지연 후에 실행할 코드
            rightIK.position = rightIKTarget.position;
            leftIK.position = leftIKTarget.position;

            // 레버 핸들의 위치를 중간점으로 설정합니다.
            leverHandle.position = CalculateMidpointOfHands();
        }

        // 플레이어의 양손 IK 타겟 위치의 중간점 계산
        private Vector3 CalculateMidpointOfHands()
        {
            Vector3 midpoint = (leftIK.position + rightIK.position) / 2;
            return midpoint;
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