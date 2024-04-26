using UnityEngine;

public class PaperUI : MonoBehaviour
{
    [SerializeField] private GameObject[] paperObjects; // 서로 다른 종이 오브젝트들을 저장하는 배열
    [SerializeField] private GameObject[] paperUIs; // 서로 다른 종이 UI들을 저장하는 배열
    public LayerMask layerMask; // OverlapSphere로 플레이어와 상호작용하는 콜라이더를 제한하기 위한 레이어 마스크

    void Update()
    {
        CheckUI(); // UI 상태를 확인하는 함수 호출
    }

    // UI 상태를 확인하는 함수
    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F)) // F 키 입력이 감지되면
        {
            // 주변에 반경 3f 내에 있는 콜라이더를 검사
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, layerMask);
            foreach (Collider col in colliders) // 감지된 각 콜라이더에 대해 반복
            {
                for (int i = 0; i < paperObjects.Length; i++) // 종이 오브젝트 배열의 길이만큼 반복
                {
                    // 종이 오브젝트와 종이 UI를 연결
                    GameObject paperObject = paperObjects[i];
                    GameObject paperUI = paperUIs[i];

                    if (col.gameObject == paperObject) // 콜라이더와 현재 종이 오브젝트가 일치하는지 확인
                    {
                        // 이미 활성화된 UI인지 확인하고 비활성화
                        if (paperUI.activeSelf)
                        {
                            paperUI.SetActive(false);
                        }
                        else
                        {
                            // 다른 모든 종이 UI를 비활성화
                            foreach (GameObject ui in paperUIs)
                            {
                                ui.SetActive(false);
                            }

                            // 현재 종이에 해당하는 UI를 활성화
                            paperUI.SetActive(true);
                        }
                        break; // 종이 오브젝트를 찾았으므로 반복문 종료
                    }
                }
            }
        }
    }
}
