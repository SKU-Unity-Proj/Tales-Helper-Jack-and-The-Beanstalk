using UnityEngine;

public class PaperUI : MonoBehaviour
{
    [SerializeField] private GameObject[] paperObjects; // 서로 다른 종이 오브젝트들을 저장하는 배열
    [SerializeField] private GameObject[] paperUIs; // 서로 다른 종이 UI들을 저장하는 배열
    public LayerMask layerMask; // OverlapSphere로 플레이어와 상호작용하는 콜라이더를 제한하기 위한 레이어 마스크

    void Update()
    {
        CheckUI();
    }

    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            
            // 해당 레이어를 가진 콜라이더 판별
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, layerMask);
            foreach (Collider col in colliders)
            {
                for (int i = 0; i < paperObjects.Length; i++)
                {

                    // 각 종이에 알맞은 ui를 지정해주기 위해 서로 연결
                    GameObject paperObject = paperObjects[i];
                    GameObject paperUI = paperUIs[i];

                    if (col.gameObject == paperObject) //콜라이더와 종이 오브젝트가 일치하는지 확인 조건
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

                            // 올바른 종이 UI 활성화
                            paperUI.SetActive(true);
                        }
                        break;
                    }
                }
            }
        }
    }
}
