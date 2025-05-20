using UnityEngine;


[System.Serializable]
public class DurabilityCalculator
{
    public int CalculateDurabilityCost(Equipment equipment, string usageContext, float intensity = 1.0f)
    {
        if (equipment == null) return 0;

        int baseDamage = 1; // �⺻ ������ �Ҹ�

        if (equipment is FootWear footwear)
        {
            if (usageContext.ToLower() == "walkstep")
            {
                baseDamage = 1;
                if (footwear.soleType == FootWear.SoleType.HeavyDuty) baseDamage = Mathf.Max(1, baseDamage / 2);
            }
        }
        // �ٸ� ��� ���� �� ��� ��Ȳ�� ���� ������ �Ҹ� ���

        float finalDamage = baseDamage * intensity;
        return Mathf.Max(0, Mathf.RoundToInt(finalDamage));
    }
}
