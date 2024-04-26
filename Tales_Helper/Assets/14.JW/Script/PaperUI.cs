using UnityEngine;

public class PaperUI : MonoBehaviour
{
    [SerializeField] private GameObject[] paperObjects; // ���� �ٸ� ���� ������Ʈ���� �����ϴ� �迭
    [SerializeField] private GameObject[] paperUIs; // ���� �ٸ� ���� UI���� �����ϴ� �迭
    public LayerMask layerMask; // OverlapSphere�� �÷��̾�� ��ȣ�ۿ��ϴ� �ݶ��̴��� �����ϱ� ���� ���̾� ����ũ

    void Update()
    {
        CheckUI(); // UI ���¸� Ȯ���ϴ� �Լ� ȣ��
    }

    // UI ���¸� Ȯ���ϴ� �Լ�
    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F)) // F Ű �Է��� �����Ǹ�
        {
            // �ֺ��� �ݰ� 3f ���� �ִ� �ݶ��̴��� �˻�
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, layerMask);
            foreach (Collider col in colliders) // ������ �� �ݶ��̴��� ���� �ݺ�
            {
                for (int i = 0; i < paperObjects.Length; i++) // ���� ������Ʈ �迭�� ���̸�ŭ �ݺ�
                {
                    // ���� ������Ʈ�� ���� UI�� ����
                    GameObject paperObject = paperObjects[i];
                    GameObject paperUI = paperUIs[i];

                    if (col.gameObject == paperObject) // �ݶ��̴��� ���� ���� ������Ʈ�� ��ġ�ϴ��� Ȯ��
                    {
                        // �̹� Ȱ��ȭ�� UI���� Ȯ���ϰ� ��Ȱ��ȭ
                        if (paperUI.activeSelf)
                        {
                            paperUI.SetActive(false);
                        }
                        else
                        {
                            // �ٸ� ��� ���� UI�� ��Ȱ��ȭ
                            foreach (GameObject ui in paperUIs)
                            {
                                ui.SetActive(false);
                            }

                            // ���� ���̿� �ش��ϴ� UI�� Ȱ��ȭ
                            paperUI.SetActive(true);
                        }
                        break; // ���� ������Ʈ�� ã�����Ƿ� �ݺ��� ����
                    }
                }
            }
        }
    }
}
