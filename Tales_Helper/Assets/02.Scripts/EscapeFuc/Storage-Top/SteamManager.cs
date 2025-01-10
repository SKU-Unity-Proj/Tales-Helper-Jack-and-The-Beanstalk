using System.Collections.Generic;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    public List<GameObject> steamList; // ���� ������Ʈ ����Ʈ
    public List<ValveController> valves; // ��� ���� ����Ʈ
    public List<WindowController> windows; // ��� â�� ����Ʈ

    private void Start()
    {
        if (valves.Count != windows.Count || windows.Count != steamList.Count)
        {
            Debug.LogError("â��, ����, ������ ������ ��ġ���� �ʽ��ϴ�!");
            return;
        }

        // �� ����� â�� ����
        for (int i = 0; i < valves.Count; i++)
        {
            valves[i].linkedWindow = windows[i];
        }
    }

    private void Update()
    {
        // ��� ���� ���¸� ������Ʈ
        for (int i = 0; i < steamList.Count; i++)
        {
            UpdateSteamState(i);
        }
    }

    private void UpdateSteamState(int index)
    {
        // Ư�� �ε����� ���갡 ���ȴ��� Ȯ��
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
            Debug.Log($"���� {index + 1}�� �������ϴ�.");
        }
    }

    private void EnableSteam(int index)
    {
        if (!steamList[index].activeSelf)
        {
            steamList[index].SetActive(true);
            Debug.Log($"���� {index + 1}�� �ٽ� �������ϴ�.");
        }
    }
}
