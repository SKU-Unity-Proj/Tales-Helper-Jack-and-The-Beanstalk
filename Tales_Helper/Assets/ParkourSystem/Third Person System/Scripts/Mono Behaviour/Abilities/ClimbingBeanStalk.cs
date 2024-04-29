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

    public float climbSpeed = 1.2f; // 사다리를 타는 속도

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
        _animator.CrossFadeInFixedTime("ClimbBeanStalk", 0.1f); // 사다리 타는 애니메이션 재생
        _mover.DisableGravity();
        _mover.SetVelocity(Vector3.zero);
    }

    public override void UpdateAbility()
    {
        // 수직 입력
        float vertical = _action.move.y;
        float horizontal = _action.move.x;

        // 사다리 타는 애니메이션에 수직 입력 설정
        _animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        _animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);

        // 플레이어를 기준으로 상하 이동
        Vector3 moveDirection = transform.up * vertical;
        Vector3 newPosition = transform.position + moveDirection * climbSpeed * Time.deltaTime;
        transform.position = newPosition;

        // 플레이어를 기준으로 좌우 이동
        moveDirection = transform.right * horizontal;
        newPosition = transform.position + moveDirection * climbSpeed * Time.deltaTime;
        transform.position = newPosition;

        // 아래로 이동 중이고 땅에 도달한 경우 능력 종료
        if (vertical < 0f && _mover.IsGrounded())
            StopAbility();

        // 사다리에서 떨어지기
        if (_action.drop)
        {
            StopAbility();
        }
    }


    public override void OnStopAbility()
    {
        _mover.EnableGravity(); // 중력 활성화
        _mover.StopRootMotion(); // 루트 모션 중지
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position + transform.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            isClimb = true;

            // 캐릭터의 전방 벡터를 레이가 검출한 콜라이더의 방향으로 설정
            transform.forward = -hit.normal;
        }
        
        if(hit.collider == null)
        {
            isClimb = false;
        }
    }
}
