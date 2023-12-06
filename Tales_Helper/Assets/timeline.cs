using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class timeline : MonoBehaviour
{
    private PlayableDirector pd;
    public TimelineAsset[] ta;
    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "CutScene")
        {
            other.gameObject.SetActive(false);
            pd.Play(ta[0]);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
