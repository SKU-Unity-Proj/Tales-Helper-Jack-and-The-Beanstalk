using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] private GameObject NoticeUI; // 알림 UI를 가리키는 변수
    private bool isUIopen = true; // UI가 현재 열려있는지 여부를 추적하는 변수
    public LayerMask layerMask; // 충돌을 감지할 레이어를 결정하는 변수

    void Update()
    {
        CheckUI(); // UI 상태를 확인하는 함수 호출
    }

    // UI 상태를 확인하는 함수
    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F)) // F 키 입력이 감지되면
        {
            if (NoticeUI.activeSelf) // UI가 활성화되어 있다면
            {
                NoticeUI.SetActive(false); // UI를 비활성화하고
                isUIopen = false; // UI 상태를 닫힌 상태로 변경
            }

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f, layerMask); // 주변에 반경 3f 내에 있는 물체들의 충돌을 검사

            foreach (Collider col in colliders) // 감지된 각 충돌에 대해 반복
            {
                if (!NoticeUI.activeSelf && isUIopen) // UI가 비활성화되어 있고 이전에 UI가 열려있는 상태였다면
                {
                    NoticeUI.SetActive(true); // UI를 활성화하고
                    break; // 반복문 종료
                }
                isUIopen = true; // UI 상태를 열린 상태로 변경
            }

        }
    }
}
