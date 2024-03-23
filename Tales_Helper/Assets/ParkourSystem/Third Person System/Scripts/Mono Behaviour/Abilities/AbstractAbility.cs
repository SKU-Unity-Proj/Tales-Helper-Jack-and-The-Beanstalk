using System;
using UnityEngine;
using static FC.TagAndLayer;

namespace DiasGames.Abilities
{
    public abstract class AbstractAbility : MonoBehaviour
    {
        // �켱������ �����ϴ� ����, ���� ���ڰ� �� ���� �켱������ ����
        [SerializeField] private int abilityPriority = 0;

        // ���� �����Ƽ�� ���� ������ ���θ� ��Ÿ���� �÷���
        public bool IsAbilityRunning { get; private set; }

        // �����Ƽ�� �ߴܵǰų� ���۵� �� �߻��ϴ� �̺�Ʈ
        public event Action<AbstractAbility> abilityStopped = null;
        public event Action<AbstractAbility> abilityStarted = null;

        // �����Ƽ �켱������ ���� ����
        public int AbilityPriority { get { return abilityPriority; } }

        // �ִϸ����� ������Ʈ ����
        protected Animator _animator = null;

        // �����Ƽ ���� �� ���� �ð�
        public float StartTime { get; private set; } = 0;
        public float StopTime { get; private set; } = 0;

        // ĳ���� ������ ���� ����
        protected CharacterActions _action;

        /// <summary>
        /// ĳ���� ���ۿ� ���� ������ �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="newAction">���ο� CharacterActions ����</param>
        public void SetActionReference(ref CharacterActions newAction)
        {
            _action = newAction;
        }

        protected virtual void Start()
        {
            // �ִϸ����� ������Ʈ�� ������
            _animator = GetComponent<Animator>();
        }

        public void StartAbility()
        {
            // �����Ƽ�� �����ϴ� �޼ҵ�
            IsAbilityRunning = true;
            StartTime = Time.time;
            OnStartAbility();
            abilityStarted?.Invoke(this);
        }
        public void StopAbility()
        {
            // �ʹ� ª�� �ð� ���� �����Ƽ�� �ߴ��Ϸ��� �õ��� ����
            if (Time.time - StartTime < 0.1f)
                return;

            // �����Ƽ�� �ߴ��ϴ� �޼ҵ�
            IsAbilityRunning = false;
            StopTime = Time.time;
            OnStopAbility();
            abilityStopped?.Invoke(this);
        }

        // �����Ƽ�� ������ �غ� �Ǿ����� Ȯ���ϴ� �߻� �޼ҵ�
        public abstract bool ReadyToRun();

        // �����Ƽ ���� �� ����Ǵ� �߻� �޼ҵ�
        public abstract void OnStartAbility();

        // �� �����Ӹ��� �����Ƽ�� ������Ʈ�ϴ� �߻� �޼ҵ�
        public abstract void UpdateAbility();

        // �����Ƽ �ߴ� �� ����Ǵ� ���� �޼ҵ�
        public virtual void OnStopAbility() { }

        // �ִϸ��̼� ���¸� �����ϴ� ���� �޼ҵ�
        protected void SetAnimationState(string stateName, float transitionDuration = 0.1f, int StateLayer = 0)
        {
            if (_animator.HasState(StateLayer, Animator.StringToHash(stateName)))
            {
                _animator.CrossFadeInFixedTime(stateName, transitionDuration, StateLayer);

                if (StateLayer == 1)
                    SetLayerPriority(1,1);
            }
                
        }

        public void SetLayerPriority(int StateLayer = 1, int Priority = 1) // �ִϸ������� ���̾� �켱���� ��(����) ����
        {
            _animator.SetLayerWeight(StateLayer, Priority);
        }

        /// <summary>
        /// Ư�� ������ �ִϸ��̼��� �������� Ȯ���ϴ� �޼ҵ�
        /// </summary>
        /// <param name="state">Ȯ���� �ִϸ��̼� ������ �̸�</param>
        /// <returns>�ִϸ��̼��� �������� ����</returns>
        protected bool HasFinishedAnimation(string state)
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // �ִϸ����Ͱ� ��ȯ ���̸� false ��ȯ
            if (_animator.IsInTransition(0)) return false;

            // ������ ���°� ���� �����̰�, ����ȭ�� �ð��� ���� ������ true ��ȯ
            if (stateInfo.IsName(state))
            {
                float normalizeTime = Mathf.Repeat(stateInfo.normalizedTime, 1);
                if (normalizeTime >= 0.95f) return true;
            }

            return false;
        }
    }
}
