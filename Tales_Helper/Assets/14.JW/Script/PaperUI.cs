using UnityEngine;

public class PaperUI : MonoBehaviour
{
    [SerializeField] private GameObject[] paperObjects; // ���� �ٸ� ���� ������Ʈ���� �����ϴ� �迭
    [SerializeField] private GameObject[] paperUIs; // ���� �ٸ� ���� UI���� �����ϴ� �迭
    public LayerMask layerMask; // OverlapSphere�� �÷��̾�� ��ȣ�ۿ��ϴ� �ݶ��̴��� �����ϱ� ���� ���̾� ����ũ

    void Update()
    {
        CheckUI();
    }

    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            
            // �ش� ���̾ ���� �ݶ��̴� �Ǻ�
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, layerMask);
            foreach (Collider col in colliders)
            {
                for (int i = 0; i < paperObjects.Length; i++)
                {

                    // �� ���̿� �˸��� ui�� �������ֱ� ���� ���� ����
                    GameObject paperObject = paperObjects[i];
                    GameObject paperUI = paperUIs[i];

                    if (col.gameObject == paperObject) //�ݶ��̴��� ���� ������Ʈ�� ��ġ�ϴ��� Ȯ�� ����
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

                            // �ùٸ� ���� UI Ȱ��ȭ
                            paperUI.SetActive(true);
                        }
                        break;
                    }
                }
            }
        }
    }
}
