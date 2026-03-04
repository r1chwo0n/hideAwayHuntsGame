using UnityEngine;

public class GunShooter : MonoBehaviour
{
    [Header("Gun Settings")]
    public float shootRange = 20f;
    //public float fireCooldown = 0.5f;
    public Transform firePoint;
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Ammo")]
    public int maxAmmo = 10;
    public int currentAmmo = 10;

    [Header("Runtime")]
    public Transform target;

    //float lastFireTime = -999f;

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

        if (currentAmmo <= 0)
            return false;

       

        return true;
    }


    public void Fire()
    {
        if (!CanFire())
            return;

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound);
            
        currentAmmo--;

        ShootRay();
    }

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
    }

    void HandleHit(Transform hit)
    {

        Debug.Log("in Handle Hit");
        hit.GetComponent<Killable>()?.TakeHit();
    }
}
