using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private GameManager _gameManager;


    private const string animParamSpeed = "Speed";

    private InputHandler inputHandler;
    private UIManager _uiManager;

    //Character Properties
    private float hp = 100f;
    private float speed = 5f;
    private float jumpforce = 5f;
    private float stamina = 100f;
    private float staminaRegen = 5f;

    //Equipment Properties
    private float durability = 100f;

    //slope
    private float slopeRaycastDistance = 1.5f; //Raycast 발사 거리
    [SerializeField] private LayerMask groundLayerMask;

    private float currentSlopeAngle;

    private Rigidbody playerRB;
    private BoxCollider playerCollider;
    private Animator playerAnimator;

    private float mouseXValue = 0f;

    public float getPlayerRotationInput
    {
        get { return mouseXValue; }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
    }

    private void init()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        inputHandler = GetComponent<InputHandler>();
        playerRB = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        playerMove();
        playerRotate();
        getSlopeAngle();
    }

    private void LateUpdate()
    {

    }

    private void playerMove()
    {
        if (inputHandler.getHorizontalInput != 0 || inputHandler.getVerticalInput != 0)
        {
            Vector3 moveDirection = new Vector3(inputHandler.getHorizontalInput, 0, inputHandler.getVerticalInput);
            moveDirection.Normalize();

            Vector3 forwardMovement = inputHandler.getVerticalInput * transform.forward * speed * Time.deltaTime;
            Vector3 sideMovement = inputHandler.getHorizontalInput * transform.right * speed * Time.deltaTime;
            Vector3 moveDistance = forwardMovement + sideMovement;

            playerRB.MovePosition(playerRB.position + moveDistance);

            if (inputHandler.getVerticalInput > 0f)
            {
                playerAnimator.SetFloat(animParamSpeed, 2f);
            }
            else if (inputHandler.getVerticalInput < 0f)
            {
                playerAnimator.SetFloat(animParamSpeed, -2f);
            }

        }
        else
        {
            playerAnimator.SetFloat(animParamSpeed, playerRB.linearVelocity.magnitude);
        }
    }

    private void playerRotate()
    {

        mouseXValue += inputHandler.getMouseXInput * 100f * Time.deltaTime;
        gameObject.transform.rotation = Quaternion.Euler(0, mouseXValue, 0);
        //Debug.Log("mouseX:" + mouseXValue);
    }

    private float getSlopeAngle()
    {
        RaycastHit hitInfo;

        Vector3 rayStartPoint = playerRB.position + Vector3.down * 0.2f; //ray Offset

        if (Physics.Raycast(rayStartPoint, Vector3.down, out hitInfo, slopeRaycastDistance, groundLayerMask))
        {
            //충돌 지점 표면 법선 벡터와 상향 벡터 각도 계산
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

            Debug.Log("Slope Angle: " + angle);


            _uiManager.SlopeAngleText = angle.ToString("F1") + "°";

            return angle;
        }

        return 0f;
    }
}
