using UnityEngine;

public class PaperUI : MonoBehaviour
{
    [SerializeField] private GameObject[] paperUIs; // 서로 다른 종이 UI들을 저장하는 배열
    public int uiNumber; // 활성화할 UI를 식별하는 번호
    private bool isUIopen = false; // UI의 열림 상태를 관리하는 불린 값
    public LayerMask layerMask;

    void Update()
    {
        CheckUI();
    }

    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // uiNumber를 기반으로 현재 활성화된 UI의 가시성을 토글
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
                        // 한 번에 하나의 UI만 활성화되도록 다른 모든 UI를 비활성화
                        foreach (GameObject ui in paperUIs)
                        {
                            ui.SetActive(false);
                        }

                        // 올바른 UI 활성화
                        paperUIs[uiNumber].SetActive(true);
                        isUIopen = true;
                        break;
                    }
                }
            }
        }
    }
}
