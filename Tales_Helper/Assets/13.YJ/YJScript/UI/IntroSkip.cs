using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroSkip : MonoBehaviour
{
    public Image skipUI;
    public TextMeshProUGUI skipText;
    private float keyPressTime = 0;
    private bool isAction = false;

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
            if (isAction)
                return;

            LoadingSceneController.Instance.LoadScene("JackHouse");

            isAction = true;
        }
        skipUI.fillAmount = keyPressTime;
    }

    IEnumerator ShowSkipKey()
    {
        skipText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        skipText.gameObject.SetActive(false);
    }
}
