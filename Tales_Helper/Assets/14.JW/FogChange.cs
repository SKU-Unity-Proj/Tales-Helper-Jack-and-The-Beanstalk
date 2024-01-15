using UnityEngine;
using System.Collections;

public class FogChange: MonoBehaviour
{
    // �浹 �� Fog�� ����� ��
    public float fogDensityOnCollision = 0.03f;
    // Fog ���濡 �ɸ��� �ð�
    public float fogChangeDuration = 2.0f;

    private float originalFogDensity;
    private bool fogChanging = false;

    void Start()
    {
        // ������ �� ������ Fog ���� ����
        originalFogDensity = RenderSettings.fogDensity;
    }

    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� �±װ� "Fog"���� Ȯ���ϰ�, ���� ���� �ƴ� ��쿡�� ó��
        if (other.CompareTag("Fog") && !fogChanging)
        {
            // �浹 �� Fog ���� ������ ����
            StartCoroutine(ChangeFog());
            // �浹�� "Fog" �±׸� ���� ������Ʈ�� ��Ȱ��ȭ
            other.gameObject.SetActive(false);
        }
    }

    IEnumerator ChangeFog()
    {
        fogChanging = true; // Fog ���� �� �÷��� Ȱ��ȭ

        float elapsedTime = 0f;

        while (elapsedTime < fogChangeDuration)
        {
            // ���� Fog ���� ������ ����
            RenderSettings.fogDensity = Mathf.Lerp(originalFogDensity, fogDensityOnCollision, elapsedTime / fogChangeDuration);

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ������ �Ϸ�� �� ���� ��ǥ������ ����
        RenderSettings.fogDensity = fogDensityOnCollision;

        fogChanging = false; // Fog ���� �� �÷��� ��Ȱ��ȭ
    }
}