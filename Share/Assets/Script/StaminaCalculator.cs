using UnityEngine;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class StaminaCalculator
{
    public int CalculateStaminaCost(Character character, string actionType, float actionIntensity = 1.0f)
    {
        int baseCost = 0;
        switch (actionType.ToLower())
        {
            case "run":
                baseCost = 5;
                break;
            case "jump":
                baseCost = 15;
                break;
            case "attack":
                baseCost = 10;
                break;
            default:
                baseCost = 1;
                break;
        }

        float finalCost = baseCost * actionIntensity;

        if (character != null)
        {
            if (character.IsOverloaded()) finalCost *= 1.5f;
            // 장비, 버프/디버프 등에 따른 스태미나 비용 가감 로직
        }

        return Mathf.Max(0, Mathf.RoundToInt(finalCost));
    }

    public float CalculateStaminaRegen(Character character)
    {
        float baseRegen = 5f; // 초당 스태미나 회복량
        if (character != null && character.CurrentHP < character.MaxHP * 0.3f)
        {
            baseRegen *= 0.5f; // 체력이 낮으면 회복 속도 감소
        }
        // 휴식 상태, 버프 등에 따른 회복량 조절
        return baseRegen;
    }
}
