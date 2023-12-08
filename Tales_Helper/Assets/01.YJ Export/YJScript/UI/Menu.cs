using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel;
    //public GameObject player;
    //public GameObject questManager;
    private bool isPause = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause == false)
            {
                isPause = true;
                menuPanel.SetActive(true);
                Time.timeScale = 0f;
            }

            else if (isPause == true)
            {
                isPause = false;
                menuPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public void ResumeButton()
    {
        isPause = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SaveButton()
    {
        /*
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
        PlayerPrefs.SetFloat("QuestId", questManager.questId);
        PlayerPrefs.SetFloat("QuestActionIndex", questManager.questIActionIndex);
        PlayerPrefs.Save();

        menuPanel.SetActive(false);
        
    }

    public void LoadButton()
    {
        
        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");

        player.transform.poistion = new Vector3(x, y, z);
        questManager.questId = questId;
        questManager.questActionIndex = questActionIndex;
        */
    }

    public void MenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
