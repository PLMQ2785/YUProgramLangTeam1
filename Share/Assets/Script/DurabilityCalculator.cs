using UnityEngine;


[System.Serializable]
public class DurabilityCalculator
{
    public int CalculateDurabilityCost(Equipment equipment, string usageContext, float intensity = 1.0f)
    {

        float baseDamage = 0.1f; // 기본 내구도 소모량

        if (equipment is FootWear footwear)
        {
            baseDamage = usageContext.ToLower() switch
            {
                "walkstep" => 1,
                //"runstep" => 2,
                //"jump" => 3,
                _ => 1 // 기본값
            };
            if (footwear.soleType == FootWear.SoleType.HeavyDuty) baseDamage = Mathf.Max(1, baseDamage / 2);


            //if (usageContext.ToLower() == "walkstep")
            //{
            //    baseDamage = 1;
            //    if (footwear.soleType == FootWear.SoleType.HeavyDuty) baseDamage = Mathf.Max(1, baseDamage / 2);
            //}
        }
        // 다른 장비 종류 및 사용 상황에 따른 내구도 소모량 계산

        float finalDamage = baseDamage * intensity;

        //return Mathf.Max(0, Mathf.RoundToInt(finalDamage));                                                       
        // 1보다 작으면 0, 아니면 올림하여 1 이상 정수 반환 (매번 닳지 않게)
        return (finalDamage < 1f) ? (Random.value < finalDamage ? 1 : 0) : Mathf.CeilToInt(finalDamage);
    }
}
