using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * �濡 ���� ���� ������ ó���մϴ�.
 *  - ���� ���� �ݽ��ϴ�.
 *  - �� �濡 �پ��� �ܼ��� �����մϴ�.
 *  - ����� �޽����� ó���մϴ�.
 *  - �г� ���� ����Ʈ�� �ùٸ� ���� �����ݴϴ�.
 */
public class Room : MonoBehaviour
{
    public GameObject door, door2; // �� ���� ������Ʈ
    public GameObject panel; // �г� ���� ������Ʈ
    public GameObject[] lights; // ����Ʈ ���� ������Ʈ �迭
    public GameObject[] clue; // �ܼ� ���� ������Ʈ �迭
    public AudioClip message; // ����� Ŭ��

    public bool fruitCodes = false; // ���� ������ Ư�� �ڵ�
    public int[] code; // ��� �ڵ�
    public int codePosition = 0; // �Է� ���� ��ġ (ù ��°, �� ��° �Ǵ� �� ��° ��������)
    public enum DOOR_STATE { OPEN, CLOSED, OPENING, CLOSING }; // ���� ���¸� ��Ÿ���� ������
    public DOOR_STATE doorState = DOOR_STATE.CLOSED; // ���� �ʱ� ���´� ����

    // �����ϴ� ����
    int[] numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    int[] fruitNumbers = new int[] { 1, 2, 3, 4, 5, 6 };

    // �� ���� (��ġ/������ �ð�/��)
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

    // ��� �ڵ带 �������� �缳���ϰ� �濡 �ܼ� �̹����� �����մϴ�.
    public void ResetCode()
    {
        code = new int[] { 0, 0, 0 }; // �ڵ带 �ʱ�ȭ�մϴ�.

        int[] randomNumbers = GetRandomizedNumbers(); // �������� ���ڸ� ���� �����ɴϴ�.

        // ������ �迭���� ù ��° �� ���� ���ڸ� �ڵ忡 �Ҵ��ϰ� �ܼ� ���縦 �����մϴ�.
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = randomNumbers[i]; // �ڵ忡 ������ ���ڸ� �Ҵ��մϴ�.

            if (i >= clue.Length)
                return;

            var clueMaterial = clue[i].GetComponent<Renderer>().material; // �ܼ� ���縦 �����ɴϴ�.
            if (fruitCodes)
            {
                // ���� �ܼ� �̹����� �����մϴ�.
                clueMaterial.mainTextureOffset = new Vector2((code[i] - 1) / 6f, 0);
            }
            else
            {
                clueMaterial.mainTexture = Game.instance.digits[code[i]]; // ���� �ܼ� �̹����� �����մϴ�.
            }
        }
    }

    // �� ���� �ٸ� ���ڸ� �������� �մϴ�. 0..9�� ���ڸ� ���� ���� ù ��° �� ���� ������ �� �ֽ��ϴ�.
    // �Ǵ� ������ ��� 1..6�� ���ڸ� ���� �� �ֽ��ϴ�.
    int[] GetRandomizedNumbers()
    {
        if (fruitCodes)
        {
            return fruitNumbers.OrderBy((x) => Random.Range(0, 1f)).ToArray(); // ���� ���ڸ� �������� ���� ��ȯ�մϴ�.
        }
        else
        {
            return numbers.OrderBy((x) => Random.Range(0, 1f)).ToArray(); // ���ڸ� �������� ���� ��ȯ�մϴ�.
        }
    }

    // ���� ���ϴ�.
    public void OpenDoor()
    {
        if (!door) return;
        if (doorState == DOOR_STATE.CLOSED)
        {
            doorState = DOOR_STATE.OPENING;
            startTimeOfDoorState = Time.time;
            door.GetComponent<AudioSource>().Play(); // ���� �� �� �Ҹ��� ����մϴ�.
        }
        if (doorState == DOOR_STATE.OPEN)
        {
            doorState = DOOR_STATE.CLOSING;
            startTimeOfDoorState = Time.time;
            door.GetComponent<AudioSource>().Play(); // ���� ���� �� �Ҹ��� ����մϴ�.
        }
    }

    // �ڵ带 Ȯ���ϰ� ������ �ڵ带 �Է��ߴ��� Ȯ���մϴ�.
    public (bool correct, bool completed) CheckCode(int digitGuess)
    {
        bool isCorrectGuess = (digitGuess == code[codePosition]); // �ùٸ� �������� Ȯ���մϴ�.

        if (isCorrectGuess)
        {
            // ����Ʈ�� �ʷϻ����� �ٲߴϴ�.
            for (int i = 0; i < lights.Length; i++)
            {
                var lightMaterial = lights[i].GetComponent<Renderer>().material;
                //lightMaterial.SetColor("_EmissionColor", i <= codePosition ? Color.green : Color.black);
                lightMaterial.color = (i <= codePosition) ? Color.green : Color.black;
            }
                
            codePosition++; // ���� ��ġ�� �̵�
            bool everyDigitCorrect = (codePosition == code.Length); // ��� ���ڰ� �¾Ҵ��� Ȯ��
            if (everyDigitCorrect)
            {
                codePosition = 0; // ��ġ �ʱ�ȭ
                return (correct: true, completed: true); // �ùٸ��� �Ϸ��
            }
            else
            {
                return (correct: true, completed: false); // �ùٸ����� �̿Ϸ�
            }
        }
        else // �߸��� ����
        {
            // ����Ʈ�� ���ϴ�.
            for (int i = 0; i < lights.Length; i++)
            {
                var lightMaterial = lights[i].GetComponent<Renderer>().material;
                //lightMaterial.SetColor("_EmissionColor", Color.black);
                lightMaterial.color = Color.black;
            }
            codePosition = 0; // ��ġ �ʱ�ȭ
            return (correct: false, completed: false); // �߸��Ǿ��� �̿Ϸ�
        }
    }


    void Update()
    {
        // ���� ������ ������ �ִϸ��̼�
        float t = (Time.time - startTimeOfDoorState) / timeOfDoorOpening; // ���� �����ų� ������ �ð��� ���� t ���� ����մϴ�.
        if (doorState == DOOR_STATE.OPENING)
        {
            OpeningDoor(t); // ���� ������ ���̸� OpeningDoor �Լ��� ȣ���Ͽ� ���� ���ϴ�.
        }
        if (doorState == DOOR_STATE.CLOSING)
        {
            ClosingDoor(t); // ���� ������ ���̸� ClosingDoor �Լ��� ȣ���Ͽ� ���� �ݽ��ϴ�.
        }
    }

    void OpeningDoor(float t)
    {
        if (!door && !door2) return; // ���̳� ��2�� ������ �Լ��� �����մϴ�.

        // ���� �� ���� ��ġ�� �����մϴ�.
        door.transform.position = doorClosedPosition + door.transform.right * Mathf.Min(1, t) * doorWidth;
        door2.transform.position = door2ClosedPosition - door2.transform.right * Mathf.Min(1, t) * doorWidth;

        // ���� ������ ���ȴ��� Ȯ���մϴ�.
        if (t > 1)
        {
            doorState = DOOR_STATE.OPEN; // ���� ���¸� OPEN���� �����մϴ�.
        }
    }

    void ClosingDoor(float t)
    {
        if (!door || !door2) return; // ���̳� ��2�� ������ �Լ��� �����մϴ�.

        // ���� ���� ���� ��ġ�� �����մϴ�.
        door.transform.position = doorClosedPosition + door.transform.right * Mathf.Max(0, 1 - t) * doorWidth;
        door2.transform.position = doorClosedPosition - door2.transform.right * Mathf.Max(0, 1 - t) * doorWidth;

        // ���� ������ �������� Ȯ���մϴ�.
        if (t > 1)
        {
            doorState = DOOR_STATE.CLOSED; // ���� ���¸� CLOSED�� �����մϴ�.
        }
    }
}
