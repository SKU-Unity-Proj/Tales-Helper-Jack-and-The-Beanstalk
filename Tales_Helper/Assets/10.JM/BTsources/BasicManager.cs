using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicManager : MonoBehaviour
{
    public static BasicManager Instance { get; private set; }
    public DetectableTarget PlayerTarget { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    public void RegisterPlayer(DetectableTarget playerTarget)
    {
        PlayerTarget = playerTarget;
    }
}
