using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static string nextScene; // 다음 씬의 이름을 저장하는 변수

    [SerializeField]
    Image progressBar; // 진행 상황을 표시할 이미지 프로그래스 바

    private void Start()
    {
        StartCoroutine(LoadScene()); // 씬 로딩 코루틴을 시작
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName; // 다음 씬의 이름을 설정
        SceneManager.LoadScene("LoadingScene"); // 로딩 씬을 로드
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // 비동기로 씬을 로드하는 오퍼레이션 생성
        op.allowSceneActivation = false; // 씬 전환을 허용하지 않음

        float timer = 0.0f; // 타이머 초기화

        while (!op.isDone)
        {
            yield return null; // 한 프레임 대기

            timer += Time.deltaTime; // 시간 증가
            if (op.progress < 0.9f)
            {
                // 씬 로딩 진행도가 90% 미만이면
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                // 프로그래스 바의 채워진 정도를 점진적으로 증가시킴
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f; // 타이머 초기화
                }
            }
            else
            {
                // 씬 로딩 진행도가 90% 이상이면
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                // 프로그래스 바를 다 채움
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true; // 씬 전환을 허용함
                    yield break; // 코루틴 종료
                }
            }
        }
    }
}
