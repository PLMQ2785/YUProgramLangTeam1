using UnityEngine;


[System.Serializable]
public class Equipment
{
    public string itemName = "Generic Equipment";
    public int maxDurability = 100;
    public int durability;
    // public float weight; // 장비 무게 속성 추가 고려

    public Equipment()
    {
        durability = maxDurability;
    }

    public Equipment(string name, int maxDura, int currentDura/*, float itemWeight = 0f*/)
    {
        itemName = name;
        maxDurability = maxDura;
        durability = Mathf.Clamp(currentDura, 0, maxDurability);
        // weight = itemWeight;
    }

    public virtual void Use(int amount = 1) // int로 변경 (내구도 비용 계산 결과가 int)
    {
        durability -= amount;
        if (durability < 0) durability = 0;
        Debug.Log($"{itemName} used. Durability: {durability}/{maxDurability}");
    }

    public void Repair(int amount)
    {
        durability += amount;
        if (durability > maxDurability) durability = maxDurability;
        Debug.Log($"{itemName} repaired. Durability: {durability}/{maxDurability}");
    }

    public float GetDurabilityNormalized()
    {
        if (maxDurability == 0) return 0;
        return (float)durability / maxDurability;
    }
}
