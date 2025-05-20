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
            // ���, ����/����� � ���� ���¹̳� ��� ���� ����
        }

        return Mathf.Max(0, Mathf.RoundToInt(finalCost));
    }

    public float CalculateStaminaRegen(Character character)
    {
        float baseRegen = 5f; // �ʴ� ���¹̳� ȸ����
        if (character != null && character.CurrentHP < character.MaxHP * 0.3f)
        {
            baseRegen *= 0.5f; // ü���� ������ ȸ�� �ӵ� ����
        }
        // �޽� ����, ���� � ���� ȸ���� ����
        return baseRegen;
    }
}
