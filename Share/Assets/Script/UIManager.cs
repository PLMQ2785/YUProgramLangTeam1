using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI slopeAngle;
    public TextMeshProUGUI temp;
    public TextMeshProUGUI weather;
    public Slider durabilitySlider;

    private const string slopeAngleText = "Slope : ";
    private const string tempText = "Temp : ";
    private const string weatherText = "Weather : ";
    private const float durabilityDefault = 1f;

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
        slopeAngle.text = slopeAngleText + 0f;
        temp.text = tempText + 0f;
        weather.text = weatherText + "Clear";
        durabilitySlider.value = durabilityDefault;
    }
}
