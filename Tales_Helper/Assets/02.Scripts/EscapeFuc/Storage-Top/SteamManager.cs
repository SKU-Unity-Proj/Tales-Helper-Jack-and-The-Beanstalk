using System.Collections.Generic;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    public List<GameObject> steamList; // 스팀 오브젝트 리스트
    public List<ValveController> valves; // 모든 벨브 리스트
    public List<WindowController> windows; // 모든 창문 리스트

    private void Start()
    {
        if (valves.Count != windows.Count || windows.Count != steamList.Count)
        {
            Debug.LogError("창문, 벨브, 스팀의 개수가 일치하지 않습니다!");
            return;
        }

        // 각 벨브와 창문 연결
        for (int i = 0; i < valves.Count; i++)
        {
            valves[i].linkedWindow = windows[i];
        }
    }

    private void Update()
    {
        // 모든 스팀 상태를 업데이트
        for (int i = 0; i < steamList.Count; i++)
        {
            UpdateSteamState(i);
        }
    }

    private void UpdateSteamState(int index)
    {
        // 특정 인덱스의 벨브가 열렸는지 확인
        if (valves[index].isUnlocked)
        {
            DisableSteam(index);
        }
        else
        {
            EnableSteam(index);
        }
    }

    private void DisableSteam(int index)
    {
        if (steamList[index].activeSelf)
        {
            steamList[index].SetActive(false);
            Debug.Log($"스팀 {index + 1}이 꺼졌습니다.");
        }
    }

    private void EnableSteam(int index)
    {
        if (!steamList[index].activeSelf)
        {
            steamList[index].SetActive(true);
            Debug.Log($"스팀 {index + 1}이 다시 켜졌습니다.");
        }
    }
}
