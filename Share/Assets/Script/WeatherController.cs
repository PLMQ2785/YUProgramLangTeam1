using System.Collections.Generic;
using UnityEngine;


public class WeatherController : MonoBehaviour
{
    [Header("���� ������Ʈ")]
    [SerializeField] private Light DirectionalLight; // �¾�
    [SerializeField] private Light moonLight; // �� (���û���)
    [SerializeField] private GameTime gameTime; // GameTime ��ũ��Ʈ ����
    [SerializeField] private Weather currentWeather; // Weather ��ũ��Ʈ ����
    private List<Light> SpotLights = new List<Light>();

    [Header("���� ������")]
    [SerializeField] private LightPreset DayNightPreset;
    [SerializeField] private LightPreset LampPreset; // �ΰ� ����� (���û���)

    [Header("����")]
    //[SerializeField, Range(0, 1440), Header("Modifiers"), Tooltip("���� �ð�")] private float TimeOfDay;
    [SerializeField, Tooltip("�¾� ����� Y�� ȸ����")] private float SunDirection = 80.5f;                  //���� 80.5, �ܿ� 34.5, ��/���� 57.5 (�ٻ�ġ)
    //[SerializeField, Tooltip("�ð� ���")] private float TimeMultiplier = 1;
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
        if (gameTime == null) // GM���� Initialize ȣ�� ������ ���..
        {
            Init();
        }
    }

    private void CollectSceneLights()
    {
        if (ControlLights)
        {
            //������ �����ߴ� �ڵ�, Deprecated�Ǿ����Ƿ� ��ü
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

    /// ������ ������ ����ȵ�
    /// �� ������ ����, ���� �ð��� �ð���ӿ� ���� ����� (24 x 60 = 1440)
    /// UpdateLighting�� ���޵� �ð� ������ ����, ���� ���� ������ ���ð� ���� ������ �����¿� �°� ������.

    void Update()
    {
        if (DayNightPreset == null || gameTime == null || currentWeather == null)
            return;

        //TimeOfDay = TimeOfDay + (Time.deltaTime * TimeMultiplier);
        //TimeOfDay = TimeOfDay % 1440;
        //UpdateLighting(gameTime.GetLightUpdateTime());
        UpdateLighting(gameTime.TimeOfDayNormalized);

        // UI ������Ʈ ���� (UIManager�� ã�� ���� ������Ʈ)
        //UIManager uiManager = FindObjectOfType<UIManager>();
        UIManager uiManager = FindFirstObjectByType<UIManager>();
    }

    /// �ð� ������ ����, ���� ���� ������ ���ð� ���� ������ �����¿� �°� ������.
    /// �߰���, ���� �ð��� ���� ���⼺ ����(�¾�)�� ȸ����Ŵ
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = DayNightPreset.AmbientColour.Evaluate(timePercent);
        RenderSettings.fogColor = DayNightPreset.FogColour.Evaluate(timePercent);
        // �ð��� ���� �Ȱ� Ȱ��ȭ/��Ȱ��ȭ �� �е� ���� ���� �߰� ����

        //�ð� ������ ���� ���⼺ ����(�¾�)�� ȸ����Ŵ
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
                    // �ð��� ���� �ΰ� ���� ����/���� ����
                    lamp.color = LampPreset.DirectionalColour.Evaluate(timePercent);
                }
            }
        }

        //�� ���� ������ Ȯ���ϰ�, Ȱ��ȭ�Ǿ� �ִ��� Ȯ���� �� ������ ����
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

    //���� 80.5, �ܿ� 34.5, ��/���� 57.5 (�ٻ�ġ)
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
