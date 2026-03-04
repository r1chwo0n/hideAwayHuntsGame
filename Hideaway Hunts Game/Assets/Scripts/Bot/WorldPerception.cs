using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SituationSummary
{
    public int enemyCountInRange;
    public int usSeeingEnemies;
    public int enemiesSeeingUs;
    public float avgEnemyDistance; //ระยะเฉลี่ยของศัตรูทั้งหมดที่อยู่ในระยะตรวจจับ
    public float nearestEnemyDistance;
    public float maxEnemyMovementIntensity; // มีใครกำลังทำตัวนน่าสงสัยมั้ย 
    //public float nearestEnemyVerticalDiff;
    public float nearestEnemyMovementIntensity;

    public int formsRemaining;
    public int enemyFormsRemaining;
    public float ammoRatio;
}

public class WorldPerception : MonoBehaviour
{
    [Header("Enemies in world")]
    public List<Transform> enemies = new();

    // ================= SETUP =================

    void Start()
    {
        // register enemy ที่ถูกลากมาใน inspector
        foreach (var e in enemies)
            RegisterEnemy(e);
    }

    public void RegisterEnemy(Transform enemy)
    {
        if (!enemy || enemies.Contains(enemy) == false)
            return;

        var k = enemy.GetComponent<Killable>();
        if (k != null)
        {
            // ยกเลิกของเก่าก่อน (เผื่อเรียกซ้ำ) แล้วค่อยบวกเข้าใหม่
            k.OnKilled -= RemoveEnemy;
            k.OnKilled += RemoveEnemy;
        }
    }

    void RemoveEnemy(Transform deadEnemy)
    {
        if (enemies.Contains(deadEnemy))
        {
            enemies.Remove(deadEnemy);
        }
    }

    // ================= WORLD SENSE =================

    public SituationSummary SenseWorld(PerceptionController perception)
    {
        SituationSummary s = new SituationSummary();

        if (perception == null || perception.origin == null)
            return s;

        float totalDist = 0f; // ระยะรวมของศัตรูทั้งหมดที่อยู่ในระยะตรวจจับ
        float nearest = float.MaxValue;

        int countInRange = 0;
        int countSeeingUs = 0;
        int countUsSeeingEnemies = 0;
        //float maxMovement = 0f;
        float nearestMovement = 0f;

        // ใช้ ToArray() เพื่อความปลอดภัยกรณี enemy ถูก remove ระหว่าง loop
        foreach (var enemy in enemies.ToArray())
        {
            if (!enemy) continue;

            EnemyPerception p = perception.SenseEnemy(enemy);

            if (!p.inRange) // ต้องเข้าระยะตรวจจับก่อน
                continue;

            countInRange++;
            totalDist += p.distance3D;

            if (p.distance3D < nearest)
            {
                nearest = p.distance3D;
                nearestMovement = p.movementIntensity;
            }

            if (p.enemyCanSeeMe)
                countSeeingUs++;

            if (p.lineOfSight)
                countUsSeeingEnemies++;

            //if(p.movementIntensity > maxMovement)
            //    maxMovement = p.movementIntensity;
        }

        s.enemyCountInRange = countInRange;
        s.usSeeingEnemies = countUsSeeingEnemies;
        s.enemiesSeeingUs = countSeeingUs;
        //s.maxEnemyMovementIntensity = maxMovement;
        s.nearestEnemyMovementIntensity = nearestMovement;

        // avg distance (0–1)
        // ศัตรูโดยรวมอยู่ใกล้หรือไกลเราแค่ไหน เมื่อเทียบกับระยะมองเห็นสูงสุด
        // totalDist / countInRange ระยะเฉลี่ยจริง
        s.avgEnemyDistance = countInRange > 0
            ? Mathf.Clamp01((totalDist / countInRange) / perception.detectRadius)
            : 1f;

        // nearest enemy (absolute)
        float maxNearest = perception.detectRadius * 1.2f; // ไม่มีศัตรูใกล้เลย

        if (countInRange > 0)
            // Clamp(value, min, max) if value < min return min; if value > max return max; else return value
            nearest = Mathf.Clamp(nearest, 0f, maxNearest);

        s.nearestEnemyDistance =
            countInRange > 0 ? nearest : maxNearest;

        return s;
    }

    // ================= VISIBLE TARGETS =================

    public List<Transform> GetVisibleEnemies(PerceptionController perception)
    {
        List<Transform> visible = new();

        if (perception == null || perception.origin == null)
            return visible;

        foreach (var enemy in enemies.ToArray())
        {
            if (!enemy) continue;

            EnemyPerception p = perception.SenseEnemy(enemy);

            if (p.inRange && p.lineOfSight)
                visible.Add(enemy);
        }

        return visible;
    }
}
