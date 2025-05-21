using UnityEngine;

public class GunShooter : MonoBehaviour
{
    public float shootRange = 100f;
    public float shootDamage = 20f;
    public Camera playerCamera;
    //public ParticleSystem muzzleFlash;
    //public GameObject hitEffectPrefab;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // เล่น muzzle flash ถ้ามี
        //if (muzzleFlash != null)
        //    muzzleFlash.Play();

        // Ray ยิงจากตรงกลางกล้อง
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange))
        {
            Debug.Log("ยิงโดน: " + hit.collider.name);

            // ถ้ามี hit effect prefab ให้แสดงตรงที่โดน
            //if (hitEffectPrefab != null)
            //{
            //    Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            //}

            // ถ้าศัตรูมี health component
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(shootDamage);
            }
        }
    }
}
