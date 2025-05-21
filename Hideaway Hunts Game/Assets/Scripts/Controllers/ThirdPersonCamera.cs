using UnityEngine;

public class ThirdPersonCameraWithCollision : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -0.1f);
    public float smoothSpeed = 10f;
    public float minDistance = 0f;
    public float maxDistance = 2f;
    public LayerMask collisionMask;
    RaycastHit hit;

    void LateUpdate()
{
    // จุดเริ่มต้นของ Ray → ตำแหน่ง Player (เราขยับขึ้นให้พ้นพื้น)
    Vector3 rayOrigin = target.position + Vector3.up * 1.5f;

    // จุดที่อยากให้กล้องไปอยู่ → เอา offset ไปคูณกับ rotation
    Vector3 desiredCameraPos = target.position + target.rotation * offset;

    // ทิศทางจาก player ไปตำแหน่งกล้อง
    Vector3 direction = desiredCameraPos - rayOrigin;

    RaycastHit hit;

        // ยิง Ray เพื่อเช็กว่ามีอะไรมาขวางไหม
        if (Physics.Raycast(rayOrigin, direction.normalized, out hit, direction.magnitude, collisionMask))
        {
            // 🎯 ถ้ามีของบัง → เส้นเขียว จาก player ไปจุดที่ชน
            Debug.DrawLine(rayOrigin, hit.point, Color.green);
            // 🎯 ถ้ามีของบัง → เส้นแดง จาก player ไปจุดที่อยากให้กล้องอยู    
            Debug.Log("Hit: " + hit.collider.name);
    }
        else
        {
            // ❌ ไม่มีอะไรบัง → เส้นแดง จาก player ไปจุดที่อยากให้กล้องอยู่
            Debug.DrawLine(rayOrigin, desiredCameraPos, Color.red);
        }

    // แก้ระยะตาม ray hit
    float distance = maxDistance;
    if (Physics.Raycast(rayOrigin, direction.normalized, out hit, direction.magnitude, collisionMask))
    {
        distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
    }

    // ตำแหน่งกล้องสุดท้าย
    Vector3 finalCameraPos = rayOrigin + direction.normalized * distance;

    // กล้องค่อยๆ เคลื่อน
    transform.position = Vector3.Lerp(transform.position, finalCameraPos, smoothSpeed * Time.deltaTime);

    // กล้องมองที่ตัวละคร
    transform.LookAt(target.position + Vector3.up * 1.5f);
}

}
