using DiasGames.Abilities;
using DiasGames.Components;
using UnityEngine;

public class ClimbingBeanStalk : AbstractAbility
{
    private IMover _mover = null;
    private StopRaycast stopRaycast;

    public bool isClimb;

    public LayerMask layerMask;
    public float raycastDistance = 0.5f;
    public float offsetDistance = 0.1f;

    public float climbSpeed = 1.2f; // ��ٸ��� Ÿ�� �ӵ�

    private void Awake()
    {
        _mover = GetComponent<IMover>();
        stopRaycast = GetComponent<StopRaycast>();
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
        if (vertical < 0f && _mover.IsGrounded())
            StopAbility();

        // ��ٸ����� ��������
        if (_action.drop)
        {
            StopAbility();
        }
    }


    public override void OnStopAbility()
    {
        _mover.EnableGravity(); // �߷� Ȱ��ȭ
        _mover.StopRootMotion(); // ��Ʈ ��� ����
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position + transform.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            isClimb = true;

            // ĳ������ ���� ���͸� ���̰� ������ �ݶ��̴��� �������� ����
            transform.forward = -hit.normal;
        }
        
        if(hit.collider == null)
        {
            isClimb = false;
        }
    }
}
