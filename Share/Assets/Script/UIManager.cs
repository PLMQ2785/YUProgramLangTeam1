using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slopeAngleTextUI;
    public TextMeshProUGUI tempTextUI;
    public TextMeshProUGUI weatherTextUI;
    public Slider durabilitySliderUI;
    public TextMeshProUGUI fatigueScoreTextUI;

    [Header("Environment")]
    public TMP_InputField temperatureInput;
    public TMP_InputField humidityInput;
    public TMP_Dropdown weatherDropdown;
    public TMP_Dropdown sunPositionDropdown;   //계절별, 여름 80.5, 겨울 34.5, 봄/가을 57.5

    [Header("Character")]
    public TMP_Dropdown soleTypeDropdown;
    public TMP_InputField heightInput;
    public TMP_InputField weightInput;
    public TMP_InputField ageInput;
    public TMP_Dropdown genderDropdown;

    [Header("Fatigue Coef")]
    public TMP_InputField kSlopeInput;
    public TMP_InputField kTempInput;
    public TMP_InputField kHumidInput;

    [Header("Fatigue Weights")]
    public TMP_InputField wTempInput;
    public TMP_InputField wHumidInput;
    public TMP_InputField wSlopeInput;

    [Header("Time")]
    public Slider timeSlider;
    public TextMeshProUGUI timeDisplayText;
    public Slider multiplierSlider;
    public TextMeshProUGUI multiplierDisplayText;

    [Header("Target Components")] // 인스펙터/GameManager에서 할당
    public Weather weather;
    public WeatherController weatherController;
    public Character character;
    public GameTime gameTime;


    private const string SLOPE_ANGLE_PREFIX = "Slope : ";
    private const string TEMP_PREFIX = "Temp : ";
    private const string WEATHER_PREFIX = "Weather : ";
    private const string FATIGUE_PREFIX = "Fatigue Score : ";
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
        //slopeAngleTextUI.text = SLOPE_ANGLE_PREFIX + 0f;
        //tempTextUI.text = TEMP_PREFIX + 0f;
        //weatherTextUI.text = WEATHER_PREFIX + "Clear";
        //durabilitySliderUI.value = DURABILITY_DEFAULT_VALUE;

        weather = GameManager.Instance.GetWeather();
        weatherController = GameManager.Instance.GetWeatherController();
        character = GameManager.Instance.GetCharacter();
        gameTime = GameManager.Instance.GetGameTime();

        SetupControlPanel();
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

    public string FatigueScoreText
    {
        set { fatigueScoreTextUI.text = FATIGUE_PREFIX + value; }
    }


    //public void UpdateDurabilitySlider(float currentDurability, float maxDurability)
    //{
    //    if (durabilitySliderUI && maxDurability > 0)
    //    {
    //        durabilitySliderUI.value = Mathf.Clamp01(currentDurability / maxDurability);
    //    }
    //    else if (durabilitySliderUI)
    //    {
    //        durabilitySliderUI.value = 0;
    //    }

    //}

    private void SetupControlPanel()
    {
        soleTypeDropdown.ClearOptions();
        soleTypeDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(FootWear.SoleType))));
        //if (character.EquippedFootWear != null) soleTypeDropdown.value = (int)character.EquippedFootWear.soleType;

        weatherDropdown.ClearOptions();
        List<string> weatherOptions = new List<string> { "Clear", "Cloudy", "Rainy", "Snowy" };
        weatherDropdown.AddOptions(weatherOptions);
        //int currentWIndex = weatherOptions.FindIndex(w => w == weather.CurrentCondition);
        //weatherDropdown.value = currentWIndex >= 0 ? currentWIndex : 0;
        //WeatherText = weather.CurrentCondition;

        sunPositionDropdown.ClearOptions();
        sunPositionDropdown.AddOptions(new List<string> { "Spring", "Summer", "Autumn", "Winter" });

        genderDropdown.ClearOptions();
        genderDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(Gender))));
        genderDropdown.value = (int)character.CharacterGender;

        temperatureInput.text = weather.TemperatureCelcius.ToString("F1");
        TempText = weather.TemperatureCelcius.ToString("F1");
        humidityInput.text = weather.Humidity.ToString("F1");

        heightInput.text = character.Height.ToString("F0");
        weightInput.text = character.Weight.ToString("F1");
        ageInput.text = character.Age.ToString();

        FatigueCalculator fatigueCalc = character.GetFatigueCalculator();
        if (fatigueCalc != null)
        {
            kSlopeInput.text = fatigueCalc.k_slope.ToString("F3");
            kTempInput.text = fatigueCalc.k_temp.ToString("F4");
            kHumidInput.text = fatigueCalc.k_humid.ToString("F4");
            wTempInput.text = fatigueCalc.w3_Temp.ToString("F2");
            wHumidInput.text = fatigueCalc.w4_Humid.ToString("F2");
            wSlopeInput.text = fatigueCalc.w5_Slope.ToString("F2");
        }

        if (timeSlider != null) { timeSlider.minValue = 0; timeSlider.maxValue = 1440; timeSlider.value = gameTime.CurrentTimeOfDayMinutes; UpdateTimeDisplay(timeSlider.value); }
        if (multiplierSlider != null) { multiplierSlider.minValue = 0; multiplierSlider.maxValue = 1000; multiplierSlider.value = gameTime.TimeMultiplier; UpdateMultiplierDisplay(multiplierSlider.value); }

        AddAllListeners();
    }

    private void AddAllListeners()
    {
        temperatureInput.onEndEdit.AddListener(OnTemperatureChanged);
        humidityInput.onEndEdit.AddListener(OnHumidityChanged);
        soleTypeDropdown.onValueChanged.AddListener(OnSoleTypeChanged);
        weatherDropdown.onValueChanged.AddListener(OnWeatherChanged);
        sunPositionDropdown.onValueChanged.AddListener(OnSunPositionChanged);
        heightInput.onEndEdit.AddListener(OnHeightChanged);
        weightInput.onEndEdit.AddListener(OnWeightChanged);
        ageInput.onEndEdit.AddListener(OnAgeChanged);
        genderDropdown.onValueChanged.AddListener(OnGenderChanged);
        kSlopeInput.onEndEdit.AddListener(OnKSlopeChanged);
        kTempInput.onEndEdit.AddListener(OnKTempChanged);
        kHumidInput.onEndEdit.AddListener(OnKHumidChanged);
        wTempInput.onEndEdit.AddListener(OnWTempChanged);
        wHumidInput.onEndEdit.AddListener(OnWHumidChanged);
        wSlopeInput.onEndEdit.AddListener(OnWSlopeChanged);
        timeSlider.onValueChanged.AddListener(OnTimeChanged);
        multiplierSlider.onValueChanged.AddListener(OnMultiplierChanged);
    }

    //public string SlopeAngleText { set { if (slopeAngleTextUI) slopeAngleTextUI.text = SLOPE_ANGLE_PREFIX + value; } }
    //public string TempText { set { if (tempTextUI) tempTextUI.text = TEMP_PREFIX + value; } }
    //public string WeatherText { set { if (weatherTextUI) weatherTextUI.text = WEATHER_PREFIX + value; } }
    public void UpdateDurabilitySlider(float currentDurability, float maxDurability) { if (durabilitySliderUI) { durabilitySliderUI.value = (maxDurability > 0) ? Mathf.Clamp01(currentDurability / maxDurability) : 0; } }
    public void UpdateTimeDisplay(float minutes) { if (timeDisplayText != null) { TimeSpan time = TimeSpan.FromMinutes(minutes); timeDisplayText.text = $"Time: {time.Hours:D2}:{time.Minutes:D2}"; } }
    private void UpdateMultiplierDisplay(float multiplier) { if (multiplierDisplayText != null) { multiplierDisplayText.text = $"Multiplier: {multiplier:F1}x"; } }

    void OnTemperatureChanged(string value) { if (float.TryParse(value, out float v)) { weather.SetTemperature(v); TempText = v.ToString("F1") + "°C"; } else { temperatureInput.text = weather.TemperatureCelcius.ToString("F1"); } }
    void OnHumidityChanged(string value) { if (float.TryParse(value, out float v)) weather.SetHumidity(v); else { humidityInput.text = weather.Humidity.ToString("F1"); } }
    void OnSoleTypeChanged(int index) { character.ChangeFootwearType((FootWear.SoleType)index); }
    void OnWeatherChanged(int index) { string w = weatherDropdown.options[index].text; weather.SetCurrentCondition(w); WeatherText = w; }
    void OnSunPositionChanged(int index) { weatherController.SetSunAzimuthForSeason(sunPositionDropdown.options[index].text); }
    void OnHeightChanged(string value) { if (float.TryParse(value, out float v)) character.SetHeight(v); else { heightInput.text = character.Height.ToString("F0"); } }
    void OnWeightChanged(string value) { if (float.TryParse(value, out float v)) character.SetWeight(v); else { weightInput.text = character.Weight.ToString("F1"); } }
    void OnAgeChanged(string value) { if (int.TryParse(value, out int v)) character.SetAge(v); else { ageInput.text = character.Age.ToString(); } }
    void OnGenderChanged(int index) { character.SetGender((Gender)index); }
    void OnKSlopeChanged(string value) { if (float.TryParse(value, out float v)) character.GetFatigueCalculator().k_slope = v; else { kSlopeInput.text = character.GetFatigueCalculator().k_slope.ToString("F3"); } }
    void OnKTempChanged(string value) { if (float.TryParse(value, out float v)) character.GetFatigueCalculator().k_temp = v; else { kTempInput.text = character.GetFatigueCalculator().k_temp.ToString("F4"); } }
    void OnKHumidChanged(string value) { if (float.TryParse(value, out float v)) character.GetFatigueCalculator().k_humid = v; else { kHumidInput.text = character.GetFatigueCalculator().k_humid.ToString("F4"); } }
    void OnWTempChanged(string value) { if (float.TryParse(value, out float v)) character.GetFatigueCalculator().w3_Temp = v; else { wTempInput.text = character.GetFatigueCalculator().w3_Temp.ToString("F2"); } }
    void OnWHumidChanged(string value) { if (float.TryParse(value, out float v)) character.GetFatigueCalculator().w4_Humid = v; else { wHumidInput.text = character.GetFatigueCalculator().w4_Humid.ToString("F2"); } }
    void OnWSlopeChanged(string value) { if (float.TryParse(value, out float v)) character.GetFatigueCalculator().w5_Slope = v; else { wSlopeInput.text = character.GetFatigueCalculator().w5_Slope.ToString("F2"); } }
    void OnTimeChanged(float value) { gameTime.SetTimeOfDay(value); UpdateTimeDisplay(value); }
    void OnMultiplierChanged(float value) { gameTime.SetTimeMultiplier(value); UpdateMultiplierDisplay(value); }

}
