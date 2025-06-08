using UnityEngine;

[System.Serializable]
public class FootWear : Equipment
{
    public enum SoleType { Normal, Luxury, Light }

    public SoleType soleType = SoleType.Normal;

    [Header("Wear & Durability")]
    [Tooltip("신발이 감당할 수 있는 총 마모량 (이 값이 클수록 오래 감)")]
    public float maxWearCapacity = 1000f; 
    [Tooltip("현재 누적된 마모량")]
    public float currentWear = 0f;

    public FootWear() : base()
    {
        itemName = "Normal";
    }

    public FootWear(string name, int maxDura, int currentDura, SoleType type, float itemWeight = 0.5f)
        : base(name, maxDura, currentDura, itemWeight)
    {
        this.soleType = type;
        UpdateDurabilityFromWear();
    }

    public void UpdateDurabilityFromWear()
    {
        if (maxWearCapacity <= 0)
        {
            durability = 0;
            return;
        }
        float wearPercentage = currentWear / maxWearCapacity;
        durability = (int)Mathf.Clamp((1.0f - wearPercentage) * maxDurability, 0, maxDurability);
    }
}