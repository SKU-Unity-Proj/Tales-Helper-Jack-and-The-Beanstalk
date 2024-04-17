using DiasGames.Abilities;
using System.Collections.Generic;
using DiasGames.Components;
using UnityEngine;

//ItemHugPos Position(0, -0.037, 0.336)

namespace DiasGames.Abilities
{
    public class RaiseAbility : AbstractAbility
    {
        [Header("Animation")]
        [SerializeField] private string RaiseAnimationState = "Raise";

        private List<Collider> _raiseObjs = new List<Collider>();

        private IMover _mover = null;

        public Transform raisePos;

        private bool _startingRaise = false; // 기어가기 시작 중인지를 나타내는 플래그
        private bool _stoppingRaise = false; // 기어가기를 중단 중인지를 나타내는 플래그


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
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (_raiseObjs.Contains(other))
                _raiseObjs.Remove(other);

        }

        public override bool ReadyToRun() // 실행 조건
        {
            return _action.interact && _mover.IsGrounded();
        }

        public override void OnStartAbility() // 실행될 때 호출
        {

            if (_stoppingRaise && _raiseObjs.Count == 1)
            {
                //_mover.StopMovement(); // velocity 0
                _startingRaise = true;
                _stoppingRaise = true;

                SetAnimationState(RaiseAnimationState);
            }
            
        }


        public override void UpdateAbility() // 실행 중에 계속 호출
        {
            // 기어가기 시작 애니메이션을 기다림
            if (_startingRaise)
            {
                if (_animator.IsInTransition(0)) return; // 애니메이션의 0번 레이어가 상태를 전환중이면 return시킴

                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(RaiseAnimationState))
                    _startingRaise = false;

                return;
            }

            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && !_animator.IsInTransition(0))
            {
                StopAbility();
                _stoppingRaise = false;
            }

        }

        public override void OnStopAbility()
        {
            if (_stoppingRaise == true)
            {
                _raiseObjs.RemoveAt(0);
            }
        }

    }
}
