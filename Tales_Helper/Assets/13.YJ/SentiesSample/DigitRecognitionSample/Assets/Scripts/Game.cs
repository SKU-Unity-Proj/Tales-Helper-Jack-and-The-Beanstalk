using UnityEngine;

/*
 * �� Ŭ������ �÷��̾ �����մϴ�. ���콺�� Ű����� ���� ����� �Է��� �ٷ�ϴ�.
 * ���� ���� ���¸� �����մϴ�.
 */
public class Game : MonoBehaviour
{
    public GameObject player; // �÷��̾� ���� ������Ʈ
    public Room[] rooms; // �� �迭
    public AudioClip[] numbers; // ���� ����� Ŭ�� �迭
    public Texture2D[] digits; // ���� �ؽ�ó �迭
    public Camera playerCamera; // �÷��̾� ī�޶�
    public Texture2D circleCursor; // ���� Ŀ�� �ؽ�ó

    public enum MODE { WALK, CONTROL }; // ���� ��� ������
    public MODE mode = MODE.CONTROL; // �ʱ� ���� ���� �ȱ�

    Room currentRoom; // ���� ��
    CharacterController controller; // ĳ���� ��Ʈ�ѷ�
    float speed = 4; // �ȱ� �ӵ�
    float gravity = 0; // �߷����� ���� ���� ���� �ӵ�

    public static Game instance; // ���� �ν��Ͻ�

    public bool isSample = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; // �ν��Ͻ� ����
        controller = player.GetComponent<CharacterController>(); // ĳ���� ��Ʈ�ѷ� ����
        Cursor.lockState = CursorLockMode.Locked; // Ŀ���� ��� ���� ����

        // �� �濡 ���� �ڵ带 �缳���ϰ� �г� �ݹ��� �����մϴ�.
        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].ResetCode(); // �ڵ� �ʱ�ȭ
            var panel = rooms[i].GetComponent<Panel>(); // ���� �г� ��������
            if (panel != null) panel.callback = GotNumber; // �г� �ݹ� ����
        }
        currentRoom = rooms[0]; // ���� ���� ù ��° ������ ����
    }


    // �гηκ��� ���ڰ� �ԷµǸ� ȣ��˴ϴ�. ������ ���ڿ� Ȯ���� �����մϴ�.
    void GotNumber(Room room, int n, float probability)
    {
        GetComponent<AudioSource>().PlayOneShot(numbers[n]); // ���ڿ� �ش��ϴ� ����� Ŭ�� ���
        Debug.Log("Predicted number " + n + "\nProbability " + (probability * 100) + "%");

        // �ڵ尡 �ùٸ��� Ȯ���մϴ�.
        (bool correct, bool completed) = room.CheckCode(n);
        if (!correct)
        {
            currentRoom = room; // ���� �� ����
            Invoke("SoundAlarm", 0.5f); // �溸�� ���
        }
    }

    // �溸�� ��� �� �ڵ� �缳��
    void SoundAlarm()
    {
        currentRoom.GetComponent<Panel>().SoundAlarm(); // �溸�� ���
        currentRoom.ResetCode(); // �ڵ� �缳��
    }

    // �޽��� ���
    void PlayMessage()
    {
        if (currentRoom.message != null)
        {
            GetComponent<AudioSource>().PlayOneShot(currentRoom.message); // �޽��� ���
        }
    }

    void Update()
    {
        if(isSample) 
        {
            // �÷��̾� �̵�:
            float mouseSensitivity = 1f;
            float vert = Input.GetAxis("Vertical"); // ���� �Է�
            float horiz = Input.GetAxis("Horizontal"); // ���� �Է�
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // ���콺 X �� �Է�

            float factor = 1; // �̵� �ӵ� ���
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) factor = 2; // Shift Ű�� ������ �̵� �ӵ��� �� �谡 �˴ϴ�.

            gravity -= 9.81f * Time.deltaTime; // �߷� ����
            controller.Move((player.transform.forward * vert + player.transform.right * horiz) * Time.deltaTime * speed * factor + player.transform.up * (gravity) * Time.deltaTime); // �̵�
            if (controller.isGrounded) gravity = 0; // ���� ������ �߷� �ʱ�ȭ

            if (mode == MODE.WALK)
            {
                controller.transform.Rotate(Vector3.up, mouseX); // �ȱ� ��忡���� ���콺 X ������ ȸ���մϴ�.
            }

            // ���콺�� ����Ͽ� ���� �� ȭ�鿡 �׸��� ���̸� ��ȯ�մϴ�.
            if (Input.GetKeyDown(KeyCode.F))
            {
                switch (mode)
                {
                    case MODE.WALK:
                        mode = MODE.CONTROL;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.SetCursor(circleCursor, new Vector2(16, 16), CursorMode.Auto); // Ŀ���� �������� ����
                        break;
                    case MODE.CONTROL:
                        mode = MODE.WALK;
                        Cursor.lockState = CursorLockMode.Locked;
                        break;
                }
            }

            // ���� ����:
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Application.Quit(); // ���ø����̼� ����
            }
        }
    }
}
