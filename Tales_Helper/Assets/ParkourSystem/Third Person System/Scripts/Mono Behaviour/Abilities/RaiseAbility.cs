using DiasGames.Abilities;
using System.Collections.Generic;
using DiasGames.Components;
using UnityEngine;

namespace DiasGames.Abilities
{
    public class RaiseAbility : AbstractAbility
    {
        [Header("Animation")]
        [SerializeField] private string RaiseAnimationState = "Raise";

        private List<Collider> _raiseObjs = new List<Collider>();

        private IMover _mover = null;

        public Transform raisePos;

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
                // ¸ðµç ÀÌµ¿À» ¸ØÃã
                _mover.StopMovement();

                _startingRaise = true;
                _stoppingRaise = false;
                SetAnimationState(RaiseAnimationState);
            }
        }

        public override void UpdateAbility()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(RaiseAnimationState))
            {
                if (_startingRaise && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && !_animator.IsInTransition(0))
                {
                    _stoppingRaise = true;
                    _startingRaise = false;

                    StopAbility();
                    SetAnimationState("Grounded");
                }
            }
             
        }

        public override void OnStopAbility()
        {
            if (_stoppingRaise)
            {
                //_raiseObjs.RemoveAt(0);
                _stoppingRaise = false;
            }
        }
    }
}
