using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private string currentCondition = "Clear";
    [SerializeField] private float temperatureCelcius = 25f;
    [SerializeField, Range(0f, 100f)] private float humidity = 50f;

    public string CurrentCondition => currentCondition;
    public float TemperatureCelcius => temperatureCelcius;
    public float Humidity => humidity;

    public string GetCurrentCondition() => currentCondition;

    public void SetCurrentCondition(string newCondition)
    {
        currentCondition = newCondition;
        Debug.Log($"Weather condition changed to: {newCondition}");
        // ���� ���濡 ���� �߰� ���� ������ ���⿡
    }

    public void SetTemperature(float temp)
    {
        temperatureCelcius = temp;
    }

    public void SetHumidity(float value)
    {
        humidity = Mathf.Clamp(value, 0f, 100f);
    }
}
