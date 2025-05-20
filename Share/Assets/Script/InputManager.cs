using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string JUMP = "Jump";
    private const string MOUSE_X = "Mouse X";
    private const string MOUSE_Y = "Mouse Y";

    private float horizontalInput;
    private float verticalInput;
    private float mouseXInput;
    private float mouseYInput;
    private bool jumpInput;

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

    }

    private void FixedUpdate()
    {
        ProcessInput();
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


    public bool IsKeyPressed(KeyCode keyCode)
    {
        return Input.GetKey(keyCode);
    }

    public bool IsKeyDown(KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode);
    }

    public Vector2 GetMouseCoord()
    {
        return Input.mousePosition;
    }

    public bool IsMouseButtonPressed(int button)
    {
        return Input.GetMouseButton(button);
    }

    public bool IsMouseButtonDown(int button)
    {
        return Input.GetMouseButtonDown(button);
    }
}
