using UnityEngine;

/*
 * 이 클래스는 플레이어를 제어합니다. 마우스와 키보드와 같은 사용자 입력을 다룹니다.
 * 또한 게임 상태를 제어합니다.
 */
public class Game : MonoBehaviour
{
    public GameObject player; // 플레이어 게임 오브젝트
    public Room[] rooms; // 방 배열
    public AudioClip[] numbers; // 숫자 오디오 클립 배열
    public Texture2D[] digits; // 숫자 텍스처 배열
    public Camera playerCamera; // 플레이어 카메라
    public Texture2D circleCursor; // 원형 커서 텍스처

    public enum MODE { WALK, CONTROL }; // 게임 모드 열거형
    public MODE mode = MODE.CONTROL; // 초기 게임 모드는 걷기

    Room currentRoom; // 현재 방
    CharacterController controller; // 캐릭터 컨트롤러
    float speed = 4; // 걷기 속도
    float gravity = 0; // 중력으로 인한 현재 낙하 속도

    public static Game instance; // 게임 인스턴스

    public bool isSample = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // 인스턴스 설정
        controller = player.GetComponent<CharacterController>(); // 캐릭터 컨트롤러 설정
        Cursor.lockState = CursorLockMode.Locked; // 커서를 잠금 모드로 설정

        // 각 방에 대해 코드를 재설정하고 패널 콜백을 설정합니다.
        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].ResetCode(); // 코드 초기화
            var panel = rooms[i].GetComponent<Panel>(); // 방의 패널 가져오기
            if (panel != null) panel.callback = GotNumber; // 패널 콜백 설정
        }
        currentRoom = rooms[0]; // 현재 방을 첫 번째 방으로 설정
    }


    // 패널로부터 숫자가 입력되면 호출됩니다. 예측된 숫자와 확률을 전달합니다.
    void GotNumber(Room room, int n, float probability)
    {
        GetComponent<AudioSource>().PlayOneShot(numbers[n]); // 숫자에 해당하는 오디오 클립 재생
        Debug.Log("Predicted number " + n + "\nProbability " + (probability * 100) + "%");

        // 코드가 올바른지 확인합니다.
        (bool correct, bool completed) = room.CheckCode(n);
        if (!correct)
        {
            currentRoom = room; // 현재 방 설정
            Invoke("SoundAlarm", 0.5f); // 경보음 재생
        }
    }

    // 경보음 재생 및 코드 재설정
    void SoundAlarm()
    {
        currentRoom.GetComponent<Panel>().SoundAlarm(); // 경보음 재생
        currentRoom.ResetCode(); // 코드 재설정
    }

    // 메시지 재생
    void PlayMessage()
    {
        if (currentRoom.message != null)
        {
            GetComponent<AudioSource>().PlayOneShot(currentRoom.message); // 메시지 재생
        }
    }

    void Update()
    {
        if(isSample) 
        {
            // 플레이어 이동:
            float mouseSensitivity = 1f;
            float vert = Input.GetAxis("Vertical"); // 수직 입력
            float horiz = Input.GetAxis("Horizontal"); // 수평 입력
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // 마우스 X 축 입력

            float factor = 1; // 이동 속도 계수
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) factor = 2; // Shift 키를 누르면 이동 속도가 두 배가 됩니다.

            gravity -= 9.81f * Time.deltaTime; // 중력 적용
            controller.Move((player.transform.forward * vert + player.transform.right * horiz) * Time.deltaTime * speed * factor + player.transform.up * (gravity) * Time.deltaTime); // 이동
            if (controller.isGrounded) gravity = 0; // 땅에 닿으면 중력 초기화

            if (mode == MODE.WALK)
            {
                controller.transform.Rotate(Vector3.up, mouseX); // 걷기 모드에서는 마우스 X 축으로 회전합니다.
            }

            // 마우스를 사용하여 보기 및 화면에 그리기 사이를 전환합니다.
            if (Input.GetKeyDown(KeyCode.F))
            {
                switch (mode)
                {
                    case MODE.WALK:
                        mode = MODE.CONTROL;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.SetCursor(circleCursor, new Vector2(16, 16), CursorMode.Auto); // 커서를 원형으로 설정
                        break;
                    case MODE.CONTROL:
                        mode = MODE.WALK;
                        Cursor.lockState = CursorLockMode.Locked;
                        break;
                }
            }

            // 게임 종료:
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Application.Quit(); // 어플리케이션 종료
            }
        }
    }
}
