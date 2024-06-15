using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LtoBsceneChange : MonoBehaviour
{
    private Animator _animator;

    // Ư�� �ִϸ��̼� ������ �̸�
    [SerializeField] private string targetAnimationState = "LockDoor_Open_R";

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ���� �ִϸ������� ���¸� ������
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // Ư�� �ִϸ��̼� ���°� ���� ������ Ȯ��
        if (stateInfo.IsName(targetAnimationState))
        {
            // �ڷ�ƾ ����
            StartCoroutine(ChangeSceneAfterDelay(3f));
        }
    }

    IEnumerator ChangeSceneAfterDelay(float delay)
    {
        // ��� �ð�
        yield return new WaitForSeconds(delay);

        // �� ����
        SceneManager.LoadScene("GiantMap-Bedroom");
    }
}
