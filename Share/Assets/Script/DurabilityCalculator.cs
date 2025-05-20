using UnityEngine;


[System.Serializable]
public class DurabilityCalculator
{
    public int CalculateDurabilityCost(Equipment equipment, string usageContext, float intensity = 1.0f)
    {
        if (equipment == null) return 0;

        int baseDamage = 1; // 기본 내구도 소모량

        if (equipment is FootWear footwear)
        {
            if (usageContext.ToLower() == "walkstep")
            {
                baseDamage = 1;
                if (footwear.soleType == FootWear.SoleType.HeavyDuty) baseDamage = Mathf.Max(1, baseDamage / 2);
            }
        }
        // 다른 장비 종류 및 사용 상황에 따른 내구도 소모량 계산

        float finalDamage = baseDamage * intensity;
        return Mathf.Max(0, Mathf.RoundToInt(finalDamage));
    }
}
