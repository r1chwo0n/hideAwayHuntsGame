using UnityEngine;

public class GunShooter : MonoBehaviour
{
    [Header("Gun Settings")]
    public float shootRange = 100f;
    public float fireCooldown = 0.5f;
    public Transform firePoint;

    [Header("Runtime")]
    public Transform target;   // ถูก set จาก ActionExecutor

    float lastFireTime = -999f;

    void Awake()
    {
        if (!firePoint)
            firePoint = transform;
    }

    public void Fire()
    {
        if (Time.time < lastFireTime + fireCooldown)
            return;

        lastFireTime = Time.time;

        ShootRay();
    }

    void ShootRay()
    {
        Vector3 origin = firePoint.position;
        Vector3 direction;

        // 🔹 ถ้ามี target → ยิงไปที่ target
        if (target)
            direction = (target.position - origin).normalized;
        else
            direction = firePoint.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.Log($"GunShooter: Hit {hit.transform.name}");

            HandleHit(hit.transform);
        }

        Debug.DrawRay(origin, direction * shootRange, Color.red, 0.5f);
    }

    void HandleHit(Transform hit)
    {
        // 🔹 ถ้าโดน PlayerForm
        PlayerForm form = hit.GetComponent<PlayerForm>();
        if (form)
        {
            form.OnShot();
            return;
        }

        // 🔹 ถ้าโดนอย่างอื่น (ฉาก / กำแพง)
        // ทำ effect เพิ่มได้
    }
}
