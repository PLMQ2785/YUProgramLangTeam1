using UnityEngine;

public enum Gender { Male, Female }

[System.Serializable] // �ν����Ϳ��� Character ��ũ��Ʈ�� ǥ�õǵ��� ��
public class FatigueCalculator
{
    [Header("Calculation Coefficients (���� �ʿ�)")]
    [Tooltip("��絵 ���")]
    public float k_slope = 0.1f;
    [Tooltip("��� ��� (21�� ����)")]
    public float k_temp = 0.005f;
    [Tooltip("���� ���")]
    public float k_humid = 0.002f;

    [Header("Fatigue Formula Weights (���� �ʿ�)")]
    [Tooltip("��� ����ġ")]
    public float w3_Temp = 1.0f;
    [Tooltip("���� ����ġ")]
    public float w4_Humid = 1.0f;
    [Tooltip("��絵 ����ġ")]
    public float w5_Slope = 1.5f;

    /// BMI (ü���� ����)�� ���.
    /// weightKg ������ (kg)
    /// heightCm Ű (cm)
    /// ->BMI ��
    public float CalculateBMI(float weightKg, float heightCm)
    {
        if (heightCm <= 0) return 0;
        float heightM = heightCm / 100f;
        return weightKg / (heightM * heightM);
    }

    /// BMR (���� ��緮)�� Mifflin-St Jeor �������� ���.
    /// weightKg ������ (kg)
    /// heightCm Ű (cm)
    /// age ����
    /// gender ����
    /// ->BMR ��
    public float CalculateBMR(float weightKg, float heightCm, int age, Gender gender)
    {
        float bmr = (10f * weightKg) + (6.25f * heightCm) - (5f * age);
        return gender == Gender.Male ? bmr + 5f : bmr - 161f;
    }

    /// ��絵�� ���� ������ ����մϴ�.
    /// deltaH ���� �̵� �Ÿ�
    /// deltaD ���� �̵� �Ÿ�
    /// ->��絵 (����)
    public float CalculateSlopeAngle(float deltaH, float deltaD)
    {
        return (deltaD > 0.001f) ? Mathf.Atan(deltaH / deltaD) : 0f;
    }

    /// ���� �Ƿε� ������ ���. (0-100 �����ϸ� �ʿ�)
    /// character ĳ���� ������
    /// weather ���� ������
    /// slopeAngleRad ���� ��絵 (����)
    /// intensityFactor Ȱ�� ���� (0-1+)
    /// durationMinutes Ȱ�� ���� �ð� (��)
    /// -> ���� �Ƿε� ���� (�����ϸ� ��)
    public float CalculateFatigueScore(Character character, Weather weather, float slopeAngleRad, float intensityFactor, float durationMinutes)
    {

        float bmi = CalculateBMI(character.Weight, character.Height);
        float bmr = CalculateBMR(character.Weight, character.Height, character.Age, character.CharacterGender);

        // SlopeAngle�� ��(Degree)�� ��ȯ�Ͽ� ����� ���� ������, ���Ŀ� ���� ���� ��� ����
        float slopeFactor = k_slope * slopeAngleRad; // ���� ��� ����
        float tempFactor = k_temp * Mathf.Pow(weather.TemperatureCelcius - 21f, 2);
        float humidityFactor = k_humid * weather.Humidity;

        float score = 0.5f +
                      (0.073f * bmi) +
                      (0.00064f * bmr) +
                      (w3_Temp * tempFactor) +
                      (w4_Humid * humidityFactor) +
                      (w5_Slope * slopeFactor) +
                      (0.694f * intensityFactor) +
                      (0.0092f * character.Age) +
                      (0.087f * durationMinutes); // durationMinutes -> durationMinutes / 60.0f (�ð� ����) ������ ���� ����

        // ���� ������ 0-100 ���̷� �����ϸ��ϰų� Ŭ�����ϴ� ���� �߰� �ʿ� �� �� ����

        return Mathf.Max(0, score); // �ּ� 0�� ����
    }
}