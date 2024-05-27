using UnityEngine;

public class EpisodeControll : MonoBehaviour
{

    void Start()
    {
        LoadPlayerState(); // �÷��̾� ���� �ε�
    }

    void Update()
    {
        // �÷��̾� ������Ʈ ����
    }

    public void SavePlayerState()
    {
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
        PlayerPrefs.Save();
    }

    public void LoadPlayerState()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            transform.position = new Vector3(x, y, z);
        }
    }

    void OnDestroy()
    {
        SavePlayerState(); // �÷��̾� ���� ����
    }
}
