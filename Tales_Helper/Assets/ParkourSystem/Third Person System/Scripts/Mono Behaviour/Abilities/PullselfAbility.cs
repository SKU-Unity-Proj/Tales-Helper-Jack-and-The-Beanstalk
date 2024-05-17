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
    public class PullselfAbility : AbstractAbility
    {
        [Header("Animation")]
        [SerializeField] private string PullselfAnimationState = "Pullself";

        private List<Collider> _pullObjs = new List<Collider>();

        private IMover _mover = null;

        private bool _canPull = true; // Raise�� ������ �� �ִ��� ���θ� ��Ÿ���� ����

        public Transform pullPos;

        public GameObject giantCol;

        public UnityEvent onFallDownEvent; // 2�� ���� �����

        private bool _startingPull = false;
        private bool _stoppingPull = false;

        private float _tapCountdown = 0.5f;  // ��Ÿ ���� �ð�
        private bool _shouldContinue = false;  // ��Ÿ ������ ���� ����
        private int _interactCount = 0;  // interact �Է� ī����


        private void Awake()
        {
            _mover = GetComponent<IMover>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("pullCol"))
            {
                _pullObjs.Add(other);
                Debug.Log(other.name);
                Debug.Log(_pullObjs.Count);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_pullObjs.Contains(other))
            {
                _pullObjs.Remove(other);
                Debug.Log(_pullObjs.Count);
            }
        }

        public override bool ReadyToRun()
        {
            return _action.interact && _mover.IsGrounded() && _pullObjs.Count > 0 && _canPull;
        }

        public override void OnStartAbility()
        {
            if (_pullObjs.Count > 0)
            {
                this.transform.position = pullPos.position;
                this.transform.rotation = Quaternion.LookRotation(-Vector3.right);

                _mover.StopMovement();  // �̵� ����

                _canPull = false; // �߰� ���� ����

                _startingPull = true;
                _stoppingPull = false;

                SetAnimationState(PullselfAnimationState);

                StartCoroutine(WaitBeforeStoppingRaise(_tapCountdown));
            }
        }

        public override void UpdateAbility()
        {
            if ((_animator.GetCurrentAnimatorStateInfo(0).IsName(PullselfAnimationState) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f &&
                !_animator.IsInTransition(0)))
            {
                _stoppingPull = true;
                _startingPull = false;

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

                    if(_interactCount == 2)
                    {
                        onFallDownEvent.Invoke();
                        giantCol.SetActive(true);
                    }
                        
                }

                else
                {
                    _animator.speed = 1;  // ���ϸ��̼� �簳

                    _interactCount = 0;   // ī��Ʈ �ʱ�ȭ

                }
            }

        }

        public override void OnStopAbility()
        {
            if (_stoppingPull)
            {
                _stoppingPull = false;
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
            _canPull = true;  // �ٽ� ���� ����
        }
    }
}
