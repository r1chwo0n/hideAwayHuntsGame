using UnityEngine;

public class ThirdPersonCameraWithCollision : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 1.5f, -2f);
    public float smoothSpeed = 10f;
    public float mouseSensitivity = 2f;

    [Header("Rotation Limits")]
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("Collision Settings")]
    public float collisionRadius = 0.3f;
    public float minDistance = 0.5f;
    public float maxDistance = 2f;
    public LayerMask collisionMask;

    private float pitch = 0f;
    private float yaw = 0f; // NEW: horizontal rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // NEW: Handle mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Apply rotation to camera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredCameraPos = target.position + rotation * offset;

        // Collision check
        Vector3 rayOrigin = target.position + Vector3.up * 1.5f;
        Vector3 direction = desiredCameraPos - rayOrigin;
        float distance = direction.magnitude;

        if (Physics.SphereCast(rayOrigin, collisionRadius, direction.normalized, out RaycastHit hit, distance, collisionMask))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        Vector3 finalCameraPos = rayOrigin + direction.normalized * distance;

        transform.position = Vector3.Lerp(transform.position, finalCameraPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(rayOrigin);
    }
}
