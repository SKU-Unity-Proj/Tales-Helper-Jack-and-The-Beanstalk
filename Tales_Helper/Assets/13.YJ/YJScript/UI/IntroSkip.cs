using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSkip : MonoBehaviour
{
    public Image skipUI;
    private float keyPressTime = 0;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(ShowSkipKey());
        }

        if (Input.GetKey(KeyCode.F))
        {
            keyPressTime += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            keyPressTime = 0;
        }

        if (keyPressTime > 1f)
        {
            SceneManager.LoadScene("JackHouse");
        }
        skipUI.fillAmount = keyPressTime;
    }

    IEnumerator ShowSkipKey()
    {
        skipUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        skipUI.gameObject.SetActive(false);
    }
}
