using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string JUMP = "Jump";
    private const string mouseX = "Mouse X";
    private const string mouseY = "Mouse Y";

    private float horizontalInput;
    private float verticalInput;
    private float mouseXInput;
    private float mouseYInput;
    private bool jumpInput;

    public float getMouseXInput
    {
        get { return mouseXInput; }
    }
    public float getMouseYInput
    {
        get { return mouseYInput; }
    }
    public float getHorizontalInput
    {
        get { return horizontalInput; }
    }
    public float getVerticalInput
    {
        get { return verticalInput; }
    }

    public bool getJumpInput
    {
        get { return jumpInput; }
    }


    private void Awake()
    {
        valueInit();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void valueInit()
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
        GetAxisWASD();
        GetAxisMouse();
    }

    private void LateUpdate()
    {

    }

    private void GetAxisWASD()
    {
        if (Mathf.Abs(Input.GetAxis(HORIZONTAL)) > 0f)
        {
            horizontalInput = Input.GetAxis(HORIZONTAL);
        }
        else
        {
            horizontalInput = 0f;
        }

        if (Mathf.Abs(Input.GetAxis(VERTICAL)) > 0f)
        {
            verticalInput = Input.GetAxis(VERTICAL);
        }
        else
        {
            verticalInput = 0f;
        }

        if (Mathf.Abs(Input.GetAxis(JUMP)) > 0f)
        {
            jumpInput = true;
        }
        else
        {
            jumpInput = false;
        }
    }

    private void GetAxisMouse()
    {
        if (Mathf.Abs(Input.GetAxis(mouseX)) > 0f)
        {
            mouseXInput = Input.GetAxis(mouseX);
        }
        else
        {
            mouseXInput = 0f;
        }

        if (Mathf.Abs(Input.GetAxis(mouseY)) > 0f)
        {
            mouseYInput = Input.GetAxis(mouseY);
        }
        else
        {
            mouseYInput = 0f;
        }
    }
}
