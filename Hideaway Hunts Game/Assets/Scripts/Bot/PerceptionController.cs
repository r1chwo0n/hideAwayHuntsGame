using UnityEngine;
// ร่างที่ active อยู่เพียง 1 ร่าง
[System.Serializable]
public struct EnemyPerception
{
    public Transform enemy;
    public float distance;
    public bool inRange;
    public bool lineOfSight;
    public bool enemyCanSeeMe;
    public float relativeAngle;
}

public class PerceptionController : MonoBehaviour
{
    public Transform origin;
    public float detectRadius = 50f;

    public EnemyPerception SenseEnemy(Transform enemy)
    {
        EnemyPerception p = new EnemyPerception();
        p.enemy = enemy;

        Vector3 dir = enemy.position - origin.position;
        p.distance = dir.magnitude;
        p.inRange = p.distance <= detectRadius;

        p.lineOfSight = HasLineOfSight(enemy);
        p.enemyCanSeeMe = EnemyHasLineOfSight(enemy);
        p.relativeAngle = GetRelativeAngle(enemy);

        return p;
    }

    private bool HasLineOfSight(Transform enemy)
    {
        if (!enemy || !origin)
            return false;

        Vector3 myEye = origin.position + Vector3.up * 1.6f;
        Vector3 enemyEye = enemy.position + Vector3.up * 1.6f;

        Vector3 dir = enemyEye - myEye;
        float dist = dir.magnitude;
        dir.Normalize();

        if (Physics.Raycast(myEye, dir, out RaycastHit hit, dist))
            return hit.transform == enemy;

        return false;
    }


    bool EnemyHasLineOfSight(Transform enemy)
    {
        if (!enemy || !origin)
            return false;

        Vector3 enemyEye = enemy.position + Vector3.up * 1.6f;   // ปรับตามความสูงตัวละคร
        Vector3 myBody = origin.position + Vector3.up * 1.6f;

        Vector3 dir = myBody - enemyEye;
        float dist = dir.magnitude;
        dir.Normalize();

        if (Physics.Raycast(enemyEye, dir, out RaycastHit hit, dist))
        {
            return hit.transform == origin;
        }

        return false;
    }

    float GetRelativeAngle(Transform enemy)
    {
        if (!enemy || !origin)
            return 1f;

        Vector3 toEnemy = (enemy.position - origin.position).normalized; // ชี้จากเราไปศัตรู
        Vector3 forward = origin.forward; // ที่ที่เราหันหน้าอยู่

        float dot = Vector3.Dot(forward, toEnemy); // บอกว่าเวกเตอร์ 2 ตัวชี้ไปทางเดียวกัน 1 = อยู่หน้าเรา -1 = อยู่หลังเรา 0 = อยู่ข้างๆ
        dot = Mathf.Clamp(dot, -1f, 1f);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // แปลงเป็นองศา 0–180

        return angle / 180f; // normalize 0–1
    }


}
