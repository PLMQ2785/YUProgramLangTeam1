using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))] // 또는 CapsuleCollider
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))] // Character 데이터 컴포넌트 필수
public class CharacterController : MonoBehaviour
{
    [Header("Dependencies")]
    private InputManager inputManager;
    private UIManager uiManager;

    [Header("Components")]
    private Character characterData;
    private Rigidbody playerRB;
    private BoxCollider playerCollider; // 또는 CapsuleCollider
    private Animator playerAnimator;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 720f;

    // Animation Parameters
    private const string ANIM_PARAM_SPEED = "Speed";
    private const string ANIM_PARAM_JUMP = "Jump";
    private const string ANIM_PARAM_GROUNDED = "Grounded";

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayerMask;
    private float groundCheckDistance = 1f;
    private Vector3 groundCheckOffset = new Vector3(0, -0.0f, 0); // 콜라이더 하단 기준 조정
    [SerializeField] private bool isGrounded;

    [Tooltip("지면 체크 구의 반지름 (캐릭터 콜라이더 너비보다 약간 작게)")]
    [SerializeField] private float groundCheckRadius = 0.45f;
    [Tooltip("지면으로 간주할 최대 거리 (발바닥 ~ 지면)")]
    private RaycastHit groundHitInfo; // 충돌 정보를 저장할 변수

    [Header("Slope Detection")]
    [SerializeField] private float slopeRaycastDistance = 1.5f;
    private float currentSlopeAngle;

    public void Init()
    {
        uiManager = GameManager.Instance.GetUIManager();

        characterData = GetComponent<Character>();
        playerRB = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();
        playerAnimator = GetComponent<Animator>();

        if (characterData == null) Debug.LogError("Character data component not found!");
        inputManager = GameManager.Instance.GetInputManager();

        playerRB.freezeRotation = true;
        playerRB.interpolation = RigidbodyInterpolation.Interpolate;
        playerRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Debug.Log("CharacterController Initialized.");
    }

    void Update()
    {


        HandleInput();
        UpdateAnimator();
        GroundCheck();
    }

    void FixedUpdate()
    {

        CalculateSlopeAngle();
        PerformMovement();
        PerformRotation();
    }

    private void HandleInput()
    {
        Vector3 moveInput = new Vector3(inputManager.HorizontalInputValue, 0, inputManager.VerticalInputValue).normalized;
        bool isRunning = inputManager.IsKeyPressed(KeyCode.LeftShift) && characterData.CurrentStamina > characterData.CalcStaminaCost("run", UnityEngine.Time.fixedDeltaTime);
        float currentSpeedMultiplier = isRunning ? 1.5f : 1.0f;

        characterData.Move(moveInput, currentSpeedMultiplier, isGrounded); // Character 데이터에 이동 의도 전달

        if (inputManager.JumpTriggered && isGrounded && characterData.CanPerformAction(characterData.CalcStaminaCost("jump")))
        {
            characterData.Jump(); // Character 데이터가 스태미나 처리
            ApplyJumpForce();
            playerAnimator.SetBool(ANIM_PARAM_JUMP, true);
        }

        if (inputManager.IsKeyDown(KeyCode.E))
        {
            characterData.TakeAction("interact");
        }

        if (isGrounded)
        {
            playerAnimator.SetBool(ANIM_PARAM_JUMP, false);
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
        //// BoxCollider의 경우 center와 extents를 고려하여 발밑 위치 계산
        ////Vector3 rayStartPoint = transform.position + playerCollider.center + new Vector3(0, -playerCollider.bounds.extents.y + 0.1f, 0) + groundCheckOffset;
        ////Vector3 rayStartPoint = transform.position + playerCollider.center + new Vector3(0, 0f, 0) + groundCheckOffset;
        ////Vector3 rayStartPoint = transform.position;

        //// BoxCollider의 발밑 위치 계산
        ////Vector3 rayStartPoint = transform.position + playerCollider.center + new Vector3(0, -playerCollider.size.y * 0.5f + 0.1f, 0);
        //Vector3 rayStartPoint = transform.position + playerCollider.center + new Vector3(0, -playerCollider.size.y * 0.5f + 0.005f, 0);

        ////if (playerRB.linearVelocity.y > 0.1f)
        ////{
        ////    isGrounded = false;
        ////}
        ////else
        ////{
        ////}
        //isGrounded = Physics.Raycast(rayStartPoint, Vector3.down, groundCheckDistance, groundLayerMask);


        //Debug.DrawRay(rayStartPoint, Vector3.down * groundCheckDistance, isGrounded ? Color.blue : Color.red);


        // SphereCast 시작 위치 계산:
        // 캐릭터 콜라이더의 중심에서 시작하되, 구의 반지름만큼 위로 올려서 시작해야
        // 구의 가장 아랫부분이 콜라이더의 중심에 오게 됩니다.
        // 우리는 구의 가장 아랫부분이 발바닥에 오도록 해야 하므로, 
        // (중심 - 절반높이 + 반지름) 위치가 구의 중심이 됩니다.
        // 여기서 약간(0.1f) 더 위에서 시작하여 아래로 쏩니다.
        Vector3 sphereCenterWhenGrounded = transform.position + playerCollider.center
                                         + new Vector3(0, -playerCollider.bounds.extents.y + groundCheckRadius, 0);
        Vector3 sphereStartPoint = sphereCenterWhenGrounded + Vector3.up * 0.1f;

        // 캐스트 거리 계산: 
        // 위에서 0.1f 만큼 올렸으므로, 다시 0.1f 만큼 내려오면 발바닥 위치 + groundCheckDistance 만큼 더 내려가도록.
        float castDistance = 0.1f + groundCheckDistance;

        // SphereCast 실행
        bool hitGround = Physics.SphereCast(
            sphereStartPoint,       // 시작 위치
            groundCheckRadius,      // 구의 반지름
            Vector3.down,           // 방향 (아래)
            out groundHitInfo,      // 충돌 정보 (out 키워드 사용)
            castDistance,           // 최대 거리
            groundLayerMask,        // 감지할 레이어
            QueryTriggerInteraction.Ignore // 트리거는 무시
        );

        // 디버깅용 시각화 (Scene 뷰에서만 보임)
#if UNITY_EDITOR
        Vector3 endPoint = sphereStartPoint + Vector3.down * (hitGround ? groundHitInfo.distance : castDistance);
        Color sphereColor = hitGround ? Color.green : Color.red;
        Debug.DrawLine(sphereStartPoint, endPoint, sphereColor);
        // 구를 시각화하려면 OnDrawGizmos를 사용하거나 커스텀 에디터 스크립트가 필요할 수 있습니다.
        // 간단하게 라인으로 대체합니다.
#endif

        // Y축 속도 확인 로직 (여전히 추천)
        if (playerRB.linearVelocity.y > 0.1f)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = hitGround;
        }

        // 지면에 닿았고, 경사도 계산이 필요하다면 groundHitInfo.normal 사용 가능
        if (isGrounded)
        {
            // CalculateSlopeAngle(); // 이 부분에서 groundHitInfo를 사용할 수 있음
        }
        else
        {
            // currentSlopeAngle = 0f; // 공중에서는 경사도 0
        }

    }

    //private void AdjustMovementForSlope()
    //{
    //    if (isGrounded && currentSlopeAngle > 1f && currentSlopeAngle < playerRB.slopeLimit)
    //    {
    //        RaycastHit hitInfo;
    //        // 캐릭터 발밑에서 레이캐스트
    //        Vector3 rayStart = transform.position + playerCollider.center + Vector3.up * 0.1f;
    //        if (Physics.Raycast(rayStart, Vector3.down, out hitInfo, playerCollider.bounds.extents.y + 0.3f, groundLayerMask))
    //        {
    //            Vector3 groundNormal = hitInfo.normal;
    //            // 현재 속도를 경사면에 투영
    //            Vector3 projectedVelocity = Vector3.ProjectOnPlane(playerRB.linearVelocity, groundNormal);

    //            // 아래로 내려가거나 경사면에서 움직일 때만 적용
    //            if (playerRB.linearVelocity.y < 0.1f)
    //            {
    //                playerRB.linearVelocity = projectedVelocity;
    //            }

    //            // 경사면에서 미끄러지지 않도록 아래로 약간의 힘을 가함 (움직일 때)
    //            if (currentMoveDirectionForPhysics.magnitude > 0.1f && playerRB.linearVelocity.y > -1f)
    //            {
    //                playerRB.AddForce(Vector3.down * 10f * playerRB.mass); // 질량 고려
    //            }
    //        }
    //    }
    //}

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
        // TODO: 발소리 재생 로직 (AudioManager 연동 등)
        // Example: AudioManager.Instance.PlayFootstepSound(characterData.EquippedFootwear.soleType);
    }
}