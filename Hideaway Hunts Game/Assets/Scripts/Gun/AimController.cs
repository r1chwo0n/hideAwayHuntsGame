using UnityEngine;

public class AimController : MonoBehaviour
{
    [Header("References")]
    public Transform body;          // ตัว AI (หมุนทั้งตัว)
    public Transform gunPivot;      // จุดหมุนปืน / แขน
    public Transform muzzle;        // ปากกระบอก
    public Transform target;        // เป้าหมาย (player)

    [Header("Aim Settings")]
    public float bodyTurnSpeed = 5f;
    public float gunTurnSpeed = 10f;
    public float aimTolerance = 4f; // องศาที่ถือว่าเล็งตรง

    [Header("Debug")]
    public bool isAimed;

    void Update()
    {
        if (target == null) return;

        AimBody();
        AimGun();
        CheckAim();
    }

    void AimBody()
    {
        Vector3 dir = target.position - body.position;
        dir.y = 0f; // หมุนเฉพาะแกน Y

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        body.rotation = Quaternion.Slerp(
            body.rotation,
            rot,
            Time.deltaTime * bodyTurnSpeed
        );
    }

    void AimGun()
    {
        Vector3 dir = target.position - gunPivot.position;

        Quaternion rot = Quaternion.LookRotation(dir);
        gunPivot.rotation = Quaternion.Slerp(
            gunPivot.rotation,
            rot,
            Time.deltaTime * gunTurnSpeed
        );
    }

    void CheckAim()
    {
        Vector3 dir = target.position - muzzle.position;
        float angle = Vector3.Angle(muzzle.forward, dir);
        isAimed = angle <= aimTolerance;
    }

    // ใช้เรียกจาก AI
    public bool IsAimed()
    {
        return isAimed;
    }
}
