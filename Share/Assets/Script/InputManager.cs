using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string JUMP = "Jump";
    private const string MOUSE_X = "Mouse X";
    private const string MOUSE_Y = "Mouse Y";

    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private float mouseXInput;
    [SerializeField] private float mouseYInput;
    [SerializeField] private bool jumpInput;

    public float HorizontalInputValue => horizontalInput;
    public float VerticalInputValue => verticalInput;
    public float MouseXValue => mouseXInput;
    public float MouseYValue => mouseYInput;
    public bool JumpTriggered => jumpInput;


    private void Awake()
    {
        Init();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Init()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        mouseXInput = 0f;
        mouseYInput = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {

    }

    public void ProcessInput()
    {

        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        jumpInput = Input.GetButtonDown(JUMP);
        mouseXInput = Input.GetAxis(MOUSE_X);
        mouseYInput = Input.GetAxis(MOUSE_Y);
    }


    public bool IsKeyPressed(KeyCode keyCode) => Input.GetKey(keyCode);
    public bool IsKeyDown(KeyCode keyCode) => Input.GetKeyDown(keyCode);
    public Vector2 GetMouseCoord() => Input.mousePosition;
    public bool IsMouseButtonPressed(int button) => Input.GetMouseButton(button);
    public bool IsMouseButtonDown(int button) => Input.GetMouseButtonDown(button);
}
