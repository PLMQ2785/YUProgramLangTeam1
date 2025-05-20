using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField] private float height = 1.8f;
    [SerializeField] private float weight = 70f;
    [SerializeField] private float currentLoad = 0f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Equipment & Calculators")]
    [SerializeField] private FootWear equippedFootWear;
    [SerializeField] private StaminaCalculator staminaCalc = new StaminaCalculator();
    [SerializeField] private DurabilityCalculator durabilityCalc = new DurabilityCalculator();

    [Header("State")]
    [SerializeField] private bool isOverloadedFlag = false;

    public float Height => height;
    public float Weight => weight;
    public float CurrentLoad => currentLoad;
    public float TotalWeight => weight + currentLoad;
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public FootWear EquippedFootWear => equippedFootWear;

    void Awake()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        if (staminaCalc == null) staminaCalc = new StaminaCalculator();
        if (durabilityCalc == null) durabilityCalc = new DurabilityCalculator();

        if (equippedFootWear == null) // 기본 신발 장착 예시
        {
            EquipFootWear(new FootWear("Boots1", 50, 30, FootWear.SoleType.Normal));
        }
        UpdateCurrentLoad();
    }

    void Update()
    {
        if (currentStamina < maxStamina)
        {
            RegenerateStamina(staminaCalc.CalculateStaminaRegen(this) * UnityEngine.Time.deltaTime);
        }
        // UI 업데이트 (캐릭터 상태 변화 시)
    }

    public void Move(Vector3 direction, float speedMultiplier = 1.0f)
    {
        // 실제 이동은 CharacterController에서 처리. 여기서는 상태 변경 및 비용 계산.
        bool isRunning = speedMultiplier > 1.1f && direction.magnitude > 0.1f;
        if (isRunning)
        {
            ConsumeStamina(CalcStaminaCost("run", UnityEngine.Time.deltaTime * speedMultiplier));
        }
        if (equippedFootWear != null && direction.magnitude > 0.1f)
        {
            UseEquipment(equippedFootWear, "walkstep", 0.05f * speedMultiplier * (isRunning ? 2f : 1f));
        }
    }

    public void Jump()
    {
        if (CanPerformAction(CalcStaminaCost("jump")))
        {
            ConsumeStamina(CalcStaminaCost("jump"));
            Debug.Log("Character Jump Action Initiated");
        }
        else
        {
            Debug.Log("Not enough stamina to jump!");
        }
    }

    public void TakeAction(string actionType, float intensity = 1.0f)
    {
        int cost = CalcStaminaCost(actionType, intensity);
        if (CanPerformAction(cost))
        {
            ConsumeStamina(cost);
            Debug.Log($"Character performed action: {actionType}");
            // 실제 행동 로직 수행
        }
        else
        {
            Debug.Log($"Not enough stamina for action: {actionType}");
        }
    }

    public void ChangeLoadWeight(float amount)
    {
        currentLoad += amount;
        if (currentLoad < 0) currentLoad = 0;
        isOverloadedFlag = IsOverloaded();
        Debug.Log($"Load changed by {amount}. Current load: {currentLoad}. Overloaded: {isOverloadedFlag}");
    }

    public int CalcStaminaCost(string actionType, float intensity = 1.0f)
    {
        if (staminaCalc == null) return 0;
        return staminaCalc.CalculateStaminaCost(this, actionType, intensity);
    }

    public void ConsumeStamina(int amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
    }

    public void RegenerateStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
    }

    public bool CanPerformAction(int staminaCost)
    {
        return currentStamina >= staminaCost;
    }

    public int CalcDurabilityCost(Equipment item, string usageContext, float intensity = 1.0f)
    {
        if (durabilityCalc == null) return 0;
        return durabilityCalc.CalculateDurabilityCost(item, usageContext, intensity);
    }

    public void UseEquipment(Equipment item, string usageContext, float intensity = 1.0f)
    {
        if (item == null) return;
        int cost = CalcDurabilityCost(item, usageContext, intensity);
        item.Use(cost);

        if (gameObject.CompareTag("Player"))
        {
            //UIManager uiManager = FindObjectOfType<UIManager>(); // GameManager를 통해 접근 권장
            UIManager uiManager = GameManager.Instance.GetUIManager();
            uiManager?.UpdateDurabilitySlider(item.durability, item.maxDurability);
        }
    }

    public bool IsOverloaded()
    {
        float maxCarryWeight = weight * 1.5f; // 예: 기본 몸무게의 1.5배까지 운반 가능
        return TotalWeight > maxCarryWeight;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        Debug.Log($"{name} took {amount} damage. HP: {currentHP}/{maxHP}");
        if (currentHP <= 0)
        {
            Die();
        }
        // UI 업데이트 (체력 바 등)
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        Debug.Log($"{name} healed {amount} HP. HP: {currentHP}/{maxHP}");
        // UI 업데이트
    }

    private void Die()
    {
        Debug.Log($"{name} has died.");
        // 사망 관련 로직 (애니메이션, 리스폰, 게임 오버 등)
    }

    public void EquipFootWear(FootWear newFootWear)
    {
        // 이전 신발 해제 및 인벤토리 반환 로직 (필요 시)
        equippedFootWear = newFootWear;
        if (newFootWear != null)
        {
            Debug.Log($"Equipped: {newFootWear.itemName}");
        }
        else
        {
            Debug.Log("FootWear unequipped.");
        }
        UpdateCurrentLoad();
    }

    private void UpdateCurrentLoad()
    {
        currentLoad = 0f;
        // 모든 장착/소지 아이템 무게 합산 로직
        // if (equippedFootWear != null && equippedFootWear.weight > 0) currentLoad += equippedFootWear.weight;
        isOverloadedFlag = IsOverloaded();
    }
}
