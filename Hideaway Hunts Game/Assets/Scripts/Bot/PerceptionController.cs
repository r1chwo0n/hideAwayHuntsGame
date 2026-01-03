using UnityEngine;

[System.Serializable]
public struct EnemyPerception
{
    public Transform enemy;

    public float distance3D;
    public float horizontalDistance; // แนวนอน
    public float verticalDifference; // แนวตั้ง

    public bool inRange;
    public bool lineOfSight;
    public bool enemyCanSeeMe;

    public float relativeAngle; // 0–1 (0 = หน้า, 1 = หลัง)
}

public class PerceptionController : MonoBehaviour
{
    [Header("References")]
    public Transform origin;

    [Header("Sense Settings")]
    public float detectRadius = 50f;
    public float eyeHeight = 1.6f;
    public LayerMask sightMask; // พวก obstacle enemy player

    public EnemyPerception SenseEnemy(Transform enemy)
    {
        EnemyPerception p = new EnemyPerception();
        p.enemy = enemy;

        if (!enemy || !origin)
            return p;

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

    // จาก ตน.ของเรา มองเห็นศัตรูโดยไม่มี obstacle บังใช่มั้ย
    // Raycast : เส้นตรงเส้นเดียวยิงออกไป
    //private bool HasLineOfSight(Transform enemy)
    //{
    //    Vector3 myEye = origin.position + Vector3.up * eyeHeight; // ยกตำแหน่งขึ้นระดับสายตา
    //    Vector3 enemyEye = enemy.position + Vector3.up * eyeHeight;

    //    Vector3 dir = enemyEye - myEye;
    //    float dist = dir.magnitude; // ระยะจริงระหว่างตาเรา vs ศัตรู
    //    dir.Normalize(); // ทิศทางจากเราไปศัตรู

    //    // ยิงจากตาไปในทิศ dir ยาวไม่เกิน dist 
    //    if (Physics.Raycast(myEye, dir, out RaycastHit hit, dist, sightMask))
    //    {
    //        return hit.transform == enemy;
    //    }

    //    return false;
    //}

    bool HasLineOfSight(Transform enemy)
    {
        if (!enemy || !origin)
            return false;

        //Vector3 myEye = origin.position + Vector3.up * eyeHeight;
        Vector3 myEye = GetEyePosition(origin);

        return CheckMultiRayLOS(myEye, enemy, sightMask);
    }


    // ===== Line of sight (ศัตรู -> เรา)
    //private bool EnemyHasLineOfSight(Transform enemy)
    //{
    //    Vector3 enemyEye = enemy.position + Vector3.up * eyeHeight;
    //    Vector3 myEye = origin.position + Vector3.up * eyeHeight;

    //    Vector3 dir = myEye - enemyEye;
    //    float dist = dir.magnitude;
    //    dir.Normalize();

    //    if (Physics.Raycast(enemyEye, dir, out RaycastHit hit, dist, sightMask))
    //    {
    //        return hit.transform == origin;
    //    }

    //    return false;
    //}

    bool EnemyHasLineOfSight(Transform enemy)
    {
        if (!enemy || !origin)
            return false;

        //Vector3 enemyEye = enemy.position + Vector3.up * eyeHeight;
        Vector3 enemyEye = GetEyePosition(enemy);

        return CheckMultiRayLOS(enemyEye, origin, sightMask);
    }


    // ===== Angle เฉพาะแนวราบ
    private float GetRelativeAngle(Transform enemy)
    {
        Vector3 toEnemy = enemy.position - origin.position;
        toEnemy.y = 0f;

        Vector3 forward = origin.forward;
        forward.y = 0f;

        if (toEnemy.sqrMagnitude < 0.001f)
            return 0f;

        toEnemy.Normalize();
        forward.Normalize();

        float dot = Vector3.Dot(forward, toEnemy);
        dot = Mathf.Clamp(dot, -1f, 1f);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // 0–180
        return angle / 180f;
    }


    // ===== Helper Methods =====
    bool CheckMultiRayLOS(
    Vector3 fromEye,
    Transform target,
    LayerMask mask
)
    {
        CapsuleCollider cap = target.GetComponent<CapsuleCollider>();
        float h = cap ? cap.height : 1.6f;

        Vector3[] targetPoints =
        {
        target.position + Vector3.up * (h * 0.9f), // head
        target.position + Vector3.up * (h * 0.6f), // chest
        target.position + Vector3.up * (h * 0.3f)  // legs
    };

        foreach (var point in targetPoints)
        {
            Vector3 dir = point - fromEye;
            float dist = dir.magnitude;

            if (dist < 0.05f)
                return true;

            dir.Normalize();
            Debug.DrawRay(fromEye, dir * dist, Color.red);

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