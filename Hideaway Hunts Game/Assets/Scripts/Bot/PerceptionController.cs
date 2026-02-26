using UnityEngine;

[System.Serializable]
public struct EnemyPerception
{
    public Transform enemy;

    public float distance3D;
    public float horizontalDistance; // แนวนอน
    public float verticalDifference; // ต่างสูง

    public bool inRange;
    public bool lineOfSight;
    public bool enemyCanSeeMe;
    
    public float relativeAngle; // 0–1 (0 = หน้า, 1 = หลัง)

    public float movementIntensity; // [0,1]
}

public class PerceptionController : MonoBehaviour
{
    [Header("References")]
    public Transform origin;

    [Header("Sense Settings")]
    public float detectRadius = 30f;
    public float eyeHeight = 1.6f; // assume
    public LayerMask sightMask; // พวก obstacle enemy player

    public EnemyPerception SenseEnemy(Transform enemy)
    {
        EnemyPerception p = new EnemyPerception();     
        p.enemy = enemy;

        Vector3 delta = enemy.position - origin.position;

        // --- Distance ---
        p.distance3D = delta.magnitude;
        p.verticalDifference = Mathf.Abs(delta.y);

        delta.y = 0f;
        p.horizontalDistance = delta.magnitude;

        p.inRange = p.horizontalDistance <= detectRadius;

        // --- Vision ---
        p.lineOfSight = HasLineOfSight(enemy);
        p.enemyCanSeeMe = EnemyHasLineOfSight(enemy);

        // --- Angle ---
        p.relativeAngle = GetRelativeAngle(enemy);

        return p;
    }

    public bool HasLineOfSight(Transform enemy)
    {
        if (!enemy || !origin)
            return false;

        //Vector3 myEye = origin.position + Vector3.up * eyeHeight;
        Vector3 myEye = GetEyePosition(origin);

        return CheckMultiRayLOS(myEye, enemy, sightMask);
    }

    public bool HasLineOfSightFromPosition(Vector3 fromPosition, Transform enemy)
    {
        if (!enemy)
            return false;

        Vector3 eyePos = fromPosition + Vector3.up * eyeHeight;

        return CheckMultiRayLOS(eyePos, enemy, sightMask);
    }

    bool EnemyHasLineOfSight(Transform enemy)
    {
        if (!enemy || !origin)
            return false;

        //Vector3 enemyEye = enemy.position + Vector3.up * eyeHeight;
        Vector3 enemyEye = GetEyePosition(enemy);

        return CheckMultiRayLOS(enemyEye, origin, sightMask);
    }


    // ===== Angle เฉพาะแนวราบ ศัตรูอยู่ทิศไหน
    private float GetRelativeAngle(Transform enemy)
    {
        Vector3 toEnemy = enemy.position - origin.position;
        toEnemy.y = 0f;

        Vector3 forward = origin.forward; // ทิศที่เราหันหน้าอยู่
        forward.y = 0f;

        if (toEnemy.sqrMagnitude < 0.001f)
            return 0f;

        toEnemy.Normalize();
        forward.Normalize();

        float dot = Vector3.Dot(forward, toEnemy); // 1 อยู่ตรงหน้า, 0 ข้างๆ, -1 ข้างหลัง
        dot = Mathf.Clamp(dot, -1f, 1f);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 0–180
        return angle / 180f; // normalize: 0 ข้างหน้า, 0.5 ข้างๆ, 1 ข้างหลัง
    }


    // ===== Helper Methods =====
    bool CheckMultiRayLOS(
    Vector3 fromEye,
    Transform target,
    LayerMask mask
    ) // visibility check จากเราถึงศัตรูมีอะไรกั้นมั้ย
    {
        CapsuleCollider cap = target.GetComponent<CapsuleCollider>();
        float h = cap ? cap.height : 2.0f;

        Vector3[] targetPoints =
        {
        target.position + Vector3.up * (h * 0.9f), // หัว
        target.position + Vector3.up * (h * 0.6f), // อก
        target.position + Vector3.up * (h * 0.3f)  // ขา
    };

        foreach (var point in targetPoints)
        {
            Vector3 dir = point - fromEye;
            float dist = dir.magnitude;

            if (dist < 0.05f)
                return true;

            dir.Normalize();
            // Debug.DrawRay(fromEye, dir * dist, Color.red);

            if (Physics.Raycast(fromEye, dir, out RaycastHit hit, dist, mask))
            {
                if (hit.transform == target)
                    return true;
            }
        }

        return false;
    }

    // คำนวณ eye position จาก collider
    Vector3 GetEyePosition(Transform t)
    {
        CapsuleCollider cap = t.GetComponent<CapsuleCollider>();

        if (cap == null)
            return t.position + Vector3.up * eyeHeight; // fallback

        // ตำแหน่งกลาง capsule + ครึ่งสูง - offset เล็กน้อย
        float eyeY = cap.center.y + (cap.height * 0.5f) - 0.1f;
        return t.TransformPoint(new Vector3(0f, eyeY, 0f));
    }


}