using UnityEngine;
using System.Collections;

public class ValveController : MonoBehaviour
{
    public bool isUnlocked = false; // ���� ��� ����
    public float rotationSpeed = 360f; // ���� ȸ�� �ӵ� (��/��)

    public Transform valveTransform; // ���� ������Ʈ�� Transform
    public WindowController linkedWindow; // ����� â��

    public SoundList valveSound, lockSound;

    public void Interact()
    {
        if (isUnlocked)
        {
            Debug.Log("�̹� ���갡 ���� �ֽ��ϴ�.");
            return;
        }

        if (linkedWindow != null && linkedWindow.IsMatched())
        {
            StartCoroutine(TurnValve());
        }
        else
        {
            Debug.Log("â���� ���� ����� ���긦 ���� �� �ֽ��ϴ�.");
            SoundManager.Instance.PlayOneShotEffect((int)lockSound, transform.position, 1f);
        }
    }

    private IEnumerator TurnValve()
    {
        float totalRotation = 0f;

        SoundManager.Instance.PlayOneShotEffect((int)valveSound, transform.position, 1f);

        while (totalRotation < 720f) // �� ���� ȸ�� (360��)
        {
            float rotationStep = rotationSpeed * Time.deltaTime; // ������ ��� ȸ���� ���
            valveTransform.Rotate(0, -rotationStep, 0); // Z�� �������� ȸ��
            totalRotation += rotationStep;
            yield return null;
        }

        // ȸ�� �Ϸ� �� Z�� �� ����
        valveTransform.localEulerAngles = new Vector3(
            valveTransform.localEulerAngles.x,
            valveTransform.localEulerAngles.y,
            Mathf.Round(valveTransform.localEulerAngles.z) // ȸ������ ��Ȯ�� 360���� ����
        );

        Debug.Log("���긦 ���������� ȸ�����׽��ϴ�.");
        isUnlocked = true; // ���긦 ��� ���� ���·� ����
    }
}
