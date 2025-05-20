using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))] // 또는 CapsuleCollider
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))] // Character 데이터 컴포넌트 필수
public class CharacterController : MonoBehaviour
{
    [Header("Dependencies")]
    private InputManager inputManager;
    // private UIManager uiManager; // GameManager 통해 접근 권장

    [Header("Components")]
    private Character characterData;
    private Rigidbody playerRB;
    private BoxCollider playerCollider; // 또는 CapsuleCollider
    private Animator playerAnimator;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 720f;

    // Animation Parameters
    private const string ANIM_PARAM_SPEED = "Speed";
    private const string ANIM_PARAM_JUMP = "JumpTrigger";
    private const string ANIM_PARAM_GROUNDED = "IsGrounded";

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, -0.8f, 0); // 콜라이더 하단 기준 조정
    private bool isGrounded;

    [Header("Slope Detection")]
    [SerializeField] private float slopeRaycastDistance = 1.5f;
    private float currentSlopeAngle;

    public void Init(InputManager inpManager)
    {
        inputManager = inpManager;
        // uiManager = FindObjectOfType<UIManager>(); // GameManager 통해 접근 권장

        characterData = GetComponent<Character>();
        playerRB = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();
        playerAnimator = GetComponent<Animator>();

        if (characterData == null) Debug.LogError("Character data component not found!");
        if (inputManager == null) Debug.LogError("InputManager not provided to CharacterController!");

        playerRB.freezeRotation = true;
        playerRB.interpolation = RigidbodyInterpolation.Interpolate;
        playerRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Debug.Log("CharacterController Initialized.");
    }

    void Update()
    {
        if (inputManager == null || characterData == null) return;

        HandleInput();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (inputManager == null || characterData == null) return;

        GroundCheck();
        CalculateSlopeAngle();
        PerformMovement();
        PerformRotation();
    }

    private void HandleInput()
    {
        Vector3 moveInput = new Vector3(inputManager.HorizontalInputValue, 0, inputManager.VerticalInputValue).normalized;
        bool isRunning = inputManager.IsKeyPressed(KeyCode.LeftShift) && characterData.CurrentStamina > characterData.CalcStaminaCost("run", UnityEngine.Time.fixedDeltaTime);
        float currentSpeedMultiplier = isRunning ? 1.5f : 1.0f;

        characterData.Move(moveInput, currentSpeedMultiplier); // Character 데이터에 이동 의도 전달

        if (inputManager.JumpTriggered && isGrounded && characterData.CanPerformAction(characterData.CalcStaminaCost("jump")))
        {
            characterData.Jump(); // Character 데이터가 스태미나 처리
            ApplyJumpForce();
            playerAnimator.SetTrigger(ANIM_PARAM_JUMP);
        }

        if (inputManager.IsKeyDown(KeyCode.E))
        {
            characterData.TakeAction("interact");
        }
    }

    private Vector3 currentMoveDirectionForPhysics = Vector3.zero;

    private void PerformMovement()
    {
        Vector3 moveInput = new Vector3(inputManager.HorizontalInputValue, 0, inputManager.VerticalInputValue).normalized;
        currentMoveDirectionForPhysics = moveInput;

        bool isRunning = inputManager.IsKeyPressed(KeyCode.LeftShift) && characterData.CurrentStamina > 0; // 스태미나 소모는 Character.Move에서 처리
        float actualSpeed = characterData.moveSpeed * (isRunning ? 1.5f : 1.0f);

        Vector3 worldMoveDirection = transform.TransformDirection(moveInput);
        Vector3 targetVelocity = worldMoveDirection * actualSpeed;
        targetVelocity.y = playerRB.linearVelocity.y; // y축 속도 유지

        Vector3 velocityChange = (targetVelocity - playerRB.linearVelocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -10f, 10f);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -10f, 10f);
        velocityChange.y = 0;

        if (isGrounded)
        {
            playerRB.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            playerRB.AddForce(velocityChange * 0.2f, ForceMode.VelocityChange); // 공중 제어력 감소
        }
        //필요는 없을거 같음
        //AdjustMovementForSlope();
    }

    private void PerformRotation()
    {
        float mouseXDelta = inputManager.MouseXValue * rotationSpeed * UnityEngine.Time.deltaTime;
        transform.Rotate(Vector3.up, mouseXDelta);
    }

    private void ApplyJumpForce()
    {
        playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, 0f, playerRB.linearVelocity.z); // 점프 전 y축 속도 초기화
        playerRB.AddForce(Vector3.up * characterData.jumpForce, ForceMode.Impulse);
    }

    private void GroundCheck()
    {
        // BoxCollider의 경우 center와 extents를 고려하여 발밑 위치 계산
        Vector3 rayStartPoint = transform.position + playerCollider.center + new Vector3(0, -playerCollider.bounds.extents.y + 0.1f, 0) + groundCheckOffset;
        isGrounded = Physics.Raycast(rayStartPoint, Vector3.down, groundCheckDistance, groundLayerMask);
        Debug.DrawRay(rayStartPoint, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    private void AdjustMovementForSlope()
    {
        //이건 굳이 필요없을거 같음
    }

    private void CalculateSlopeAngle()
    {
        RaycastHit hitInfo;
        Vector3 rayStart = transform.position + playerCollider.center + Vector3.up * 0.1f;

        if (Physics.Raycast(rayStart, Vector3.down, out hitInfo, playerCollider.bounds.extents.y + 0.5f, groundLayerMask))
        {
            currentSlopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
        }
        else
        {
            currentSlopeAngle = 0f;
        }

        UIManager uiManager = FindObjectOfType<UIManager>(); // GameManager 통해 접근 권장
        if (uiManager != null)
        {
            uiManager.SlopeAngleText = currentSlopeAngle.ToString("F1") + "°";
        }
    }

    private void UpdateAnimator()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(playerRB.linearVelocity);
        float forwardSpeed = localVelocity.z;

        playerAnimator.SetFloat(ANIM_PARAM_SPEED, Mathf.Abs(forwardSpeed)); // 속력으로 전달 (전/후진 모두)
        playerAnimator.SetBool(ANIM_PARAM_GROUNDED, isGrounded);
    }

    private void OnFootstep(AnimationEvent animationEvent) // 애니메이션 이벤트에서 호출
    {
        Debug.Log("Footstep event triggered by animation.");
        // 발소리 재생 로직 (AudioManager 연동 등)
        // AudioManager.Instance.PlayFootstepSound(characterData.EquippedFootwear.soleType); <- 프로토타입만 구상
    }
}
