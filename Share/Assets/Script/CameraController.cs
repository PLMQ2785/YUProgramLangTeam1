using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class CameraController : MonoBehaviour
{
    public GameObject cameraAnchorPoint;
    public GameObject cameraFollowPosition;

    private Camera mainCamera;
    private InputHandler inputHandler;
    private Transform playerTransform;

    private CharacterController playerController;

    private float mouseXValue = 0f;
    private float mouseYValue = 0f;

    private float rotationSpeed = 100f; //Camera rotation speed


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
    }

    void init()
    {
        mainCamera = GameObject.FindFirstObjectByType<Camera>();
        inputHandler = GetComponent<InputHandler>(); playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        playerCamMove();
    }

    private void playerCamMove()
    {
        mainCameraPositionUpdate();
        mainCameraRotation();
    }

    private void mainCameraPositionUpdate()
    {
        mainCamera.transform.position = cameraFollowPosition.transform.position;
        mainCamera.transform.forward = cameraAnchorPoint.transform.forward;
    }

    private void mainCameraRotation()
    {
        float mouseXInput = playerController.getPlayerRotationInput;
        float mouseYInput = -inputHandler.getMouseYInput * rotationSpeed * Time.deltaTime;

        //cameraAnchorPoint.transform.forward = playerTransform.forward;
        //cameraFollowPosition.transform.forward = playerTransform.forward;

        //Debug.Log("mouseY:" + mouseYInput);

        //mouseYValue += mouseYInput * rotationSpeed * Time.deltaTime;
        //mouseYValue = Mathf.Clamp(mouseYValue, -70f, 70f); //Lock up/down rot

        mouseYValue += mouseYInput;
        mouseYValue = Mathf.Clamp(mouseYValue, -70f, 70f); //Lock up/down rot

        //Camera.main.transform.forward = playerTransform.forward;
        Vector3 cameraAngle = Camera.main.transform.eulerAngles;
        cameraAngle.x = mouseYValue;
        Camera.main.transform.eulerAngles = cameraAngle;

        //Debug.Log("cameraXAngle:" + cameraXAngle);

    }

    private void OnFootstep(AnimationEvent animationEvent)
    {

    }


}
