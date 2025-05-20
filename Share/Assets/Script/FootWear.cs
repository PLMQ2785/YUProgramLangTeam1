using UnityEngine;

[System.Serializable]
public class FootWear : Equipment
{
    public enum SoleType { Normal, Grip, Silent, HeavyDuty }

    public SoleType soleType = SoleType.Normal;

    public FootWear() : base()
    {
        itemName = "Generic Footwear";
    }

    public FootWear(string name, int maxDura, int currentDura, SoleType type/*, float itemWeight = 0.5f*/) : base(name, maxDura, currentDura/*, itemWeight*/)
    {
        soleType = type;
    }

    public override void Use(int amount = 1)
    {
        base.Use(amount);
        // 신발 사용 로직
    }
}
