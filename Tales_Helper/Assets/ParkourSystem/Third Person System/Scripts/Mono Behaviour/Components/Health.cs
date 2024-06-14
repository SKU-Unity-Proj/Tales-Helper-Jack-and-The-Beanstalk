using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DiasGames.Components
{
    // 'Health' ������Ʈ ����: ĳ������ �ǰ� ���� ����
    public class Health : MonoBehaviour, IDamage
    {
        // ĳ������ �ִ� ü�� ����Ʈ
        [SerializeField] private int MaxHealthPoints = 100;

        // ĳ���� ��� �� �߻��� �̺�Ʈ
        [Space]
        [SerializeField] private UnityEvent OnCharacterDeath;

        // ���� ����
        private int _currentHP = 100; // ���� ü��
        private float _lastDamageTime; // ���������� �������� ���� �ð�
        private Coroutine _restoreRoutine; // ü�� ȸ�� �ڷ�ƾ
        private const float RestoreDelay = 3f; // ü�� ȸ�� ���� �ð� (3��)
        private const int RestoreAmount = 10; // �� ���� ȸ���� ü�� ��

        // ���� ü�¿� ���� ���� ������
        public int CurrentHP { get { return _currentHP; } }

        // �ִ� ü�¿� ���� ���� ������
        public int MaxHP { get { return MaxHealthPoints; } }

        // ü�� ���� �� ��� �̺�Ʈ
        public event Action OnHealthChanged;
        public event Action OnDead;

        // ���� �� ȣ���. ���� ü���� �ִ� ü������ �ʱ�ȭ
        private void Start()
        {
            _currentHP = MaxHealthPoints;
            OnHealthChanged?.Invoke();
        }

        // ������ ó�� �޼ҵ�
        public void Damage(int damagePoints)
        {
            // ���� ü�� ����
            _currentHP -= damagePoints;
            // ������ ������ �ð� ������Ʈ
            _lastDamageTime = Time.time;

            // ������ ü�� ȸ�� �ڷ�ƾ �ߴ� �� �����
            if (_restoreRoutine != null)
                StopCoroutine(_restoreRoutine);
            _restoreRoutine = StartCoroutine(RestoreHealthOverTime());

            // ü���� 0 ���ϰ� �Ǹ� ��� ó��
            if (_currentHP <= 0)
            {
                _currentHP = 0;
                OnDead?.Invoke();
                OnCharacterDeath.Invoke();
            }

            // ü�� ���� �̺�Ʈ ȣ��
            OnHealthChanged?.Invoke();
        }

        // ü���� ���������� ȸ���ϴ� �ڷ�ƾ
        private IEnumerator RestoreHealthOverTime()
        {
            // ������ ���� �ð�(3��) �Ŀ� ü�� ȸ�� ����
            yield return new WaitForSeconds(RestoreDelay);

            // ü���� �ִ�ġ�� �����ϰų� �ֱ� �������� ���� �� 3�ʰ� ���� ������ ü�� ȸ��
            while (_currentHP < MaxHealthPoints && Time.time - _lastDamageTime >= RestoreDelay)
            {
                RestoreHealth(RestoreAmount);
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// ������ ���� ü���� ȸ���ϴ� �޼ҵ�
        /// </summary>
        /// <param name="hp">ȸ���� ü�� ����Ʈ</param>
        public void RestoreHealth(int hp)
        {
            // ü�� ȸ��
            _currentHP += hp;
            // �ִ� ü���� �ʰ����� �ʵ��� ����
            if (_currentHP > MaxHealthPoints)
                _currentHP = MaxHealthPoints;

            // ü�� ���� �̺�Ʈ ȣ��
            OnHealthChanged?.Invoke();
        }

        /// <summary>
        /// ĳ������ ü���� ������ ȸ���ϴ� �޼ҵ� & �� ���� �װ��ϴ� �޼ҵ� - ���Ŀ� �ʿ��ϸ� ������ ����� ��
        /// </summary>
        public void RestoreFullHealth()
        {
            // ü���� �ִ�ġ�� ȸ��
            _currentHP = MaxHealthPoints;

            // ü�� ���� �̺�Ʈ ȣ��
            OnHealthChanged?.Invoke();
        }

        public void InflictFullDamage()
        {
            // ü���� �ִ�ġ�� ȸ��
            _currentHP = 0;
            OnDead?.Invoke();
            OnCharacterDeath.Invoke();
            // ü�� ���� �̺�Ʈ ȣ��
            OnHealthChanged?.Invoke();
        }
    }
}
