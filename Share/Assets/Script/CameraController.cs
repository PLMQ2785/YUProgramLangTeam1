using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    public Transform cameraAnchorPoint;
    public Transform cameraFollowTarget;

    [Header("References")]
    [SerializeField] private InputManager inputManager; // GameManager 통해 할당받거나 인스펙터에서 직접 할당

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float minYAngle = -70f;
    [SerializeField] private float maxYAngle = 70f;
    [SerializeField] private float followSpeed = 15f;

    private Camera mainCamera;
    private float currentXAngle = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogError("Main Camera not found!");

        // InputManager 참조 설정 (GameManager가 있다면 GameManager를 통해 받는 것이 좋음)
        if (inputManager == null)
        {
            if (GameManager.Instance != null) inputManager = GameManager.Instance.GetInputManager();
            if (inputManager == null) Debug.LogError("InputManager not assigned to CameraController!");
        }

        if (cameraAnchorPoint == null) Debug.LogError("Camera Anchor Point not assigned!");
        if (cameraFollowTarget == null) Debug.LogError("Camera Follow Target not assigned!");

        // 초기 카메라 각도 설정
        // currentXAngle = cameraAnchorPoint.localEulerAngles.x;
        //Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 숨김
        //Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (inputManager == null || mainCamera == null || cameraAnchorPoint == null || cameraFollowTarget == null) return;

        HandleRotation();
        HandlePosition();
    }

    private void HandleRotation()
    {
        float mouseYInput = inputManager.MouseYValue;
        currentXAngle -= mouseYInput * rotationSpeed * UnityEngine.Time.deltaTime;
        currentXAngle = Mathf.Clamp(currentXAngle, minYAngle, maxYAngle);

        // 카메라 앵커(보통 플레이어 몸통의 일부 또는 카메라 전용 회전축)의 X축 회전으로 상하 시점 조절
        // 플레이어 캐릭터의 Y축 회전(좌우 회전)은 CharacterController에서 담당하고, cameraAnchorPoint는 그 자식으로 따라감
        cameraAnchorPoint.localEulerAngles = new Vector3(currentXAngle, cameraAnchorPoint.localEulerAngles.y, 0);
    }

    private void HandlePosition()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraFollowTarget.position, followSpeed * UnityEngine.Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraAnchorPoint.rotation, followSpeed * UnityEngine.Time.deltaTime);
    }


}
