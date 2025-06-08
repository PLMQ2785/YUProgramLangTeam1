using UnityEngine;

public class LimitGizmoDistance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        float maxDistance = 50f; // 카메라와의 최대 거리
        if (Vector3.Distance(Camera.main.transform.position, transform.position) < maxDistance)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1f); // Gizmo를 그리는 코드
        }
    }
}
