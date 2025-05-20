using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private string currentCondition = "Clear";
    [SerializeField] private float temperatureCelcius = 25f;

    public string CurrentCondition => currentCondition;
    public float TemperatureCelcius => temperatureCelcius;

    public string GetCurrentCondition()
    {
        return currentCondition;
    }

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
}
