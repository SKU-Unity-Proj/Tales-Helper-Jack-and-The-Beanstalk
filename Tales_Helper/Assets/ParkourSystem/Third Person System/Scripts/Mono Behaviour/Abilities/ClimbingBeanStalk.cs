using DiasGames.Abilities;
using DiasGames.Components;
using UnityEngine;

public class ClimbingBeanStalk : AbstractAbility
{
    private IMover _mover = null;

    public bool isClimb;

    public LayerMask layerMask;
    public float raycastDistance = 0.5f;

    public float climbSpeed = 1.2f; // ��ٸ��� Ÿ�� �ӵ�

    private float timeSinceLeftWall = 0f;
    private const float timeToDisableAbility = 0.9f;

    private bool canRaycast = true;

    private void Awake()
    {
        _mover = GetComponent<IMover>();
    }

    public override bool ReadyToRun()
    {
        return !_mover.IsGrounded() && isClimb;
    }

    public override void OnStartAbility()
    {
        _animator.CrossFadeInFixedTime("ClimbBeanStalk", 0.1f); // ��ٸ� Ÿ�� �ִϸ��̼� ���
        _mover.DisableGravity();
        _mover.SetVelocity(Vector3.zero);
    }

    public override void UpdateAbility()
    {
        // ���� �Է�
        float vertical = _action.move.y;
        float horizontal = _action.move.x;

        // ��ٸ� Ÿ�� �ִϸ��̼ǿ� ���� �Է� ����
        _animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        _animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);

        // �÷��̾ �������� ���� �̵�
        Vector3 moveDirection = transform.up * vertical;
        Vector3 newPosition = transform.position + moveDirection * climbSpeed * Time.deltaTime;
        transform.position = newPosition;

        // �÷��̾ �������� �¿� �̵�
        moveDirection = transform.right * horizontal;
        newPosition = transform.position + moveDirection * climbSpeed * Time.deltaTime;
        transform.position = newPosition;

        // �Ʒ��� �̵� ���̰� ���� ������ ��� �ɷ� ����
        if (_action.move.y < 0f && _mover.IsGrounded())
            StopAbility();

        // ��ٸ����� ��������
        if (_action.jump)
        {
            StopAbility();
        }
    }


    public override void OnStopAbility()
    {
        isClimb = false;
        _mover.EnableGravity(); // �߷� Ȱ��ȭ
        _mover.StopRootMotion(); // ��Ʈ ��� ����

        canRaycast = false;
        Invoke("EnableRaycast", 1f);
    }

    private void EnableRaycast()
    {
        canRaycast = true;
    }

    private void Update()
    {
        if (!canRaycast)
            return;

        RaycastHit hit;

        // ����ĳ��Ʈ�� �����Ͽ� ���� ������ Ȯ��
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, layerMask))
        {
            // ���� ���� ���
            isClimb = true;

            // ĳ������ ���� ���͸� ���̰� ������ �ݶ��̴��� �������� ����
            transform.forward = -hit.normal;

            // ���� ���� ��� �ð� �ʱ�ȭ
            timeSinceLeftWall = 0f;
        }
        else
        {
            // ���� ���� ���� ���
            isClimb = false;

            // ������ ��� �� �ð� ����
            timeSinceLeftWall += Time.deltaTime;

            // ���� �ð� ���� ���� ���� ������ �����Ƽ ����
            if (timeSinceLeftWall >= timeToDisableAbility)
                StopAbility();
        }
    }

}
