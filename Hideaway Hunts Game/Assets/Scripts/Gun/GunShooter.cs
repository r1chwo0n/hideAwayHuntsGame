using UnityEngine;

public class GunShooter : MonoBehaviour
{
    [Header("Gun Settings")]
    public float shootRange = 20f;
    public float fireCooldown = 0.5f;
    public Transform firePoint;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo = 30;

    [Header("Runtime")]
    public Transform target;

    float lastFireTime = -999f;

    public float AmmoRatio =>
        maxAmmo > 0 ? (float)currentAmmo / maxAmmo : 0f;

    void Awake()
    {
        if (!firePoint)
            firePoint = transform;

        currentAmmo = maxAmmo;
    }

    public bool CanFire()
    {
        if (target == null)
            return false;

        // optional
        //if (!target.gameObject.activeInHierarchy)
        //    return false;

        if (currentAmmo <= 0)
            return false;

        if (Time.time < lastFireTime + fireCooldown)
            return false;

        return true;
    }


    public void Fire()
    {
        if (!CanFire())
            return;

        lastFireTime = Time.time;
        currentAmmo--;

        ShootRay();
    }

    //void ShootRay()
    //{
    //    if (target == null)
    //        return;

    //    Vector3 origin = firePoint.position;
    //    //Vector3 direction = (target.position - origin).normalized;
    //    Vector3 direction = firePoint.forward; // ต้องหันหน้า รู้สึกว่าก็ต้องเล็งเหมือนกัน


    //    if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
    //    {
    //        Debug.Log($"GunShooter: Hit {hit.transform.name}");
    //        HandleHit(hit.transform);
    //    }

    //    Debug.DrawRay(origin, direction * shootRange, Color.red, 0.5f);
    //}

    void ShootRay()
    {
        if (target == null)
            return;

        Vector3 origin = firePoint.position;
        Vector3 direction = (target.position - origin).normalized;

        // เพิ่มความไม่แม่นยำเล็กน้อย
        direction += Random.insideUnitSphere * 0.01f;
        direction.Normalize();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.Log($"GunShooter: Hit {hit.transform.name}");
            HandleHit(hit.transform);
        }

        Debug.DrawRay(origin, direction * shootRange, Color.red, 0.5f);
    }

    void HandleHit(Transform hit)
    {
        // ถ้า hit นั้นไปโดน Transform ที่ไม่มี Killable เป็น Component ก็จะไม่เกิดอะไรขึ้น
        hit.GetComponent<Killable>()?.TakeHit();
    }
}
