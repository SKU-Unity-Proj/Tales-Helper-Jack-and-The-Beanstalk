using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SettingsUIElement_Boolean 클래스는 불리언 설정 UI 요소를 나타낸다.
public class SettingsUIElement_Boolean : BaseSettingsUIElement
{
#pragma warning disable 0649
    [SerializeField] Toggle ToggleBox; // 유니티 인스펙터에서 설정 가능한 토글 UI 요소를 참조한다.
    [SerializeField] bool DefaultValue; // 기본값을 저장한다.
#pragma warning restore 0649

    private bool InitialValue = false; // 초기값을 저장한다.

    // UNITY_EDITOR 조건부 컴파일 블록은 유니티 에디터 환경에서만 동작한다.
#if UNITY_EDITOR
    // BindToSetting 메소드는 설정을 UI 요소에 바인딩한다.
    public override void BindToSetting(int settingUIDIndex, ConfigurableSetting settingAttribute)
    {
        base.BindToSetting(settingUIDIndex, settingAttribute);

        BooleanSetting typeSpecificConfig = settingAttribute as BooleanSetting;
        DefaultValue = typeSpecificConfig.DefaultValue;
        ToggleBox.SetIsOnWithoutNotify(typeSpecificConfig.DefaultValue);
    }
#endif // UNITY_EDITOR

    // PopulateInitialValue 메소드는 초기 값을 설정한다.
    public override void PopulateInitialValue()
    {
        InitialValue = SettingsBinder.GetBoolean(UniqueID, SettingsManager.Settings);
    }

    // SynchroniseUIWithSettings 메소드는 UI를 현재 설정과 동기화한다.
    public override void SynchroniseUIWithSettings()
    {
        ToggleBox.SetIsOnWithoutNotify(SettingsBinder.GetBoolean(UniqueID, SettingsManager.Settings));
    }

    // ResetToOriginalValue 메소드는 설정을 초기 값으로 리셋한다.
    public override void ResetToOriginalValue()
    {
        SettingsBinder.SetBoolean(UniqueID, InitialValue, SettingsManager.Settings);
        ToggleBox.SetIsOnWithoutNotify(InitialValue);
    }

    // OnValueChanged 메소드는 토글의 값이 변경될 때 호출된다.
    public void OnValueChanged(bool newValue)
    {
        SettingsBinder.SetBoolean(UniqueID, newValue, SettingsManager.Settings);
    }
}
