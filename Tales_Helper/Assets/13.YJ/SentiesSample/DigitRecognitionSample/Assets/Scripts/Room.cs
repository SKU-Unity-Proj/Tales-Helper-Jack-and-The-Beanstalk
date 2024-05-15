using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * �濡 ���� ���� ������ ó���մϴ�.
 *  - ���� ���� �ݽ��ϴ�.
 *  - �� �濡 �پ��� �ܼ��� �����մϴ�.
 *  - ����� �޽����� ó���մϴ�.
 *  - �г� ���� ����Ʈ�� �ùٸ� ���� �����ݴϴ�.
 */
public class Room : MonoBehaviour
{
    public GameObject panel; // �г� ���� ������Ʈ
    public GameObject[] lights; // ����Ʈ ���� ������Ʈ �迭
    public GameObject[] clue; // �ܼ� ���� ������Ʈ �迭
    public AudioClip message; // ����� Ŭ��

    public bool fruitCodes = false; // ���� ������ Ư�� �ڵ�
    public int[] code; // ��� �ڵ�
    public int codePosition = 0; // �Է� ���� ��ġ (ù ��°, �� ��° �Ǵ� �� ��° ��������)

    public CinemachineVirtualCamera screenCam;
    public Liftmove liftmove;

    // ��� �ڵ带 �������� �缳���ϰ� �濡 �ܼ� �̹����� �����մϴ�.
    public void ResetCode()
    {
        code = new int[] { 7, 5, 4 }; // ���� �ڵ� �ʱ�ȭ.

        // ������ �迭���� ù ��° �� ���� ���ڸ� �ڵ忡 �Ҵ��ϰ� �ܼ� ���縦 �����մϴ�.
        for (int i = 0; i < code.Length; i++)
        {
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


                //���� ����� ����
                screenCam.Priority = 2;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Debug.Log("Elevator On");
                liftmove.enabled = true;


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
}
