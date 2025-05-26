using UnityEngine;


[System.Serializable]
public class DurabilityCalculator
{
    public int CalculateDurabilityCost(Equipment equipment, string usageContext, float intensity = 1.0f)
    {

        float baseDamage = 0.1f; // �⺻ ������ �Ҹ�

        if (equipment is FootWear footwear)
        {
            baseDamage = usageContext.ToLower() switch
            {
                "walkstep" => 1,
                //"runstep" => 2,
                //"jump" => 3,
                _ => 1 // �⺻��
            };
            if (footwear.soleType == FootWear.SoleType.HeavyDuty) baseDamage = Mathf.Max(1, baseDamage / 2);


            //if (usageContext.ToLower() == "walkstep")
            //{
            //    baseDamage = 1;
            //    if (footwear.soleType == FootWear.SoleType.HeavyDuty) baseDamage = Mathf.Max(1, baseDamage / 2);
            //}
        }
        // �ٸ� ��� ���� �� ��� ��Ȳ�� ���� ������ �Ҹ� ���

        float finalDamage = baseDamage * intensity;

        //return Mathf.Max(0, Mathf.RoundToInt(finalDamage));                                                       
        // 1���� ������ 0, �ƴϸ� �ø��Ͽ� 1 �̻� ���� ��ȯ (�Ź� ���� �ʰ�)
        return (finalDamage < 1f) ? (Random.value < finalDamage ? 1 : 0) : Mathf.CeilToInt(finalDamage);
    }
}
