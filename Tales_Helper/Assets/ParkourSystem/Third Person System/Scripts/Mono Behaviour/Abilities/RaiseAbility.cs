using DiasGames.Abilities;
using System.Collections.Generic;
using DiasGames.Components;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace DiasGames.Abilities
{
    public class RaiseAbility : AbstractAbility
    {
        [Header("Animation")]
        [SerializeField] private string RaiseAnimationState = "Raise";
        [SerializeField] private string RaiseUpAnimationState = "Raise Up";

        private List<Collider> _raiseObjs = new List<Collider>();

        private IMover _mover = null;
        private int _raiseCount = 0;
        private bool _canRaise = true; // Raise를 실행할 수 있는지 여부를 나타내는 변수

        public Transform raisePos;

        public UnityEvent onRaiseStartSecondTime; // 2번 이하 실행시
        public UnityEvent onRaiseStartThirdTime;  // 그 이상 실행시

        private bool _startingRaise = false;
        private bool _stoppingRaise = false;

        private void Awake()
        {
            _mover = GetComponent<IMover>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("raiseCol"))
            {
                _raiseObjs.Add(other);
                Debug.Log(other.name);
                Debug.Log(_raiseObjs.Count);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_raiseObjs.Contains(other))
            {
                _raiseObjs.Remove(other);
                Debug.Log(_raiseObjs.Count);
            }
        }

        public override bool ReadyToRun()
        {
            return _action.interact && _mover.IsGrounded() && _raiseObjs.Count > 0 && _canRaise;
        }

        public override void OnStartAbility()
        {


            if (_raiseObjs.Count > 0)
            {
                _mover.StopMovement();  // 이동 중지

                this.transform.rotation = Quaternion.LookRotation(Vector3.forward);

                _canRaise = false; // 추가 실행 방지
                _startingRaise = true;
                _stoppingRaise = false;

                _raiseCount++;
                if (_raiseCount <= 2)
                {
                    SetAnimationState(RaiseAnimationState);
                    onRaiseStartSecondTime.Invoke();  // 2번 이하 실행 이벤트
                }
                else
                {
                    SetAnimationState(RaiseUpAnimationState);
                    onRaiseStartThirdTime.Invoke();    // 그 이상 실행 이벤트
                    _raiseCount = 0;  // 카운트 리셋
                }

                StartCoroutine(WaitBeforeNextRaise(6.8f)); // 0.5초 동안 대기
            }
        }

        public override void UpdateAbility()
        {
            if ((_animator.GetCurrentAnimatorStateInfo(0).IsName(RaiseAnimationState) ||
                 _animator.GetCurrentAnimatorStateInfo(0).IsName(RaiseUpAnimationState)) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f &&
                !_animator.IsInTransition(0))
            {
                _stoppingRaise = true;
                _startingRaise = false;

                StopAbility();  // 어빌리티 종료

            }
        }

        public override void OnStopAbility()
        {
            if (_stoppingRaise)
            {
                _stoppingRaise = false;
            }
        }

        private IEnumerator WaitBeforeNextRaise(float delay)
        {
            yield return new WaitForSeconds(delay);
            _canRaise = true; // 다시 실행 가능
        }
    }
}
