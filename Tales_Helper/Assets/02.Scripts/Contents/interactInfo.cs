using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactInfo : MonoBehaviour
{
    public GameObject infoImage; // 띄우고 싶은 이미지 (UI Canvas에 위치한 GameObject)
    private bool hasInteracted = false; // 플레이어가 이미 상호작용했는지 여부

    void Start()
    {
        // 시작할 때 이미지 비활성화
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
        // 이미지 활성화 및 게임 정지
        if (infoImage != null)
        {
            infoImage.SetActive(true);
        }
        Time.timeScale = 0f; // 게임 정지
        hasInteracted = true; // 한 번만 실행되도록 설정
    }

    void Update()
    {
        // 스페이스바를 누르면 이미지 비활성화 및 게임 재개
        if (infoImage != null && infoImage.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            infoImage.SetActive(false);
            Time.timeScale = 1f; // 게임 재개
        }
    }
}
