//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public struct SituationSummary
//{
//    public int enemyCountInRange; // ศัตรูใกล้เรากี่ร่าง
//    public int usSeeingEnemies; // เรามองเห็นศัตรูกี่ร่าง
//    public int enemiesSeeingUs; // ศัตรูกี่ร่างที่เห็นเรา
//    public float avgEnemyDistance; // ศัตรูโดยรวมอยู่ใกล้หรือไกลเราแค่ไหน
//    public float nearestEnemyDistance; // ศัตรูที่ใกล้เราที่สุดอยู่ไกลแค่ไหน
//    //public bool underHeavyThreat; // เรากำลังถูกคุกคามอย่างหนักหรือไม่
//}


//public class WorldPerception : MonoBehaviour
//{
//    public List<Transform> enemies;

//    public SituationSummary SenseWorld(PerceptionController perception)
//    {
//        SituationSummary s = new SituationSummary();

//        if (perception == null || perception.origin == null || enemies == null)
//            return s;

//        float totalDist = 0f;
//        float nearest = float.MaxValue;

//        int countInRange = 0;
//        int countSeeingUs = 0;
//        int countUsSeeingEnemies = 0;

//        foreach (var enemy in enemies)
//        {
//            if (!enemy) continue;

//            EnemyPerception p = perception.SenseEnemy(enemy);

//            if (!p.inRange)
//                continue;

//            countInRange++;
//            totalDist += p.distance3D;

//            if (p.distance3D < nearest)
//                nearest = p.distance3D;

//            if (p.enemyCanSeeMe)
//                countSeeingUs++;

//            if (p.lineOfSight)
//                countUsSeeingEnemies++;
//        }

//        s.enemyCountInRange = countInRange;
//        s.usSeeingEnemies = countUsSeeingEnemies;
//        s.enemiesSeeingUs = countSeeingUs;

//        // 0–1 (เข้ากับ fuzzy)
//        s.avgEnemyDistance = countInRange > 0
//            ? Mathf.Clamp01((totalDist / countInRange) / perception.detectRadius)
//            : 1f;

//        // ระยะจริง (FormFuzzyAISetup ใช้ absolute)
//        //s.nearestEnemyDistance = nearest;
//        //s.nearestEnemyDistance =
//        //    countInRange > 0 ? nearest : perception.detectRadius * 1.2f;
//        float maxNearest = perception.detectRadius * 1.2f;

//        if (countInRange > 0)
//        {
//            nearest = Mathf.Clamp(nearest, 0f, maxNearest);
//        }

//        s.nearestEnemyDistance =
//            countInRange > 0 ? nearest : maxNearest;

//        return s;
//    }

//    public List<Transform> GetVisibleEnemies(PerceptionController perception)
//    {
//        List<Transform> visible = new();

//        if (perception == null || perception.origin == null || enemies == null)
//            return visible;

//        foreach (var enemy in enemies)
//        {
//            if (!enemy) continue;

//            EnemyPerception p = perception.SenseEnemy(enemy);

//            if (p.inRange && p.lineOfSight)
//            {
//                visible.Add(enemy);
//            }
//        }

//        return visible;
//    }

//}


using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SituationSummary
{
    public int enemyCountInRange;
    public int usSeeingEnemies;
    public int enemiesSeeingUs;
    public float avgEnemyDistance;
    public float nearestEnemyDistance;
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

        var hp = enemy.GetComponent<Health>();
        if (hp != null)
        {
            hp.OnDeath += RemoveEnemy;
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

        float totalDist = 0f;
        float nearest = float.MaxValue;

        int countInRange = 0;
        int countSeeingUs = 0;
        int countUsSeeingEnemies = 0;

        // ใช้ ToArray() เพื่อความปลอดภัยกรณี enemy ถูก remove ระหว่าง loop
        foreach (var enemy in enemies.ToArray())
        {
            if (!enemy) continue;

            EnemyPerception p = perception.SenseEnemy(enemy);

            if (!p.inRange)
                continue;

            countInRange++;
            totalDist += p.distance3D;

            if (p.distance3D < nearest)
                nearest = p.distance3D;

            if (p.enemyCanSeeMe)
                countSeeingUs++;

            if (p.lineOfSight)
                countUsSeeingEnemies++;
        }

        s.enemyCountInRange = countInRange;
        s.usSeeingEnemies = countUsSeeingEnemies;
        s.enemiesSeeingUs = countSeeingUs;

        // avg distance (0–1)
        s.avgEnemyDistance = countInRange > 0
            ? Mathf.Clamp01((totalDist / countInRange) / perception.detectRadius)
            : 1f;

        // nearest enemy (absolute)
        float maxNearest = perception.detectRadius * 1.2f;

        if (countInRange > 0)
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
