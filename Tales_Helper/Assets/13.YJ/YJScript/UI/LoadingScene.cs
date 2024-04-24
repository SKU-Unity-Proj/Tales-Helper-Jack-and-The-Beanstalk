using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static string nextScene; // ���� ���� �̸��� �����ϴ� ����

    [SerializeField]
    Image progressBar; // ���� ��Ȳ�� ǥ���� �̹��� ���α׷��� ��

    private void Start()
    {
        StartCoroutine(LoadScene()); // �� �ε� �ڷ�ƾ�� ����
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName; // ���� ���� �̸��� ����
        SceneManager.LoadScene("LoadingScene"); // �ε� ���� �ε�
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f); // 1�� ���

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // �񵿱�� ���� �ε��ϴ� ���۷��̼� ����
        op.allowSceneActivation = false; // �� ��ȯ�� ������� ����

        float timer = 0.0f; // Ÿ�̸� �ʱ�ȭ

        while (!op.isDone)
        {
            yield return null; // �� ������ ���

            timer += Time.deltaTime; // �ð� ����
            if (op.progress < 0.9f)
            {
                // �� �ε� ���൵�� 90% �̸��̸�
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                // ���α׷��� ���� ä���� ������ ���������� ������Ŵ
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f; // Ÿ�̸� �ʱ�ȭ
                }
            }
            else
            {
                // �� �ε� ���൵�� 90% �̻��̸�
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                // ���α׷��� �ٸ� �� ä��
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true; // �� ��ȯ�� �����
                    yield break; // �ڷ�ƾ ����
                }
            }
        }
    }
}
