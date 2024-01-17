using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Playables;

/*
public class CheckInteration : MonoBehaviour
{
    public Text beanText;
    public GameObject beanStalk;
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera beanCam;
    public LayerMask layerMask;
    public float range;

    private bool interationActivated = false;
    private RaycastHit hitInfo;
    public PlayableDirector playableDirector;

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        CheckUI();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckUI();
            CanInteration();
        }
    }

    private void CheckUI()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            UIAppear();
        }
        else
            UIDisappear();
    }

    private void UIAppear()
    {
        interationActivated = true;
        beanText.gameObject.SetActive(true);
        beanText.text = " 콩 심기 " + "<color=yellow>" + "(F)" + "</color>";
    }

    private void UIDisappear()
    {
        interationActivated = false;
        beanText.gameObject.SetActive(false);
    }

    private void CanInteration()
    {
        if (interationActivated)
        {
            if (hitInfo.transform != null)
                UIDisappear();

            if (hitInfo.transform.name == "BeanSpot")
                StartCoroutine(GrowBean());
        }
    }

    IEnumerator GrowBean()
    {
        //타임라인 실행
        //playableDirector.Play();
        //yield return new WaitForSeconds(8f);

        //콩나무 자라기
        //beanStalk.SetActive(true);
        //beanCam.MoveToTopOfPrioritySubqueue();
        //beanCam.Priority = 11;
        //mainCam.Priority = 10;
        
        //yield return new WaitForSeconds(9f);
        //SceneManager.LoadScene("GiantHouse");

        yield break;
    }
}
*/