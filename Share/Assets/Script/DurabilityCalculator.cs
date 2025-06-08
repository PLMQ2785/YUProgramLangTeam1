using UnityEngine;

[System.Serializable]
public class DurabilityCalculator
{
    [Header("Formula Weights (가중치)")]
    [Tooltip("w_dist: 이동 거리에 대한 가중치")]
    public float w_dist = 0.89f;
    [Tooltip("w_terrain: 지형 계수에 대한 가중치")]
    public float w_terrain = 0.5f;
    [Tooltip("w_userWeight: 사용자 체중에 대한 가중치")]
    public float w_userWeight = 0.0015f;
    [Tooltip("w_loadWeight: 하중에 대한 가중치")]
    public float w_loadWeight = 0.004f;
    [Tooltip("w_weather: 기후 계수에 대한 가중치")]
    public float w_weather = 1.0f;

    /// 기후 계수(ε)를 계산합니다.
    private float GetClimateFactor(Weather weather)
    {
        if (weather == null) return 1.0f;

        // 습도 계수(ε1)
        float e1 = 1.0f;
        if (weather.Humidity > 80) e1 = 1.5f;
        else if (weather.Humidity > 60) e1 = 1.3f;
        else if (weather.Humidity > 30) e1 = 1.1f;

        // 온도 계수(ε2)
        float e2 = 1.0f;
        if (weather.TemperatureCelcius < 0 || weather.TemperatureCelcius > 30) e2 = 1.3f;

        return (e1 + e2) / 2.0f;
    }

    /// 마모도(Wear)를 계산합니다.
    public float CalculateWearAmount(Character character, Weather weather, float moveDistKm, float terrainCoef)
    {
        float userWeight = character.Weight;
        float loadWeight = character.CurrentLoad;
        float weatherCoef = GetClimateFactor(weather);

        // 최종 마모량 = (각 요인별 마모율의 합) * 이동 거리
        // W = (w_dist + w_terrain*T + w_userWeight*M + w_loadWeight*H + w_weather*ε) * D

        float totalWearRate = w_dist +
                              (w_terrain * terrainCoef) +
                              (w_userWeight * userWeight) +
                              (w_loadWeight * loadWeight) +
                              (w_weather * weatherCoef);

        float finalWear = totalWearRate * moveDistKm;

        //// 깔창 타입에 따른 보너스/페널티
        //if (character.EquippedFootWear != null)
        //{
        //    switch (character.EquippedFootWear.soleType)
        //    {
        //        case FootWear.SoleType.Luxury:
        //            finalWear *= 0.8f; // 고급: 마모도 20% 감소
        //            break;
        //        case FootWear.SoleType.Light:
        //            finalWear *= 1.2f; // 경량: 마모도 20% 증가
        //            break;
        //        case FootWear.SoleType.Normal:
        //        default:
        //            // 일반: 변화 없음
        //            break;
        //    }
        //}

        return Mathf.Max(0, finalWear);
    }
}