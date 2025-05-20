using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    public Transform cameraAnchorPoint;
    public Transform cameraFollowTarget;

    [Header("References")]
    [SerializeField] private InputManager inputManager; // GameManager ���� �Ҵ�ްų� �ν����Ϳ��� ���� �Ҵ�

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

        // InputManager ���� ���� (GameManager�� �ִٸ� GameManager�� ���� �޴� ���� ����)
        if (inputManager == null)
        {
            if (GameManager.Instance != null) inputManager = GameManager.Instance.GetInputManager();
            if (inputManager == null) Debug.LogError("InputManager not assigned to CameraController!");
        }

        if (cameraAnchorPoint == null) Debug.LogError("Camera Anchor Point not assigned!");
        if (cameraFollowTarget == null) Debug.LogError("Camera Follow Target not assigned!");

        // �ʱ� ī�޶� ���� ����
        // currentXAngle = cameraAnchorPoint.localEulerAngles.x;
        //Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ����
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

        // ī�޶� ��Ŀ(���� �÷��̾� ������ �Ϻ� �Ǵ� ī�޶� ���� ȸ����)�� X�� ȸ������ ���� ���� ����
        // �÷��̾� ĳ������ Y�� ȸ��(�¿� ȸ��)�� CharacterController���� ����ϰ�, cameraAnchorPoint�� �� �ڽ����� ����
        cameraAnchorPoint.localEulerAngles = new Vector3(currentXAngle, cameraAnchorPoint.localEulerAngles.y, 0);
    }

    private void HandlePosition()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraFollowTarget.position, followSpeed * UnityEngine.Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraAnchorPoint.rotation, followSpeed * UnityEngine.Time.deltaTime);
    }


}
