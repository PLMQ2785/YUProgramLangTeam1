﻿using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField] private int age = 25;
    [SerializeField] private Gender characterGender = Gender.Male;
    [SerializeField] private float height = 180f; //cm
    [SerializeField] private float weight = 70f;  //kg

    [Header("Health & Load")]
    [SerializeField] private float currentLoad = 0f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 1f;

    [Header("Equipment & Calculators")]
    [SerializeField] private FootWear equippedFootWear;
    [SerializeField] private StaminaCalculator staminaCalc = new StaminaCalculator();
    [SerializeField] private DurabilityCalculator durabilityCalc = new DurabilityCalculator();
    [SerializeField] private FatigueCalculator fatigueCalc = new FatigueCalculator(); // 피로도 계산기 추가

    [Header("Fatigue System")]
    [SerializeField, Range(0f, 100f)] private float currentFatigue = 0f;
    private float activityDurationSeconds = 0f;
    private Vector3 previousPosition;
    private float intensityFactor = 0f;
    private float currentSlopeAngleRad = 0f; // 라디안 단위 경사도 저장
    private float fatigueUpdateInterval = 5.0f; // 피로도 업데이트 주기 (5초)
    private float timeSinceLastFatigueUpdate = 0f;

    [Header("State")]
    [SerializeField] private bool isOverloadedFlag = false;

    [Header("Dependencies")]
    private UIManager _uiManager;

    private Weather weather;

    private float fixedTerrainFactor = 1.1f;
    private float totalDistanceKm = 0f; // 총 이동 거리(km)

    public int Age => age;
    public Gender CharacterGender => characterGender;
    public float Height => height;
    public float Weight => weight;
    public float CurrentLoad => currentLoad;
    public float TotalWeight => weight + currentLoad;
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public FootWear EquippedFootWear => equippedFootWear;
    public float CurrentFatigue => currentFatigue;
    public float CurrentSlopeAngleRad => currentSlopeAngleRad;
    public float TotalDistanceKm => totalDistanceKm;

    public void SetHeight(float h) { height = h; }
    public void SetWeight(float w) { weight = w; }
    public void SetAge(int a) { age = a; }
    public void SetGender(Gender g) { characterGender = g; }
    public FatigueCalculator GetFatigueCalculator() { return fatigueCalc; }
    public void UpdateSlopeAngle(float slopeRad) { currentSlopeAngleRad = slopeRad; }
    public void SetCurrentLoad(float newLoad)
    {
        currentLoad = Mathf.Max(0, newLoad); // 하중이 음수 방지
        isOverloadedFlag = IsOverloaded(); // 과적 상태를 계산.... 이긴 한데 굳이 필요할까 싶다
        Debug.Log($"Current Load set to: {currentLoad}. Overloaded: {isOverloadedFlag}");
    }

    private GameTime _gameTime;

    void Awake()
    {

    }

    void Start()
    {
        _uiManager = GameManager.Instance.GetUIManager(); // GameManager를 통해 UIManager 접근
        _gameTime = GameManager.Instance.GetGameTime(); // GameManager를 통해 GameTime 접근

        previousPosition = transform.position;

        currentHP = maxHP;
        currentStamina = maxStamina;
        staminaCalc = new StaminaCalculator();
        durabilityCalc = new DurabilityCalculator();
        fatigueCalc = new FatigueCalculator(); // 피로도 계산기 초기화

        //Weather weather = FindObjectOfType<Weather>();
        weather = FindFirstObjectByType<Weather>();


        //기본 신발 장착, 여기서 지정한 내구도 대로 따라갑니다
        //EquipFootWear(new FootWear("Boots1", 100, 100, FootWear.SoleType.Normal));
        EquipFootWear(new FootWear("Boots1", 1250, 1250, FootWear.SoleType.Normal, 0.5f));
        UpdateCurrentLoad();


    }

    void Update()
    {
        float dt = UnityEngine.Time.deltaTime;
        float gameDeltaTime = UnityEngine.Time.deltaTime * _gameTime.TimeMultiplier;

        if (currentStamina < maxStamina)
        {
            RegenerateStamina(staminaCalc.CalculateStaminaRegen(this) * gameDeltaTime);
        }

        // UI 업데이트 (캐릭터 상태 변화 시)


        // 활동 시간 및 피로도 업데이트
        activityDurationSeconds += gameDeltaTime;
        timeSinceLastFatigueUpdate += dt;

        // 위치 변화 및 경사도, 강도 계산
        CalculateMovementDeltas();

        // 일정 주기마다 피로도 업데이트
        if (timeSinceLastFatigueUpdate >= fatigueUpdateInterval)
        {
            UpdateFatigue();
            timeSinceLastFatigueUpdate = 0f;
        }

    }

    private void CalculateMovementDeltas()
    {
        float deltaTime = UnityEngine.Time.deltaTime;

        Vector3 currentPosition = transform.position;
        Vector3 deltaPosition = currentPosition - previousPosition;

        float distanceDeltaMeters = deltaPosition.magnitude;

        // --- 새로운 내구도 계산 로직 ---
        if (distanceDeltaMeters > 0.001f && equippedFootWear != null)
        {
            float distanceDeltaKm = distanceDeltaMeters / 1000f;
            totalDistanceKm += distanceDeltaKm;

            // 앞으로 이걸로 호출할 것
            //
            float wearAmount = durabilityCalc.CalculateWearAmount(this, weather, distanceDeltaKm, fixedTerrainFactor) * 1f;
            Debug.Log("WearAmount:" + wearAmount);

            if (wearAmount > 0)
            {
                equippedFootWear.currentWear += wearAmount;
                equippedFootWear.UpdateDurabilityFromWear();
                _uiManager?.UpdateDurabilitySlider(equippedFootWear.durability, equippedFootWear.maxDurability);

                Debug.Log("Current Dura: " + equippedFootWear.durability);
            }
        }

        float deltaH = deltaPosition.y;
        float deltaD_horizontal = new Vector2(deltaPosition.x, deltaPosition.z).magnitude;
        currentSlopeAngleRad = fatigueCalc.CalculateSlopeAngle(deltaH, deltaD_horizontal);
        float currentSpeed = distanceDeltaMeters / deltaTime;
        intensityFactor = Mathf.Clamp01(currentSpeed / moveSpeed);
        previousPosition = currentPosition;
    }

    private void UpdateFatigue()
    {
        //weather = GameManager.Instance.GetWeather();
        float durationMinutes = activityDurationSeconds;
        float newFatigue = fatigueCalc.CalculateFatigueScore(this, weather, currentSlopeAngleRad, intensityFactor, durationMinutes);

        // 피로도를 누적? 매번 새로 계산?
        // 새로 계산된 값을 사용하고 0-100으로 지정해보기
        currentFatigue = Mathf.Clamp(newFatigue, 0f, 100f);

        Debug.Log($"Fatigue Updated: {currentFatigue:F2}, NewFatigue {newFatigue}");

        // UI 업데이트
        _uiManager.FatigueScoreText = currentFatigue.ToString("F1");
    }

    public void Move(Vector3 direction, float speedMultiplier = 1.0f, bool isGrounded = false)
    {
        // 실제 이동은 CharacterController에서 처리. 여기서는 상태 변경 및 비용 계산.
        bool isRunning = speedMultiplier > 1.1f && direction.magnitude > 0.1f;
        if (isRunning)
        {
            ConsumeStamina(CalcStaminaCost("run", UnityEngine.Time.deltaTime * speedMultiplier));
        }
        if (equippedFootWear != null && direction.magnitude > 0.1f && isGrounded)
        {
            //UseEquipment(equippedFootWear, "walkstep", 0.05f * speedMultiplier * (isRunning ? 0.05f : 0.01f)); //여기서 호출하지 말고 move delta로
            //UseEquipment(equippedFootWear, "walkstep", (0.05f * speedMultiplier * (isRunning ? 0.05f : 0.01f)), );
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

    //일단 만들어는 둠
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

    //public void ChangeLoadWeight(float amount)
    //{
    //    currentLoad += amount;
    //    if (currentLoad < 0) currentLoad = 0;
    //    isOverloadedFlag = IsOverloaded();
    //    Debug.Log($"Load changed by {amount}. Current load: {currentLoad}. Overloaded: {isOverloadedFlag}");
    //}

    public int CalcStaminaCost(string actionType, float intensity = 1.0f)
    {
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


    //이거 2개 이제 직접 호출 없음
    //public int CalcDurabilityCost(Equipment item, string usageContext,
    //                                float moveDist, float terrainCoef, float userWeight, float loadWeight, float weatherCoef,
    //                                float w_dist, float w_terrain, float w_userWeight, float w_loadWeight, float w_weather, float intensity = 1.0f)
    //{
    //    return durabilityCalc.CalculateDurabilityCost(item, usageContext, moveDist, terrainCoef, userWeight,
    //                                                    loadWeight, weatherCoef, w_dist, w_terrain, w_userWeight,
    //                                                    w_loadWeight, w_weather, intensity);
    //}

    //public void UseEquipment(Equipment item, string usageContext,
    //                            float moveDist, float terrainCoef, float userWeight, float loadWeight, float weatherCoef,
    //                            float w_dist, float w_terrain, float w_userWeight, float w_loadWeight, float w_weather, float intensity = 1.0f)
    //{
    //    int cost = CalcDurabilityCost(item, usageContext, moveDist, terrainCoef, userWeight,
    //                                                    loadWeight, weatherCoef, w_dist, w_terrain, w_userWeight,
    //                                                    w_loadWeight, w_weather, intensity);
    //    item.Use(cost);

    //    if (gameObject.CompareTag("Player"))
    //    {
    //        //UIManager uiManager = FindObjectOfType<UIManager>(); 
    //        _uiManager?.UpdateDurabilitySlider(item.durability, item.maxDurability);
    //    }
    //}

    public bool IsOverloaded()
    {
        float maxCarryWeight = weight * 1.5f; // 예: 기본 몸무게의 1.5배까지 운반 가능, 필요할까?..
        return TotalWeight > maxCarryWeight;
    }

    //public void TakeDamage(float amount)
    //{
    //    currentHP -= amount;
    //    if (currentHP < 0) currentHP = 0;
    //    Debug.Log($"{name} took {amount} damage. HP: {currentHP}/{maxHP}");
    //    if (currentHP <= 0)
    //    {
    //        Die();
    //    }
    //    // UI 업데이트 (체력 바 등)
    //}

    //public void Heal(float amount)
    //{
    //    currentHP += amount;
    //    if (currentHP > maxHP) currentHP = maxHP;
    //    Debug.Log($"{name} healed {amount} HP. HP: {currentHP}/{maxHP}");
    //    // UI 업데이트
    //}

    //private void Die()
    //{
    //    Debug.Log($"{name} has died.");
    //    // 사망 관련 로직 (애니메이션, 리스폰, 게임 오버 등)
    //}

    public void EquipFootWear(FootWear newFootWear)
    {
        // 이전 신발 해제 및 인벤토리 반환 로직 (필요 시)
        equippedFootWear = newFootWear;
        if (newFootWear != null)
        {
            // 신발 타입에 따라 maxWearCapacity 설정
            // 기본 계산조건에서 마모율 2.5, 500*2.5=1250, 800*2.5=1600, 300*2.5=900
            switch (newFootWear.soleType)
            {
                case FootWear.SoleType.Normal:
                    newFootWear.maxDurability = 1250; // 목표: 500km
                    break;
                case FootWear.SoleType.Luxury:
                    newFootWear.maxDurability = 1600; // 목표: 800km
                    break;
                case FootWear.SoleType.Light:
                    newFootWear.maxDurability = 900;  // 목표: 300km
                    break;
            }

            Debug.Log($"Equipped: {newFootWear.itemName}");
            equippedFootWear = newFootWear;
            _uiManager.UpdateDurabilitySlider(newFootWear.durability, newFootWear.maxDurability);
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

    public void ChangeFootwearType(FootWear.SoleType newType)
    {

        if (equippedFootWear != null)
        {
            Debug.Log(newType);

            switch (newType)
            {
                case FootWear.SoleType.Normal:
                    equippedFootWear.maxDurability = 1250; // 목표: 500km
                    break;
                case FootWear.SoleType.Luxury:
                    equippedFootWear.maxDurability = 1600; // 목표: 800km
                    break;
                case FootWear.SoleType.Light:
                    equippedFootWear.maxDurability = 900;  // 목표: 300km
                    break;
            }


            equippedFootWear.soleType = newType;
            // 내구도를 최대로 재설정
            equippedFootWear.durability = equippedFootWear.maxDurability;
            equippedFootWear.currentWear = 0f; // 마모량 초기화
            Debug.Log($"Footwear type changed to {newType}, Durability reset to {equippedFootWear.durability}.");

            // UI 업데이트 호출
            _uiManager?.UpdateDurabilitySlider(equippedFootWear.durability, equippedFootWear.maxDurability);
        }
        else
        {
            Debug.LogWarning("Cannot change sole type: No footwear equipped.");
            // 또는, 해당 타입의 새 신발을 생성/장착하는 로직 추가 가능
            // EquipFootwear(new Footwear("New Boots", 100, 100, newType));
        }
    }

    public void OnLanded()
    {
        //if (equippedFootWear != null)
        //{
        //    Debug.Log("Landed! Applying durability wear.");
        //    // 착지 충격에 대한 마모도 0.5
        //    equippedFootWear.currentWear += 0.5f;
        //    equippedFootWear.UpdateDurabilityFromWear();
        //    _uiManager?.UpdateDurabilitySlider(equippedFootWear.durability, equippedFootWear.maxDurability);
        //}
    }
}
