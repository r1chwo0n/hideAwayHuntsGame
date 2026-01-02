using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;

    [Header("Cooldown")]
    public float MaxCooldown = 0.5f;   // ยิงได้ทุก 0.5 วิ
    public float CurrentCooldown = 0f;

    public bool CanShoot => CurrentCooldown <= 0f;

    void Update()
    {
        // ลด cooldown ลงเรื่อย ๆ
        if (CurrentCooldown > 0f)
            CurrentCooldown -= Time.deltaTime;

        // ตัวอย่าง: Player input
        if (Input.GetButtonDown("Fire1") && CanShoot)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (!CanShoot) return;

        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0));

        Vector3 targetPoint = ray.GetPoint(100f);
        Vector3 direction =
            (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(direction * bulletForce, ForceMode.Impulse);

        // reset cooldown
        CurrentCooldown = MaxCooldown;
    }
}
