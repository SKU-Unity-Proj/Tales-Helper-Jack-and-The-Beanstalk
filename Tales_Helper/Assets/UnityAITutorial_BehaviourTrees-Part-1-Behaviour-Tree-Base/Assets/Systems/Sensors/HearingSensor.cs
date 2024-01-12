using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class HearingSensor : MonoBehaviour
{
    EnemyAI LinkedAI; // 이 스크립트가 부착된 게임 오브젝트에 연결된 EnemyAI 컴포넌트

    void Start()
    {
        LinkedAI = GetComponent<EnemyAI>(); // EnemyAI 컴포넌트를 찾아 연결
        HearingManager.Instance.Register(this); // HearingManager에 이 센서를 등록
    }


    void Update()
    {

    }

    // 게임 오브젝트가 파괴될 때 호출되는 OnDestroy 함수
    void OnDestroy()
    {
        // HearingManager가 존재하면 이 센서의 등록을 해제
        if (HearingManager.Instance != null)
            HearingManager.Instance.Deregister(this);
    }

    // 소리를 들었을 때 호출되는 함수
    public void OnHeardSound(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        // 소리의 위치가 청각 범위 밖에 있으면 리턴
        if (Vector3.Distance(location, LinkedAI.EyeLocation) > LinkedAI.HearingRange)
            return;

        // EnemyAI에 들은 소리를 보고
        LinkedAI.ReportCanHear(source, location, category, intensity);
    }
}
