using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField, Header("관리 오브젝트")] private Light DirectionalLight = null;
    [SerializeField] private Light MoonLight = null;
    [SerializeField] private LightPreset DayNightPreset, LampPreset;
    private List<Light> SpotLights = new List<Light>();

    [SerializeField, Range(0, 1440), Header("Modifiers"), Tooltip("현재 시간")] private float TimeOfDay;
    [SerializeField, Tooltip("태양 각도")] private float SunDirection = 75.9f; //여름 75.9, 겨울 28.9, 봄/가을 52.4 (근사치)
    [SerializeField, Tooltip("시간 배속")] private float TimeMultiplier = 1;
    [SerializeField] private bool ControlLights = true;

    private const float inverseDayLength = 1f / 1440f;

    /// 프로젝트 시작 -> ControlLights가 true일 경우, 현재 씬의 모든 비방향성 조명을 수집하여 리스트에 저장
    private void Start()
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


    /// 프리셋 없으면 실행안됨
    /// 매 프레임 마다, 게임 시간과 시간배속에 따라 계산함 (24 x 60 = 1440)
    /// UpdateLighting에 전달된 시간 비율에 따라, 현재 씬의 렌더링 세팅과 조명 색상을 프리셋에 맞게 설정함.

    private void Update()
    {
        if (DayNightPreset == null)
            return;

        TimeOfDay = TimeOfDay + (Time.deltaTime * TimeMultiplier);
        TimeOfDay = TimeOfDay % 1440;
        UpdateLighting(TimeOfDay * inverseDayLength);
    }

    /// 시간 비율에 따라, 현재 씬의 렌더링 세팅과 조명 색상을 프리셋에 맞게 설정함.
    /// 추가로, 현재 시간에 따라 방향성 조명(태양)을 회전시킴
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = DayNightPreset.AmbientColour.Evaluate(timePercent);
        RenderSettings.fogColor = DayNightPreset.FogColour.Evaluate(timePercent);

        //시간 비율에 따라 방향성 조명(태양)을 회전시킴
        if (DirectionalLight != null)
        {
            if (DirectionalLight.enabled == true)
            {
                DirectionalLight.color = DayNightPreset.DirectionalColour.Evaluate(timePercent);
                DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, SunDirection, 0));
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
}