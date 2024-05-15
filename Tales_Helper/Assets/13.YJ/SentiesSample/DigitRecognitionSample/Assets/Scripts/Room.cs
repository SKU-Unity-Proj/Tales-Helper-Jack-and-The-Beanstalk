using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * 방에 대한 게임 로직을 처리합니다.
 *  - 문을 열고 닫습니다.
 *  - 각 방에 다양한 단서를 설정합니다.
 *  - 오디오 메시지를 처리합니다.
 *  - 패널 위의 라이트가 올바른 답을 보여줍니다.
 */
public class Room : MonoBehaviour
{
    public GameObject panel; // 패널 게임 오브젝트
    public GameObject[] lights; // 라이트 게임 오브젝트 배열
    public GameObject[] clue; // 단서 게임 오브젝트 배열
    public AudioClip message; // 오디오 클립

    public bool fruitCodes = false; // 과일 레벨의 특정 코드
    public int[] code; // 비밀 코드
    public int codePosition = 0; // 입력 중인 위치 (첫 번째, 두 번째 또는 세 번째 숫자인지)

    public CinemachineVirtualCamera screenCam;
    public Liftmove liftmove;

    // 비밀 코드를 무작위로 재설정하고 방에 단서 이미지를 설정합니다.
    public void ResetCode()
    {
        code = new int[] { 7, 5, 4 }; // 정답 코드 초기화.

        // 무작위 배열에서 첫 번째 세 개의 숫자를 코드에 할당하고 단서 소재를 설정합니다.
        for (int i = 0; i < code.Length; i++)
        {
            if (i >= clue.Length)
                return;

            var clueMaterial = clue[i].GetComponent<Renderer>().material; // 단서 소재를 가져옵니다.
            if (fruitCodes)
            {
                // 과일 단서 이미지를 변경합니다.
                clueMaterial.mainTextureOffset = new Vector2((code[i] - 1) / 6f, 0);
            }
            else
            {
                clueMaterial.mainTexture = Game.instance.digits[code[i]]; // 숫자 단서 이미지를 설정합니다.
            }
        }
    }

    // 코드를 확인하고 완전한 코드를 입력했는지 확인합니다.
    public (bool correct, bool completed) CheckCode(int digitGuess)
    {
        bool isCorrectGuess = (digitGuess == code[codePosition]); // 올바른 추측인지 확인합니다.

        if (isCorrectGuess)
        {
            // 라이트를 초록색으로 바꿉니다.
            for (int i = 0; i < lights.Length; i++)
            {
                var lightMaterial = lights[i].GetComponent<Renderer>().material;
                //lightMaterial.SetColor("_EmissionColor", i <= codePosition ? Color.green : Color.black);
                lightMaterial.color = (i <= codePosition) ? Color.green : Color.black;
            }
                
            codePosition++; // 다음 위치로 이동
            bool everyDigitCorrect = (codePosition == code.Length); // 모든 숫자가 맞았는지 확인
            if (everyDigitCorrect)
            {
                codePosition = 0; // 위치 초기화


                //정답 맞출시 동작
                screenCam.Priority = 2;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Debug.Log("Elevator On");
                liftmove.enabled = true;


                return (correct: true, completed: true); // 올바르고 완료됨
            }
            else
            {
                return (correct: true, completed: false); // 올바르지만 미완료
            }
        }
        else // 잘못된 추측
        {
            // 라이트를 끕니다.
            for (int i = 0; i < lights.Length; i++)
            {
                var lightMaterial = lights[i].GetComponent<Renderer>().material;
                //lightMaterial.SetColor("_EmissionColor", Color.black);
                lightMaterial.color = Color.black;
            }
            codePosition = 0; // 위치 초기화
            return (correct: false, completed: false); // 잘못되었고 미완료
        }
    }
}
