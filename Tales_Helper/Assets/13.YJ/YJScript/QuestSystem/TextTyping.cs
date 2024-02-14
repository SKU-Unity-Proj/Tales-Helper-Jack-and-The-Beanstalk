using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTyping : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] TMP_Text tmpProText;
    string writer;
    [SerializeField] private Coroutine coroutine;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;
    [Space(10)][SerializeField] private bool startOnEnable = false;

    [Header("Collision-Based")]
    [SerializeField] private bool clearAtStart = false;
    [SerializeField] private bool startOnCollision = false;
    enum options { clear, complete }
    [SerializeField] options collisionExitOptions;

    // Use this for initialization
    void Awake()
    {
        
    }

    void Start()
    {
        if (!clearAtStart) return;
        if (text != null)
        {
            text.text = "";
        }

        if (tmpProText != null)
        {
            tmpProText.text = "";
        }
    }

    private void OnEnable()
    {
        if (text != null)
        {
            writer = text.text;
        }

        if (tmpProText != null)
        {
            writer = tmpProText.text;
        }

        print("On Enable!");
        if (startOnEnable) StartTypewriter();
    }

    private void OnCollisionEnter3D(Collision col)
    {
        print("Collision!");
        if (startOnCollision)
        {
            StartTypewriter();
        }
    }

    private void OnCollisionExit3D(Collision other)
    {
        if (collisionExitOptions == options.complete)
        {
            if (text != null)
            {
                text.text = writer;
            }

            if (tmpProText != null)
            {
                tmpProText.text = writer;
            }
        }
        // clear
        else
        {
            if (text != null)
            {
                text.text = "";
            }

            if (tmpProText != null)
            {
                tmpProText.text = "";
            }
        }

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }


    private void StartTypewriter()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        if (text != null)
        {
            text.text = "";

            coroutine = StartCoroutine(TypeWriterText());
        }

        if (tmpProText != null)
        {
            tmpProText.text = "";

            coroutine = StartCoroutine(TypeWriterText());
        }
    }

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    IEnumerator TypeWriterText()
    {
        text.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (text.text.Length > 0)
            {
                text.text = text.text.Substring(0, text.text.Length - leadingChar.Length);
            }
            text.text += c;
            text.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            text.text = text.text.Substring(0, text.text.Length - leadingChar.Length);
        }

        yield return null;
    }

    public void onclickskip()
    {

    }


    IEnumerator TypeWriterTMP()
    {
        tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (tmpProText.text.Length > 0)
            {
                tmpProText.text = tmpProText.text.Substring(0, tmpProText.text.Length - leadingChar.Length);
            }
            tmpProText.text += c;
            tmpProText.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            tmpProText.text = tmpProText.text.Substring(0, tmpProText.text.Length - leadingChar.Length);
        }
    }
}
