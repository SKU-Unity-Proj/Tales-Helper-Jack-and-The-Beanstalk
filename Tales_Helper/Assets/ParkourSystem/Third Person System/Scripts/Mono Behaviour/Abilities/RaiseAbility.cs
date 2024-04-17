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

        private bool _startingRaise = false; // ���� ���� �������� ��Ÿ���� �÷���
        private bool _stoppingRaise = false; // ���⸦ �ߴ� �������� ��Ÿ���� �÷���


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

        public override bool ReadyToRun() // ���� ����
        {
            return _action.interact && _mover.IsGrounded();
        }

        public override void OnStartAbility() // ����� �� ȣ��
        {

            if (_stoppingRaise && _raiseObjs.Count == 1)
            {
                //_mover.StopMovement(); // velocity 0
                _startingRaise = true;
                _stoppingRaise = true;

                SetAnimationState(RaiseAnimationState);
            }
            
        }


        public override void UpdateAbility() // ���� �߿� ��� ȣ��
        {
            // ���� ���� �ִϸ��̼��� ��ٸ�
            if (_startingRaise)
            {
                if (_animator.IsInTransition(0)) return; // �ִϸ��̼��� 0�� ���̾ ���¸� ��ȯ���̸� return��Ŵ

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
