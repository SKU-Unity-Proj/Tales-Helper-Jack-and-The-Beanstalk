using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * 방에 대한 게임 로직을 처리합니다.
 *  - 문을 열고 닫습니다.
 *  - 각 방에 다양한 단서를 설정합니다.
 *  - 오디오 메시지를 처리합니다.
 *  - 패널 위의 라이트가 올바른 답을 보여줍니다.
 */
public class Room : MonoBehaviour
{
    public GameObject door, door2; // 문 게임 오브젝트
    public GameObject panel; // 패널 게임 오브젝트
    public GameObject[] lights; // 라이트 게임 오브젝트 배열
    public GameObject[] clue; // 단서 게임 오브젝트 배열
    public AudioClip message; // 오디오 클립

    public bool fruitCodes = false; // 과일 레벨의 특정 코드
    public int[] code; // 비밀 코드
    public int codePosition = 0; // 입력 중인 위치 (첫 번째, 두 번째 또는 세 번째 숫자인지)
    public enum DOOR_STATE { OPEN, CLOSED, OPENING, CLOSING }; // 문의 상태를 나타내는 열거형
    public DOOR_STATE doorState = DOOR_STATE.CLOSED; // 문의 초기 상태는 닫힘

    // 지원하는 숫자
    int[] numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    int[] fruitNumbers = new int[] { 1, 2, 3, 4, 5, 6 };

    // 문 상태 (위치/열리는 시간/폭)
    float startTimeOfDoorState = 0;
    Vector3 doorClosedPosition;
    Vector3 door2ClosedPosition;
    float timeOfDoorOpening = 0.5f;
    float doorWidth = 4f;

    void Start()
    {
        if (door) doorClosedPosition = door.transform.position;
        if (door2) door2ClosedPosition = door2.transform.position;
    }

    // 비밀 코드를 무작위로 재설정하고 방에 단서 이미지를 설정합니다.
    public void ResetCode()
    {
        code = new int[] { 0, 0, 0 }; // 코드를 초기화합니다.

        int[] randomNumbers = GetRandomizedNumbers(); // 무작위로 숫자를 섞어 가져옵니다.

        // 무작위 배열에서 첫 번째 세 개의 숫자를 코드에 할당하고 단서 소재를 설정합니다.
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = randomNumbers[i]; // 코드에 무작위 숫자를 할당합니다.

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

    // 세 가지 다른 숫자를 얻으려고 합니다. 0..9의 숫자를 섞은 다음 첫 번째 세 개를 선택할 수 있습니다.
    // 또는 과일의 경우 1..6의 숫자를 섞을 수 있습니다.
    int[] GetRandomizedNumbers()
    {
        if (fruitCodes)
        {
            return fruitNumbers.OrderBy((x) => Random.Range(0, 1f)).ToArray(); // 과일 숫자를 무작위로 섞어 반환합니다.
        }
        else
        {
            return numbers.OrderBy((x) => Random.Range(0, 1f)).ToArray(); // 숫자를 무작위로 섞어 반환합니다.
        }
    }

    // 문을 엽니다.
    public void OpenDoor()
    {
        if (!door) return;
        if (doorState == DOOR_STATE.CLOSED)
        {
            doorState = DOOR_STATE.OPENING;
            startTimeOfDoorState = Time.time;
            door.GetComponent<AudioSource>().Play(); // 문을 열 때 소리를 재생합니다.
        }
        if (doorState == DOOR_STATE.OPEN)
        {
            doorState = DOOR_STATE.CLOSING;
            startTimeOfDoorState = Time.time;
            door.GetComponent<AudioSource>().Play(); // 문을 닫을 때 소리를 재생합니다.
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


    void Update()
    {
        // 문이 열리고 닫히는 애니메이션
        float t = (Time.time - startTimeOfDoorState) / timeOfDoorOpening; // 문이 열리거나 닫히는 시간에 따라 t 값을 계산합니다.
        if (doorState == DOOR_STATE.OPENING)
        {
            OpeningDoor(t); // 문이 열리는 중이면 OpeningDoor 함수를 호출하여 문을 엽니다.
        }
        if (doorState == DOOR_STATE.CLOSING)
        {
            ClosingDoor(t); // 문이 닫히는 중이면 ClosingDoor 함수를 호출하여 문을 닫습니다.
        }
    }

    void OpeningDoor(float t)
    {
        if (!door && !door2) return; // 문이나 문2가 없으면 함수를 종료합니다.

        // 문을 열 때의 위치를 설정합니다.
        door.transform.position = doorClosedPosition + door.transform.right * Mathf.Min(1, t) * doorWidth;
        door2.transform.position = door2ClosedPosition - door2.transform.right * Mathf.Min(1, t) * doorWidth;

        // 문이 완전히 열렸는지 확인합니다.
        if (t > 1)
        {
            doorState = DOOR_STATE.OPEN; // 문의 상태를 OPEN으로 설정합니다.
        }
    }

    void ClosingDoor(float t)
    {
        if (!door || !door2) return; // 문이나 문2가 없으면 함수를 종료합니다.

        // 문을 닫을 때의 위치를 설정합니다.
        door.transform.position = doorClosedPosition + door.transform.right * Mathf.Max(0, 1 - t) * doorWidth;
        door2.transform.position = doorClosedPosition - door2.transform.right * Mathf.Max(0, 1 - t) * doorWidth;

        // 문이 완전히 닫혔는지 확인합니다.
        if (t > 1)
        {
            doorState = DOOR_STATE.CLOSED; // 문의 상태를 CLOSED로 설정합니다.
        }
    }
}
