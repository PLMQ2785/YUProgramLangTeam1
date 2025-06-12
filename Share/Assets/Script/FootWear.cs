using UnityEngine;

[System.Serializable]
public class FootWear : Equipment
{
    public enum SoleType { Normal, Luxury, Light }

    public SoleType soleType = SoleType.Normal;
    
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
        if (maxDurability <= 0)
        {
            durability = 0;
            return;
        }
        float wearPercentage = currentWear / maxDurability;
        durability = (int)Mathf.Clamp((1.0f - wearPercentage) * maxDurability, 0, maxDurability);
    }
}