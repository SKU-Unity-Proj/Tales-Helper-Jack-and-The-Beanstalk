using UnityEngine;
using DiasGames.Components;

namespace DiasGames.Abilities
{
    public class Crawl : AbstractAbility
    {
        [SerializeField] private float crawlSpeed = 2f; // ���� �ӵ�
        [SerializeField] private float capsuleHeightOnCrawl = 0.5f; // �� �� ĸ���� ����

        [Header("Cast Parameters")]
        [SerializeField] private LayerMask obstaclesMask; // ��ֹ��� �����ϴ� �� ���Ǵ� ���̾� ����ũ
        [Tooltip("This is the height that sphere cast can reach to know when should force crawl state")]
        [SerializeField] private float MaxHeightToStartCrawl = 0.75f; // ���� �����ؾ� �ϴ� �ִ� ����

        [Header("Animation States")]
        [SerializeField] private string startCrawlAnimationState = "Stand to Crawl"; // ���� �����ϴ� �ִϸ��̼� ����
        [SerializeField] private string stopCrawlAnimationState = "Crawl to Stand"; // ���⸦ ���ߴ� �ִϸ��̼� ����

        private IMover _mover; // �̵��� �����ϴ� �������̽�
        private ICapsule _capsule; // ĸ�� �ݶ��̴��� �����ϴ� �������̽�

        private bool _startingCrawl = false; // ���� ���� �������� ��Ÿ���� �÷���
        private bool _stoppingCrawl = false; // ���⸦ �ߴ� �������� ��Ÿ���� �÷���

        private float _defaultCapsuleRadius = 0; // �⺻ ĸ�� ������

        private void Awake()
        {
            _mover = GetComponent<IMover>();
            _capsule = GetComponent<ICapsule>();

            _defaultCapsuleRadius = _capsule.GetCapsuleRadius();
        }

        public override bool ReadyToRun()
        {
            // ĳ���Ͱ� �ٴڿ� ���� ������ �� �غ� �� ��
            if (!_mover.IsGrounded()) return false;

            // 'crawl' �׼��� Ȱ��ȭ�ǰų� ���̿� ���� ���Ⱑ �����Ǹ� �غ� �Ϸ�
            if (_action.crawl || ForceCrawlByHeight())
                return true;

            return false;
        }

        public override void OnStartAbility()
        {
            // ��� �̵��� ����
            _mover.StopMovement();

            // �ý��ۿ� ���� ���� ������ �˸�
            _startingCrawl = true;

            // ���� �ִϸ��̼��� ����
            SetAnimationState(startCrawlAnimationState);

            // ĸ�� �ݶ��̴��� ũ�⸦ ����  
            _capsule.SetCapsuleSize(capsuleHeightOnCrawl, _capsule.GetCapsuleRadius());
        }

        public override void UpdateAbility()
        {
            // ���� ���� �ִϸ��̼��� ��ٸ�
            if (_startingCrawl)
            {
                if (_animator.IsInTransition(0)) return; // �ִϸ��̼��� 0�� ���̾ ���¸� ��ȯ���̸� return��Ŵ

                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(startCrawlAnimationState))
                    _startingCrawl = false;

                return;
            }

            // ���⸦ ���ߴ� �ִϸ��̼��� ��ٸ�
            if (_stoppingCrawl)
            {
                if (_animator.IsInTransition(0)) return;

                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f)
                    StopAbility();

                return;
            }

            // ĳ���͸� ���� �̵���Ŵ
            _mover.Move(_action.move, crawlSpeed);

            // 'crawl' �׼��� �ٽ� Ȱ��ȭ�Ǹ� ���⸦ ����
            if (_action.crawl && !ForceCrawlByHeight())
            {
                SetAnimationState(stopCrawlAnimationState);
                _stoppingCrawl = true;
                _mover.StopMovement();
            }

            if (_action.crouch)
            {
                // Crouch ���·� ��ȯ�� ���� �޼ҵ� ȣ��
                StartCrouchFromCrawl();
                return; // �߰� ������ �����ϱ� ���� ����
            }

            if (_action.jump)
            {
                // Crouch ���·� ��ȯ�� ���� �޼ҵ� ȣ��
                StartCrouchFromCrawl();
                return; // �߰� ������ �����ϱ� ���� ����
            }
        }

        public override void OnStopAbility()
        {
            // ���� ������ ����
            _startingCrawl = false;
            _stoppingCrawl = false;

            // ĸ�� ũ�⸦ ������� ����
            _capsule.ResetCapsuleSize();
        }

        private bool ForceCrawlByHeight()
        {
            RaycastHit hit;

            // SphereCast�� ����Ͽ� ��ֹ��� �ִ��� Ȯ��
            if (Physics.SphereCast(transform.position, _defaultCapsuleRadius, Vector3.up, out hit,
                MaxHeightToStartCrawl, obstaclesMask, QueryTriggerInteraction.Ignore))
            {
                // �浹 ������ �� ���� ĸ�� ���̺��� ������ true ��ȯ
                if (hit.point.y - transform.position.y > capsuleHeightOnCrawl)
                    return true;
            }

            return false;
        }
        private void StartCrouchFromCrawl()
        {
            // Crouch ���·��� ��ȯ ���� ����
            SetAnimationState(stopCrawlAnimationState); // ���⸦ ���ߴ� �ִϸ��̼�
            _stoppingCrawl = true; // ���� �ߴ� ���·� ����
            _mover.StopMovement(); // ĳ������ �̵� �ߴ�

            // �ٲ�� ���� �׼��� �����ϴ� �κ��� ���� ��
            // ���� ���, Crouch ���·� ���� ��ȯ�ϰų�,
            // Crouch ���¸� �����ϴ� ������ ��Ŀ������ Ʈ������ �� ����
            // Crawl���� Crouch�� ���� ������ �ִϸ��̼��� ��� �׳� �Ͼ�� ����
        }
    }
}
