using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[RequireComponent(typeof(HAwarenessSystem))]
public class HearingEnemyAI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FeedbackDisplay;

    [SerializeField] float _VisionConeAngle = 60f;
    [SerializeField] float _VisionConeRange = 30f;
    [SerializeField] Color _VisionConeColour = new Color(1f, 0f, 0f, 0.25f);

    [SerializeField] float _HearingRange = 20f;
    [SerializeField] Color _HearingRangeColour = new Color(1f, 1f, 0f, 0.25f);

    [SerializeField] float _ProximityDetectionRange = 3f;
    [SerializeField] Color _ProximityRangeColour = new Color(1f, 1f, 1f, 0.25f);

    public Vector3 EyeLocation => transform.position;
    public Vector3 EyeDirection => transform.forward;

    public float VisionConeAngle => _VisionConeAngle;
    public float VisionConeRange => _VisionConeRange;
    public Color VisionConeColour => _VisionConeColour;

    public float HearingRange => _HearingRange;
    public Color HearingRangeColour => _HearingRangeColour;

    public float ProximityDetectionRange => _ProximityDetectionRange;
    public Color ProximityDetectionColour => _ProximityRangeColour;

    public float CosVisionConeAngle { get; private set; } = 0f;

    HAwarenessSystem Awareness;

    void Awake()
    {
        CosVisionConeAngle = Mathf.Cos(VisionConeAngle * Mathf.Deg2Rad);
        Awareness = GetComponent<HAwarenessSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        Awareness.ReportCanHear(source, location, category, intensity);
    }

    public void ReportInProximity(DetectableTarget target)
    {
        Awareness.ReportInProximity(target);
    }

    public void OnSuspicious()
    {
        FeedbackDisplay.text = "I hear you";
    }

    public void OnDetected(GameObject target)
    {
        FeedbackDisplay.text = "I see you " + target.gameObject.name;
    }

    public void OnFullyDetected(GameObject target)
    {
        FeedbackDisplay.text = "Charge! " + target.gameObject.name;
    }

    public void OnLostDetect(GameObject target)
    {
        FeedbackDisplay.text = "Where are you " + target.gameObject.name;
    }

    public void OnLostSuspicion()
    {
        FeedbackDisplay.text = "Where did you go";
    }

    public void OnFullyLost()
    {
        FeedbackDisplay.text = "Must be nothing";
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HearingEnemyAI))]
public class HearingEnemyAIEditor : Editor
{
    public void OnSceneGUI()
    {
        var ai = target as HearingEnemyAI;

        // draw the detectopm range
        Handles.color = ai.ProximityDetectionColour;
        Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.ProximityDetectionRange);

        // draw the hearing range
        Handles.color = ai.HearingRangeColour;
        Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.HearingRange);

    }
}
#endif // UNITY_EDITOR