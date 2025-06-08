using UnityEngine;

[System.Serializable]
public class Equipment
{
    public string itemName = "Generic Equipment";
    public int maxDurability = 100;
    public int durability;
    public float itemWeight = 0f; // 장비 무게 속성 추가

    public Equipment()
    {
        durability = maxDurability;
    }

    // itemWeight를 받는 생성자 추가
    public Equipment(string name, int maxDura, int currentDura, float itemWeight = 0f)
    {
        this.itemName = name;
        this.maxDurability = maxDura;
        this.durability = Mathf.Clamp(currentDura, 0, maxDurability);
        this.itemWeight = itemWeight;
    }

    public virtual void Use(int amount = 1)
    {
        durability -= amount;
        if (durability < 0) durability = 0;
        // Debug.Log($"{itemName} used. Durability: {durability}/{maxDurability}"); // 로그가 너무 많이 찍히므로 주석 처리 권장
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