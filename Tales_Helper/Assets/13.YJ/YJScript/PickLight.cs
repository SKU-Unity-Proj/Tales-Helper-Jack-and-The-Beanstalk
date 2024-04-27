using UnityEngine;

public class PickLight : MonoBehaviour
{
    private Animator anim;

    public GameObject pickItem = null; // 주운 아이템 등록
    public GameObject pickItem_light = null; // 주운 아이템 라이트 등록
    public Transform targetPos; // 아이템 안고 있을 위치
    public float radius = 1f; // 아이템이 있는지 체크하는 원의 크기

    public GameObject zombieStopLight;

    public bool isPicking = false; // 움직임 제어
    private Vector3 originalPosition; // 움직임 제어 위치
    private Quaternion originalRotation;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (zombieStopLight == null)
        {
            zombieStopLight = GameObject.Find("ZombieStopLight"); //캐릭터 모델 안에 있음
        }
        zombieStopLight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem(); //아이템 찾기
        }

        //타겟을 자식으로 보내기
        TargetSetParent();

        //움직임 제어
        if(isPicking)
            StopMovement();
    }

    void CheckItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, radius, 1 << 9);

        if (colliders != null)
        {
            foreach (Collider collider in colliders)
            {
                //위치 등록
                SavePositionAndRotation();
                isPicking = true;

                //아이템 등록
                pickItem = collider.gameObject;
                pickItem_light = collider.transform.GetChild(1).gameObject;

                //애니메이션 실행
                anim.CrossFadeInFixedTime("PickItem", 0.2f);

                //아이템으로 회전
                Vector3 vec = pickItem.gameObject.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(vec);
                transform.rotation = targetRotation;
            }
            
        }
        else
        {
            pickItem = null;
        }
    }

    private void TargetSetParent()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PickItem") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.38f)
        {
            // 부모로 보내기
            pickItem.transform.SetParent(targetPos);
            pickItem_light.SetActive(false);

            // 라이트 켜기
            zombieStopLight.SetActive(true);

            //위치 맞추기
            /*
            PickItemRotation properties = pickItem.GetComponent<PickItemRotation>();
            if (properties != null)
            {
                pickItem.transform.localPosition = properties.position;
                pickItem.transform.localRotation = Quaternion.Euler(properties.rotation);
            }
            */

            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.53f)
                isPicking = false;
        }
    }

    void SavePositionAndRotation()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void StopMovement()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
