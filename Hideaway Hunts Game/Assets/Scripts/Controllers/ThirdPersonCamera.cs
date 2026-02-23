using UnityEngine;

public class ThirdPersonCameraWithCollision : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 1.7f, -4.5f);

    [Header("Follow")]
    public float followSmooth = 10f;
    public float rotateSmooth = 10f;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public float minDistance = 0.6f;
    public float maxDistance = 5f;
    public LayerMask collisionMask;

    private float yaw;
    private float pitch;

    private Vector3 currentTargetPos;
    public LayerMask visibleLayers;

    // =========================
    // Public API
    // =========================

     public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // รีเซ็ตให้เห็นทุกอย่างก่อน
        Camera.main.cullingMask = visibleLayers;

        // ❌ ซ่อน player ที่กำลังเล่นอยู่
        int targetLayer = newTarget.gameObject.layer;
        Camera.main.cullingMask &= ~(1 << targetLayer);
    }

    // =========================
    // Unity
    // =========================

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            yaw = target.eulerAngles.y;
            currentTargetPos = target.position + Vector3.up * offset.y;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleMouse();
        FollowTarget();
    }

    // =========================
    // Mouse Look
    // =========================

    void HandleMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    // =========================
    // Follow + Collision
    // =========================

    void FollowTarget()
    {
        // smooth target position (no head jitter)
        Vector3 wantedTargetPos = target.position + Vector3.up * offset.y;
        currentTargetPos = Vector3.Lerp(
            currentTargetPos,
            wantedTargetPos,
            followSmooth * Time.deltaTime
        );

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, offset.z);
        Vector3 desiredPos = currentTargetPos + desiredOffset;

        // collision check
        Vector3 dir = desiredPos - currentTargetPos;
        float distance = Mathf.Abs(offset.z);

        if (Physics.SphereCast(
            currentTargetPos,
            collisionRadius,
            dir.normalized,
            out RaycastHit hit,
            distance,
            collisionMask
        ))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        Vector3 finalPos = currentTargetPos + dir.normalized * distance;

        transform.position = Vector3.Lerp(
            transform.position,
            finalPos,
            followSmooth * Time.deltaTime
        );

        transform.LookAt(currentTargetPos);
    }
}
