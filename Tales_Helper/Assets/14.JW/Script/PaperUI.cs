using UnityEngine;

public class PaperUI : MonoBehaviour
{
    [SerializeField] private GameObject[] paperUIs; // ���� �ٸ� ���� UI���� �����ϴ� �迭
    public int uiNumber; // Ȱ��ȭ�� UI�� �ĺ��ϴ� ��ȣ
    private bool isUIopen = false; // UI�� ���� ���¸� �����ϴ� �Ҹ� ��
    public LayerMask layerMask;

    void Update()
    {
        CheckUI();
    }

    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // uiNumber�� ������� ���� Ȱ��ȭ�� UI�� ���ü��� ���
            if (paperUIs[uiNumber].activeSelf)
            {
                paperUIs[uiNumber].SetActive(false);
                isUIopen = false;
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f, layerMask);
                foreach (Collider col in colliders)
                {
                    if (!isUIopen)
                    {
                        // �� ���� �ϳ��� UI�� Ȱ��ȭ�ǵ��� �ٸ� ��� UI�� ��Ȱ��ȭ
                        foreach (GameObject ui in paperUIs)
                        {
                            ui.SetActive(false);
                        }

                        // �ùٸ� UI Ȱ��ȭ
                        paperUIs[uiNumber].SetActive(true);
                        isUIopen = true;
                        break;
                    }
                }
            }
        }
    }
}
