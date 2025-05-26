using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    ////private float timeOfDayMinutes = 0; // 시작시간
    //[SerializeField, Range(0, 1440), Header("Time Settings"), Tooltip("현재 시간 (분 단위, 0-1439)")]
    //private float TimeOfDay = 0f; //시작시간

    //[SerializeField, Tooltip("시간 배속")]
    //private float TimeMultiplier = 1f;

    //private const float inverseDayLength = 1f / 1440f;

    ////private const float MINUTES_IN_DAY = 1440f;

    ////public float TimeOfDayNormalized => timeOfDayMinutes / MINUTES_IN_DAY; // 0.0 ~ 1.0
    ////public float CurrentTimeOfDayMinutes => timeOfDayMinutes;

    //void Update()
    //{
    //    TimeOfDay = TimeOfDay + (Time.deltaTime * TimeMultiplier);
    //    TimeOfDay = TimeOfDay % 1440;
    //}

    //public float GetTimeofDay() => TimeOfDay;

    //public float GetLightUpdateTime() => (TimeOfDay * inverseDayLength);

    //public void SetTimeOfDay(float minutes)
    //{
    //    //timeOfDayMinutes = Mathf.Clamp(minutes, 0, MINUTES_IN_DAY);
    //}

    //public void SetTimeMultiplier(float multiplier)
    //{
    //    //timeMultiplier = multiplier;
    //}

    //public float GetInverseDayLength() => inverseDayLength;

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
