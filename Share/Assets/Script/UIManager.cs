using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slopeAngleTextUI;
    public TextMeshProUGUI tempTextUI;
    public TextMeshProUGUI weatherTextUI;
    public Slider durabilitySliderUI;

    private const string SLOPE_ANGLE_PREFIX = "Slope : ";
    private const string TEMP_PREFIX = "Temp : ";
    private const string WEATHER_PREFIX = "Weather : ";
    private const float DURABILITY_DEFAULT_VALUE = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        slopeAngleTextUI.text = SLOPE_ANGLE_PREFIX + 0f;
        tempTextUI.text = TEMP_PREFIX + 0f;
        weatherTextUI.text = WEATHER_PREFIX + "Clear";
        durabilitySliderUI.value = DURABILITY_DEFAULT_VALUE;
    }

    public string SlopeAngleText
    {
        set { slopeAngleTextUI.text = SLOPE_ANGLE_PREFIX + value; }
    }

    public string TempText
    {
        set { tempTextUI.text = TEMP_PREFIX + value; }
    }

    public string WeatherText
    {
        set { weatherTextUI.text = WEATHER_PREFIX + value; }
    }

    public void UpdateDurabilitySlider(float currentDurability, float maxDurability)
    {
        if (durabilitySliderUI && maxDurability > 0)
        {
            durabilitySliderUI.value = Mathf.Clamp01(currentDurability / maxDurability);
        }
        else if (durabilitySliderUI)
        {
            durabilitySliderUI.value = 0;
        }
    }
}
