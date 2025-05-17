using UnityEngine;

public class ThirdPersonCameraWithCollision : MonoBehaviour
{
    public Transform target; // ตัวละคร
    public Vector3 offset = new Vector3(0f, 2f, -5f); // ระยะห่างกล้อง
    public float smoothSpeed = 10f;
    public float minDistance = 1f; // ระยะที่กล้องเข้าใกล้ได้มากที่สุด
    public float maxDistance = 2f; // ระยะห่างปกติ
    public LayerMask collisionMask; // กำหนดว่าอะไรที่ถือว่า "ขวาง" (เช่น Walls)

    void LateUpdate()
    {
        // คำนวณตำแหน่งปลายกล้องที่ต้องการ
        Vector3 desiredCameraPos = target.position + target.rotation * offset;

        // หาตำแหน่งกล้องที่ไม่ชนสิ่งกีดขวาง
        Vector3 direction = desiredCameraPos - target.position;
        float distance = maxDistance;

        // ยิง Ray จากตัวละคร → ไปตำแหน่งกล้องที่ต้องการ
        if (Physics.Raycast(target.position + Vector3.up * 1.5f, direction.normalized, out RaycastHit hit, maxDistance, collisionMask))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        Vector3 finalCameraPos = target.position + target.rotation * offset.normalized * distance;

        // ค่อยๆ เคลื่อนกล้องอย่าง smooth
        transform.position = Vector3.Lerp(transform.position, finalCameraPos, smoothSpeed * Time.deltaTime);

        // มองที่ตัวละคร
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
