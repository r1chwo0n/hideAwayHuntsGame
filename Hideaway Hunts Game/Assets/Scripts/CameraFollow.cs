using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;     // จุดที่กล้องจะตาม (เช่น Transform ของ Bot)
    public Vector3 offset = new Vector3(0, 5, -10);  // ระยะห่างกล้อง
    public float smoothSpeed = 5f; // ความลื่นในการตาม

    void LateUpdate()
    {
        if (target == null) return;

        // ตำแหน่งที่อยากให้กล้องไป
        Vector3 desiredPosition = target.position + offset;

        // Interpolate ให้กล้องเคลื่อนแบบ smooth
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // ให้กล้องหันมองเป้าหมาย
        transform.LookAt(target);
    }
}
