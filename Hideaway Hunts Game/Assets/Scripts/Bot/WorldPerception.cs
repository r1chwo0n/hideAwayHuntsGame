using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SituationSummary
{
    public int enemyCountInRange; // ศัตรูใกล้เรากี่ร่าง
    public int usSeeingEnemies; // เรามองเห็นศัตรูกี่ร่าง
    public int enemiesSeeingUs; // ศัตรูกี่ร่างที่เห็นเรา
    public float avgEnemyDistance; // ศัตรูโดยรวมอยู่ใกล้หรือไกลเราแค่ไหน
    public float nearestEnemyDistance; // ศัตรูที่ใกล้เราที่สุดอยู่ไกลแค่ไหน
    //public bool underHeavyThreat; // เรากำลังถูกคุกคามอย่างหนักหรือไม่
}


public class WorldPerception : MonoBehaviour
{
    public PerceptionController perception;   // perception ของร่าง active
    public List<Transform> enemies; // ศัตรูทั้งหมด

    public SituationSummary SenseWorld()
    {
        SituationSummary s = new SituationSummary();

        if (perception == null || perception.origin == null || enemies == null)
            return s;

        float totalDist = 0f;
        float nearest = float.MaxValue;

        int countInRange = 0;
        int countSeeingUs = 0;
        int countUsSeeingEnemies = 0;

        foreach (var enemy in enemies)
        {
            if (!enemy) continue;

            EnemyPerception p = perception.SenseEnemy(enemy);

            if (p.distance < nearest)
                nearest = p.distance;

            if (p.inRange)
            {
                countInRange++;
                totalDist += p.distance;

                if (p.enemyCanSeeMe)
                    countSeeingUs++;

                if (p.lineOfSight)
                    countUsSeeingEnemies++;
            }
        }

        s.enemyCountInRange = countInRange;
        s.usSeeingEnemies = countUsSeeingEnemies;
        s.enemiesSeeingUs = countSeeingUs;
        //s.avgEnemyDistance = countInRange > 0 ? totalDist / countInRange : perception.detectRadius;
        s.avgEnemyDistance = countInRange > 0
            ? (totalDist / countInRange) / perception.detectRadius
            : 1f;

        //s.nearestEnemyDistance = countInRange > 0 ? nearest : perception.detectRadius;
        s.nearestEnemyDistance = nearest;

        //s.nearestEnemyDistance = nearest / perception.detectRadius;


        // threat heuristic (ยังไม่ใช่ decision)
        //s.underHeavyThreat = countSeeingUs >= 2;

        return s;
    }

}
