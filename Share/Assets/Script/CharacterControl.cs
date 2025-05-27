using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))] // 또는 CapsuleCollider, 근데 미끄러지는거 보정이 귀찮아서 box로 했음
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))] // Character 데이터 컴포넌트 필수
public class CharacterControl : MonoBehaviour
{
    //이름 겹쳐서 CharacterControl.cs로 변경
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
    private Vector3 groundCheckOffset = new Vector3(0, -0.0f, 0); // 콜라이더 하단 기준 조정.... 하려 했는데 필요에 따라서....
    [SerializeField] private bool isGrounded;

    [Tooltip("지면 체크 구의 반지름 (캐릭터 콜라이더 너비보다 약간 작게)")]
    [SerializeField] private float groundCheckRadius = 0.45f;
    [Tooltip("지면으로 간주할 최대 거리 (발바닥 ~ 지면)")]
    private RaycastHit groundHitInfo; // 충돌 정보를 저장할 변수 -> 단순 raycast로 하니까 다단점프 되길래 몇번 시도끝에 이걸로 하면 괜찮아서 수정

    [Header("Slope Detection")]
    [SerializeField] private float slopeRaycastDistance = 1.5f; //경사각도 계산할때 ray쏘는 거리
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

        //playerRB.freezeRotation = true; //생각해보니 딱히 필요는 없겠다 싶음..
        playerRB.interpolation = RigidbodyInterpolation.Interpolate; //물리 보간 코드로 설정(에디터로 해도 상관은 없긴함)
        playerRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //충돌 계산 모드 련속적으로

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
        Vector3 moveInput = new Vector3(inputManager.HorizontalInputValue, 0, inputManager.VerticalInputValue).normalized; //이동 입력값 get
        bool isRunning = inputManager.IsKeyPressed(KeyCode.LeftShift) && characterData.CurrentStamina > characterData.CalcStaminaCost("run", UnityEngine.Time.fixedDeltaTime); //run 입력 체크 and 스태미나 체크
        float currentSpeedMultiplier = isRunning ? 1.5f : 1.0f; //속도 배율

        characterData.Move(moveInput, currentSpeedMultiplier, isGrounded); // Character 데이터에 이동 하겠다고 넘겨주면 거기서 관련 로직 처리

        if (inputManager.JumpTriggered && isGrounded && characterData.CanPerformAction(characterData.CalcStaminaCost("jump")))
        {
            characterData.Jump(); // Character 데이터가 스태미나 처리하게 넘겨줌
            ApplyJumpForce();
            playerAnimator.SetBool(ANIM_PARAM_JUMP, true); //애니메이션 플래그
        }

        //if (inputManager.IsKeyDown(KeyCode.E))
        //{
        //    characterData.TakeAction("interact"); //일단 넣어는 놨는데..... 쓸려면 쓰세요
        //}

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

        bool isRunning = inputManager.IsKeyPressed(KeyCode.LeftShift) && characterData.CurrentStamina > 0; // 스태미나 소모는 Character.Move에서 처리하므로 거기서 찾아보세요!
        float actualSpeed = characterData.moveSpeed * (isRunning ? 1.5f : 1.0f); //걷기/달리기 속도

        Vector3 worldMoveDirection = transform.TransformDirection(moveInput); //이동할 방향
        Vector3 targetVelocity = worldMoveDirection * actualSpeed;
        targetVelocity.y = playerRB.linearVelocity.y; // y축 속도 유지

        Vector3 velocityChange = (targetVelocity - playerRB.linearVelocity);// 현재 속도와 목표 속도의 차이
        velocityChange.x = Mathf.Clamp(velocityChange.x, -10f, 10f);// x축 속도 제한
        velocityChange.z = Mathf.Clamp(velocityChange.z, -10f, 10f);// z축 속도 제한
        velocityChange.y = 0;// y축 속도는 0으로 설정 (점프시 y축 속도는 ApplyJumpForce에서 처리함)

        //Move At으로 써도 되긴 하는데... 예전에 했던 프로젝트 코드 가져오면서 그냥 미는걸로 해두었으니 참고하세요...
        //필요하면 Move At으로 바꿀수 있습니다!
        if (isGrounded)
        {
            playerRB.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        //else
        //{
        //    playerRB.AddForce(velocityChange * 0.2f, ForceMode.VelocityChange); // 공중 제어력 감소
        //}
        //AdjustMovementForSlope();
    }

    private void PerformRotation()
    {
        float mouseXDelta = inputManager.MouseXValue * rotationSpeed * UnityEngine.Time.deltaTime; //마우스 값 가져와서
        transform.Rotate(Vector3.up, mouseXDelta); //빙글빙글
    }

    private void ApplyJumpForce()
    {
        playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, 0f, playerRB.linearVelocity.z); // 점프 전 y축 속도 초기화
        playerRB.AddForce(Vector3.up * characterData.jumpForce, ForceMode.Impulse); //짬푸
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

        //다단점프 해결하기 직전에 삽질한 결과물들... ↑


        // SphereCast 시작 위치 계산:
        // 캐릭터 콜라이더의 중심에서 시작하되, 구의 반지름만큼 위로 올려서 시작해야
        // 구의 가장 아랫부분이 콜라이더의 중심에 오게 됩니다
        // 구의 가장 아랫부분이 발바닥에 오도록 해야 하므로, 
        // (중심 - 절반높이 + 반지름).
        // 약간(0.1f) 더 위에서 시작하여 아래로 쏩니다.
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

        // 디버깅용 시각화 (Scene 뷰에서만 보임), 일단 해결했으니 안써도 됨!
        //#if UNITY_EDITOR
        //        Vector3 endPoint = sphereStartPoint + Vector3.down * (hitGround ? groundHitInfo.distance : castDistance);
        //        Color sphereColor = hitGround ? Color.green : Color.red;
        //        Debug.DrawLine(sphereStartPoint, endPoint, sphereColor);
        //        // 구를 시각화하려면 OnDrawGizmos를 사용하거나 커스텀 에디터 스크립트가 필요할 수 있는데.
        //        // 간단하게 라인으로 대체합니다.
        //#endif

        // Y축 속도 확인 로직
        if (playerRB.linearVelocity.y > 0.1f)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = hitGround;
        }

        // 예전에 필요할거 같아서 넣어놨는데 지금은 딱히 필요없을거 같음
        // 지면에 닿았고, 경사도 계산이 필요하다면 groundHitInfo.normal 사용 가능
        //if (isGrounded)
        //{
        //    // CalculateSlopeAngle(); // 이 부분에서 groundHitInfo를 사용할 수 있음
        //}
        //else
        //{
        //    // currentSlopeAngle = 0f; // 공중에서는 경사도 0
        //}

    }

    //딱히 필요없을듯
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

    //경사각 계산부분
    private void CalculateSlopeAngle()
    {
        RaycastHit hitInfo; //충돌정보
        Vector3 rayStart = transform.position + playerCollider.center + Vector3.up * 0.1f; //발싸할 위치

        if (Physics.Raycast(rayStart, Vector3.down, out hitInfo, playerCollider.bounds.extents.y + 0.5f, groundLayerMask)) //발싸!
        {
            currentSlopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up); // hitInfo.normal은 지면의 법선 벡터, Vector3.up은 수직 벡터, 이 두개 벡터 각도 차이를 계산해서 반환!
        }
        else
        {
            currentSlopeAngle = 0f; //없으면?... 0
        }

        uiManager.SlopeAngleText = currentSlopeAngle.ToString("F1") + "°";

    }

    //애니메이션 업데이트
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
        // 발소리 재생 로직을 넣으려면 넣을순 있어요...
        // AudioManager.Instance.PlayFootstepSound(characterData.EquippedFootwear.soleType); 대충 이렇게?
    }
}