using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRegister : MonoBehaviour
{
    // 오브젝트가 활성화될 때 호출되는 메소드
    private void OnEnable()
    {
        // 이 게임 오브젝트에 있는 CinemachineVirtualCamera 컴포넌트를 CameraSwitcher에 등록
        CameraSwitcher.Register(GetComponent<CinemachineVirtualCamera>());
    }

    // 오브젝트가 비활성화될 때 호출되는 메소드
    private void OnDisable()
    {
        // 이 게임 오브젝트에 있는 CinemachineVirtualCamera 컴포넌트를 CameraSwitcher에서 제거
        CameraSwitcher.UnRegister(GetComponent<CinemachineVirtualCamera>());
    }
}
