using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] private GameObject NoticeUI; // �˸� UI�� ����Ű�� ����
    private bool isUIopen = true; // UI�� ���� �����ִ��� ���θ� �����ϴ� ����
    public LayerMask layerMask; // �浹�� ������ ���̾ �����ϴ� ����

    void Update()
    {
        CheckUI(); // UI ���¸� Ȯ���ϴ� �Լ� ȣ��
    }

    // UI ���¸� Ȯ���ϴ� �Լ�
    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F)) // F Ű �Է��� �����Ǹ�
        {
            if (NoticeUI.activeSelf) // UI�� Ȱ��ȭ�Ǿ� �ִٸ�
            {
                NoticeUI.SetActive(false); // UI�� ��Ȱ��ȭ�ϰ�
                isUIopen = false; // UI ���¸� ���� ���·� ����
            }

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f, layerMask); // �ֺ��� �ݰ� 3f ���� �ִ� ��ü���� �浹�� �˻�

            foreach (Collider col in colliders) // ������ �� �浹�� ���� �ݺ�
            {
                if (!NoticeUI.activeSelf && isUIopen) // UI�� ��Ȱ��ȭ�Ǿ� �ְ� ������ UI�� �����ִ� ���¿��ٸ�
                {
                    NoticeUI.SetActive(true); // UI�� Ȱ��ȭ�ϰ�
                    break; // �ݺ��� ����
                }
                isUIopen = true; // UI ���¸� ���� ���·� ����
            }

        }
    }
}
