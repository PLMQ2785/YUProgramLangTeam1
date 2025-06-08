using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    [SerializeField, Range(0, 1440), Header("Time Settings"), Tooltip("현재 시간 (분 단위, 0-1439)")]
    private float timeOfDayMinutes = 0f;

    [SerializeField, Tooltip("시간 진행 속도 배율")]
    private float timeMultiplier = 1f;

    private UIManager _uiManager;

    private const float MINUTES_IN_DAY = 1440f;

    public float TimeOfDayNormalized => timeOfDayMinutes / MINUTES_IN_DAY;
    public float CurrentTimeOfDayMinutes => timeOfDayMinutes;
    public float TimeMultiplier => timeMultiplier;

    void Start()
    {
        _uiManager = GameManager.Instance.GetUIManager();
    }

    void Update()
    {
        if (timeMultiplier > 0)
        {
            timeOfDayMinutes += UnityEngine.Time.deltaTime * timeMultiplier/60;
            timeOfDayMinutes %= MINUTES_IN_DAY;
            _uiManager.UpdateTimeDisplay(timeOfDayMinutes);
        }
    }

    public DateTime GetCurrentDateTime() => System.DateTime.Today.AddMinutes(timeOfDayMinutes);
    public void SetTimeOfDay(float minutes) => timeOfDayMinutes = Mathf.Clamp(minutes, 0, MINUTES_IN_DAY);
    public void SetTimeMultiplier(float multiplier) => timeMultiplier = Mathf.Max(0, multiplier);
}
