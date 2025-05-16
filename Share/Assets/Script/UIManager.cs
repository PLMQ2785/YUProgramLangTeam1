using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI slopeAngle;
    public TextMeshProUGUI temp;
    public TextMeshProUGUI weather;
    public Slider durabilitySlider;

    private const string _slopeAngleText = "Slope : ";
    private const string _tempText = "Temp : ";
    private const string _weatherText = "Weather : ";
    private const float _durabilityDefault = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiInit();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void uiInit()
    {
        slopeAngle.text = _slopeAngleText + 0f;
        temp.text = _tempText + 0f;
        weather.text = _weatherText + "Clear";
        durabilitySlider.value = _durabilityDefault;
    }

    public string SlopeAngleText
    {
        set { slopeAngle.text = _slopeAngleText + value; }
    }

    public string TempText
    {
        set { temp.text = _tempText + value; }
    }

    public string WeatherText
    {
        set { weather.text = _weatherText + value; }
    }

    public string durabilityText
    {
        set { durabilitySlider.value = float.Parse(value); }
    }
}
