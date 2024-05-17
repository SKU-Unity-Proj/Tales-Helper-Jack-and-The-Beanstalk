using DiasGames.Abilities;
using System.Collections.Generic;
using DiasGames.Components;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Playables;
using Cinemachine;

namespace DiasGames.Abilities
{
    public class RaiseAbility : AbstractAbility
    {
        [Header("Animation")]
        [SerializeField] private string RaiseAnimationState = "Raise";
        [SerializeField] private string RaiseUpAnimationState = "Raise Up";

        [SerializeField] private GameObject originGiant = null;  // ��� Ʈ������
        [SerializeField] private GameObject setGiant = null;  // ��� Ʈ������

        private List<Collider> _raiseObjs = new List<Collider>();

        private IMover _mover = null;

        private int _raiseCount = 0;
        private bool _canRaise = true; // Raise�� ������ �� �ִ��� ���θ� ��Ÿ���� ����

        public UnityEvent onRaiseStartSecondTime; // 2�� ���� �����
        public UnityEvent onRaiseStartThirdTime;  // �� �̻� �����

        private bool _startingRaise = false;
        private bool _stoppingRaise = false;

        private float _tapCountdown = 1f;  // ��Ÿ ���� �ð�
        private bool _shouldContinue = false;  // ��Ÿ ������ ���� ����
        private int _interactCount = 0;  // interact �Է� ī����

        //Ÿ�Ӷ���
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private CinemachineVirtualCamera timelineCam;

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
                _mover.StopMovement();  // �̵� ����

                this.transform.rotation = Quaternion.LookRotation(Vector3.forward);

                _canRaise = false; // �߰� ���� ����
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
                }

                StartCoroutine(WaitBeforeStoppingRaise(_tapCountdown));
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

                StopAbility();  // �����Ƽ ����

            }

            if (_shouldContinue && _action.interact)
            {
                _shouldContinue = false;  // ���� ��Ÿ ���
                StartCoroutine(WaitBeforeNextRaise(6.8f));
            }
            else if (!_action.interact)
            {
                _shouldContinue = true;  // ��Ÿ ��� ����
            }

            if (_animator.speed == 0 && _action.interact)
            {
                if (_interactCount < 4)  // ù ��°���� �� ��° Ŭ�������� ī��Ʈ�� ����
                {
                    _interactCount++;

                    float frameIncrement = 0.01f; // ���ϸ��̼��� normalizedTime�� ������ų �� (2~3������ ���� ����)
                    float newTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime + frameIncrement;
                    _animator.Play(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, newTime);

                }
                else if (_interactCount == 4 && _raiseCount <= 2)  // �ټ� ��° �Է¿���
                {
                    _animator.speed = 1;  // ���ϸ��̼� �簳
                    onRaiseStartSecondTime.Invoke();  // 2�� ���� ���� �̺�Ʈ

                    _interactCount = 0;   // ī��Ʈ �ʱ�ȭ

                   
                }
                else 
                {
                    _animator.speed = 1;  // ���ϸ��̼� �簳
                    onRaiseStartThirdTime.Invoke();    // �� �̻� ���� �̺�Ʈ

                    originGiant.SetActive(false);
                    setGiant.SetActive(true);

                    timelineCam.Priority = 12;
                    playableDirector.Play();

                    _raiseCount = 0;  // ī��Ʈ ����
                    _interactCount = 0;   // ī��Ʈ �ʱ�ȭ

                }
            }

        }

        public override void OnStopAbility()
        {
            if (_stoppingRaise)
            {
                _stoppingRaise = false;
            }
        }

        private IEnumerator WaitBeforeStoppingRaise(float delay)
        {
            yield return new WaitForSeconds(delay);
            _animator.speed = 0;  // ���ϸ����� �ӵ��� 0���� �����Ͽ� ���ϸ��̼� �Ͻ� ����
            _interactCount = 0;   // �Է� ī��Ʈ�� �ʱ�ȭ
        }


        private IEnumerator WaitBeforeNextRaise(float delay)
        {
            yield return new WaitForSeconds(delay);
            _canRaise = true;  // �ٽ� ���� ����
        }
    }
}
