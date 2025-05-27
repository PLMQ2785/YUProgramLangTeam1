using System.Collections.Generic;
using UnityEngine;


public class WeatherController : MonoBehaviour
{
    [Header("관리 오브젝트")]
    [SerializeField] private Light DirectionalLight; // 태양
    [SerializeField] private Light moonLight; // 달 (선택사항)
    [SerializeField] private GameTime gameTime; // GameTime 스크립트 참조
    [SerializeField] private Weather currentWeather; // Weather 스크립트 참조
    private List<Light> SpotLights = new List<Light>(); //Scene에 배치된 조명 목록

    [Header("광원 프리셋")]
    [SerializeField] private LightPreset DayNightPreset;
    [SerializeField] private LightPreset LampPreset;

    [Header("설정")]
    //[SerializeField, Range(0, 1440), Header("Modifiers"), Tooltip("현재 시간")] private float TimeOfDay;
    [SerializeField, Tooltip("태양 경로의 Y축 회전값")] private float SunDirection = 80.5f;                  //여름 80.5, 겨울 34.5, 봄/가을 57.5 (근사치)
    //[SerializeField, Tooltip("시간 배속")] private float TimeMultiplier = 1;
    [SerializeField] private bool ControlLights = true;


    //private const float inverseDayLength = 1f / 1440f;

    public void Init()
    {
        Debug.Log("WeatherController Initialized.");
        if (gameTime == null) Debug.LogError("GameTime not assigned to WeatherController.");
        if (currentWeather == null) Debug.LogError("Weather script not assigned to WeatherController.");
        if (DirectionalLight == null) Debug.LogError("Directional Light (Sun) not assigned.");
        if (DayNightPreset == null) Debug.LogError("DayNightCyclePreset not assigned.");

        CollectSceneLights();
    }

    void Start()
    {
        if (gameTime == null) // GM에서 Initialize 호출 안했을 경우..
        {
            Init();
        }
    }

    private void CollectSceneLights()
    {
        if (ControlLights)
        {
            //이전에 구현했던 코드, Deprecated되었으므로 대체
            //Light[] lights = FindObjectsOfType<Light>();
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);

            foreach (Light li in lights)
            {
                switch (li.type)
                {
                    case LightType.Disc:
                    case LightType.Point:
                    case LightType.Rectangle:
                    case LightType.Spot:
                        SpotLights.Add(li);
                        break;
                    case LightType.Directional:
                    default:
                        break;
                }
            }
        }
    }

    // 프리셋 없으면 실행안됨
    // 매 프레임 마다, 게임 시간과 시간배속에 따라 계산함 (24 x 60 = 1440)
    // UpdateLighting에 전달된 시간 비율에 따라, 현재 씬의 렌더링 세팅과 조명 색상을 프리셋에 맞게 설정함.

    void Update()
    {

        //TimeOfDay = TimeOfDay + (Time.deltaTime * TimeMultiplier);
        //TimeOfDay = TimeOfDay % 1440;
        //UpdateLighting(gameTime.GetLightUpdateTime());
        UpdateLighting(gameTime.TimeOfDayNormalized);

        // UI 업데이트 예시 (UIManager를 찾아 직접 업데이트)
        //UIManager uiManager = FindObjectOfType<UIManager>();
        UIManager uiManager = FindFirstObjectByType<UIManager>();
    }

    // 시간 비율에 따라, 현재 씬의 렌더링 세팅과 조명 색상을 프리셋에 맞게 설정함.
    // 추가로, 현재 시간에 따라 방향성 조명(태양)을 회전시킴
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = DayNightPreset.AmbientColour.Evaluate(timePercent);
        RenderSettings.fogColor = DayNightPreset.FogColour.Evaluate(timePercent);
        // 시간에 따라 안개 활성화/비활성화 및 밀도 조절 로직 추가 가능

        //시간 비율에 따라 방향성 조명(태양)을 회전시킴, 없으면 색이 적용이 안되거나 오류가 나서 null check..
        if (DirectionalLight != null)
        {
            if (DirectionalLight.enabled == true)
            {
                DirectionalLight.color = DayNightPreset.DirectionalColour.Evaluate(timePercent);
                DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, SunDirection, 0));
            }
        }

        if (LampPreset != null && ControlLights)
        {
            foreach (Light lamp in SpotLights)
            {
                if (lamp != null && lamp.isActiveAndEnabled && lamp.shadows != LightShadows.None)
                {
                    // 시간에 따라 인공 조명 색상/강도 조절
                    lamp.color = LampPreset.DirectionalColour.Evaluate(timePercent);
                }
            }
        }

        //각 스팟 조명을 확인하고, 활성화되어 있는지 확인한 후 색상을 설정
        foreach (Light lamp in SpotLights)
        {
            if (lamp != null)
            {
                if (lamp.isActiveAndEnabled && lamp.shadows != LightShadows.None && LampPreset != null)
                {
                    lamp.color = LampPreset.DirectionalColour.Evaluate(timePercent);
                }
            }
        }

    }

    //여름 80.5, 겨울 34.5, 봄/가을 57.5 (근사치)
    public void SetSunAzimuthForSeason(string season)
    {
        switch (season.ToLower())
        {
            case "spring": case "autumn": SunDirection = 57.5f; break;
            case "summer": SunDirection = 80.5f; break;
            case "winter": SunDirection = 34.5f; break;
            default: SunDirection = 80.5f; break;
        }
    }
}
