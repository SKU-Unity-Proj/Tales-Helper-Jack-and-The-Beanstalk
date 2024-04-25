using DiasGames.Abilities;
using System.Collections.Generic;
using DiasGames.Components;
using UnityEngine;
using UnityEngine.Events;

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

        public Transform raisePos;
        public UnityEvent onRaiseStart;

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
            return _action.interact && _mover.IsGrounded() && _raiseObjs.Count > 0;
        }

        public override void OnStartAbility()
        {
            if (_raiseObjs.Count > 0)
            {
                _mover.StopMovement();  // 이동 중지

                _startingRaise = true;
                _stoppingRaise = false;

                _raiseCount++;
                if (_raiseCount <= 2)
                {
                    SetAnimationState(RaiseAnimationState);
                }
                else
                {
                    SetAnimationState(RaiseUpAnimationState);
                    _raiseCount = 0;  // 카운트 리셋
                }

                onRaiseStart.Invoke();
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
    }
}
