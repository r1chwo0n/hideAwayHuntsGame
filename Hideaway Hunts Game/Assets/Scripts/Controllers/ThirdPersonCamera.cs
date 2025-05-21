using UnityEngine;

public class ThirdPersonCameraWithCollision : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // ตัวละครที่กล้องจะติดตาม

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 1.5f, -2f); // ตำแหน่งเริ่มต้นของกล้องเมื่อไม่มีการชน
    public float smoothSpeed = 10f; // ความเร็วในการเคลื่อนที่ของกล้อง
    public float mouseSensitivity = 2f; // ความไวของเมาส์

    [Header("Rotation Limits")]
    public float minPitch = -30f; // มุมก้มต่ำสุด
    public float maxPitch = 60f;  // มุมเงยสูงสุด

    [Header("Collision Settings")]
    public float collisionRadius = 0.3f; // รัศมีของ SphereCast
    public float minDistance = 0.5f; // ระยะห่างขั้นต่ำระหว่างกล้องกับตัวละคร
    public float maxDistance = 2f;   // ระยะห่างสูงสุดระหว่างกล้องกับตัวละคร
    public LayerMask collisionMask;  // เลเยอร์ที่ใช้ตรวจจับการชน

    private float pitch = 0f; // มุมเงย/ก้มของกล้อง

    void LateUpdate()
    {
        if (target == null)
            return;

        // รับค่าการเคลื่อนไหวของเมาส์
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // คำนวณการหมุนของกล้อง
        Quaternion rotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        Vector3 desiredCameraPos = target.position + rotation * offset;

        // จุดเริ่มต้นของการตรวจจับการชน
        Vector3 rayOrigin = target.position + Vector3.up * 1.5f;
        Vector3 direction = desiredCameraPos - rayOrigin;
        float distance = direction.magnitude;

        // ตรวจจับการชนด้วย SphereCast
        if (Physics.SphereCast(rayOrigin, collisionRadius, direction.normalized, out RaycastHit hit, distance, collisionMask))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        // คำนวณตำแหน่งสุดท้ายของกล้อง
        Vector3 finalCameraPos = rayOrigin + direction.normalized * distance;

        // เคลื่อนที่กล้องอย่างนุ่มนวล
        transform.position = Vector3.Lerp(transform.position, finalCameraPos, smoothSpeed * Time.deltaTime);

        // ให้กล้องมองไปที่ตัวละคร
        transform.LookAt(rayOrigin);
    }
}
 