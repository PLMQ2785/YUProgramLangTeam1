using UnityEngine;

public enum Gender { Male, Female }

[System.Serializable] // 인스펙터에서 Character 스크립트에 표시되도록 함
public class FatigueCalculator
{
    [Header("Calculation Coefficients (조정 필요)")]
    [Tooltip("경사도 계수")]
    public float k_slope = 0.1f;
    [Tooltip("기온 계수 (21도 기준)")]
    public float k_temp = 0.005f;
    [Tooltip("습도 계수")]
    public float k_humid = 0.002f;

    [Header("Fatigue Formula Weights (조정 필요)")]
    [Tooltip("기온 가중치")]
    public float w3_Temp = 1.0f;
    [Tooltip("습도 가중치")]
    public float w4_Humid = 1.0f;
    [Tooltip("경사도 가중치")]
    public float w5_Slope = 1.5f;

    /// BMI (체질량 지수)를 계산.
    /// weightKg 몸무게 (kg)
    /// heightCm 키 (cm)
    /// ->BMI 값
    public float CalculateBMI(float weightKg, float heightCm)
    {
        if (heightCm <= 0) return 0;
        float heightM = heightCm / 100f;
        return weightKg / (heightM * heightM);
    }

    /// BMR (기초 대사량)을 Mifflin-St Jeor 공식으로 계산. 기본 수식 제공 -> 팀원 이우혁
    /// weightKg 몸무게 (kg)
    /// heightCm 키 (cm)
    /// age 나이
    /// gender 성별
    /// ->BMR 값
    public float CalculateBMR(float weightKg, float heightCm, int age, Gender gender)
    {
        float bmr = (10f * weightKg) + (6.25f * heightCm) - (5f * age);
        return gender == Gender.Male ? bmr + 5f : bmr - 161f;
    }

    /// 경사도를 라디안 단위로 계산합니다.
    /// deltaH 수직 이동 거리
    /// deltaD 수평 이동 거리
    /// ->경사도 (라디안)
    public float CalculateSlopeAngle(float deltaH, float deltaD)
    {
        return (deltaD > 0.001f) ? Mathf.Atan(deltaH / deltaD) : 0f;
    }

    /// 최종 피로도 점수를 계산. (0-100 스케일링 필요)
    /// character 캐릭터 데이터
    /// weather 날씨 데이터
    /// slopeAngleRad 현재 경사도 (라디안)
    /// intensityFactor 활동 강도 (0-1+)
    /// durationMinutes 활동 지속 시간 (분)
    /// -> 계산된 피로도 점수 (스케일링 전)
    public float CalculateFatigueScore(Character character, Weather weather, float slopeAngleRad, float intensityFactor, float durationMinutes)
    {

        float bmi = CalculateBMI(character.Weight, character.Height);
        float bmr = CalculateBMR(character.Weight, character.Height, character.Age, character.CharacterGender);

        // SlopeAngle을 도(Degree)로 변환하여 사용할 수도 있지만, 공식에 따라 라디안 사용 가능
        float slopeFactor = k_slope * slopeAngleRad; // 라디안 사용 예시
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
                      (0.087f * durationMinutes); // durationMinutes -> durationMinutes / 60.0f (시간 단위) 등으로 조정 가능

        // 계산된 점수를 0-100 사이로 스케일링 필요할 수 있는데, 일단... 그냥 쓰고 반환값을 0-100으로 해둠

        return Mathf.Max(0, score); // 최소 0점 보장
    }
}