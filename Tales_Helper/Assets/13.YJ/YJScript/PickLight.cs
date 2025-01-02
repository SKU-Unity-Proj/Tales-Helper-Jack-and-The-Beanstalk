using UnityEngine;

public class PickLight : MonoBehaviour
{
    public GameObject pickItem = null; // �ֿ� ������ ���
    public GameObject pickItem_light = null; // �ֿ� ������ ����Ʈ ���
    public Transform targetPos; // ������ �Ȱ� ���� ��ġ
    public float radius = 1f; // �������� �ִ��� üũ�ϴ� ���� ũ��

    public GameObject zombieStopLight;

    public bool isPicking = false; // ������ ����
    private Vector3 originalPosition; // ������ ���� ��ġ
    private Quaternion originalRotation;

    void Start()
    {
        if (zombieStopLight == null)
        {
            zombieStopLight = GameObject.Find("ZombieStopLight"); //ĳ���� �� �ȿ� ����
        }
        zombieStopLight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem(); //������ ã��
        }

    }

    void CheckItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, radius, 1 << 12);

        if (colliders != null)
        {
            foreach (Collider collider in colliders)
            {
                //��ġ ���
                SavePositionAndRotation();
                isPicking = true;

                //������ ���
                pickItem = collider.gameObject;
                pickItem_light = collider.transform.GetChild(1).gameObject;

                // �θ�� ������
                pickItem.transform.SetParent(targetPos);
                pickItem_light.SetActive(false);

                // ����Ʈ �ѱ�
                zombieStopLight.SetActive(true);

                //���������� ȸ��
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

    void SavePositionAndRotation()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }



    /*
    private Animator anim;

    public GameObject pickItem = null; // �ֿ� ������ ���
    public GameObject pickItem_light = null; // �ֿ� ������ ����Ʈ ���
    public Transform targetPos; // ������ �Ȱ� ���� ��ġ
    public float radius = 1f; // �������� �ִ��� üũ�ϴ� ���� ũ��

    public GameObject zombieStopLight;

    public bool isPicking = false; // ������ ����
    private Vector3 originalPosition; // ������ ���� ��ġ
    private Quaternion originalRotation;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (zombieStopLight == null)
        {
            zombieStopLight = GameObject.Find("ZombieStopLight"); //ĳ���� �� �ȿ� ����
        }
        zombieStopLight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem(); //������ ã��
        }

        //Ÿ���� �ڽ����� ������
        TargetSetParent();

        //������ ����
        if (isPicking)
            StopMovement();
    }

    void CheckItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, radius, 1 << 12);

        if (colliders != null)
        {
            foreach (Collider collider in colliders)
            {
                //��ġ ���
                SavePositionAndRotation();
                isPicking = true;

                //������ ���
                pickItem = collider.gameObject;
                pickItem_light = collider.transform.GetChild(1).gameObject;

                //�ִϸ��̼� ����
                anim.SetBool("PickItem", true);

                //���������� ȸ��
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
            // �θ�� ������
            pickItem.transform.SetParent(targetPos);
            pickItem_light.SetActive(false);

            // ����Ʈ �ѱ�
            zombieStopLight.SetActive(true);

            anim.SetBool("PickItem", false);

            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.53f)
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
    }*/
}
