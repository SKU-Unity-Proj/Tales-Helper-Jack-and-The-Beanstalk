using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactInfo : MonoBehaviour
{
    public GameObject infoImage; // ���� ���� �̹��� (UI Canvas�� ��ġ�� GameObject)
    private bool hasInteracted = false; // �÷��̾ �̹� ��ȣ�ۿ��ߴ��� ����

    void Start()
    {
        // ������ �� �̹��� ��Ȱ��ȭ
        if (infoImage != null)
        {
            infoImage.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            TriggerInteraction();
        }
    }

    private void TriggerInteraction()
    {
        // �̹��� Ȱ��ȭ �� ���� ����
        if (infoImage != null)
        {
            infoImage.SetActive(true);
        }
        Time.timeScale = 0f; // ���� ����
        hasInteracted = true; // �� ���� ����ǵ��� ����
    }

    void Update()
    {
        // �����̽��ٸ� ������ �̹��� ��Ȱ��ȭ �� ���� �簳
        if (infoImage != null && infoImage.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            infoImage.SetActive(false);
            Time.timeScale = 1f; // ���� �簳
        }
    }
}
