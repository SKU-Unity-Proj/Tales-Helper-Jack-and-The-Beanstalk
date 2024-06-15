using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LtoBsceneChange : MonoBehaviour
{
    private Animator _animator;

    // 특정 애니메이션 상태의 이름
    [SerializeField] private string targetAnimationState = "LockDoor_Open_R";

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 현재 애니메이터의 상태를 가져옴
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // 특정 애니메이션 상태가 실행 중인지 확인
        if (stateInfo.IsName(targetAnimationState))
        {
            // 코루틴 시작
            StartCoroutine(ChangeSceneAfterDelay(3f));
        }
    }

    IEnumerator ChangeSceneAfterDelay(float delay)
    {
        // 대기 시간
        yield return new WaitForSeconds(delay);

        // 씬 변경
        SceneManager.LoadScene("GiantMap-Bedroom");
    }
}
