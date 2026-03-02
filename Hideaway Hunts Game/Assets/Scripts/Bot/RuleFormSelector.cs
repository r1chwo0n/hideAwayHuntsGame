using System.Collections.Generic;
using UnityEngine;
public class RuleFormSelector : MonoBehaviour, IFormSelector
{
    public Transform SelectBestForm(List<FormSituation> situations)
    {
        Transform best = null;
        float bestScore = float.MinValue;

        foreach (var s in situations)
        {
            var w = s.world;

            // เริ่มที่ 0.6 ตามค่า OK ของ Fuzzy
            float score = 0.6f;

            // วิสัยทัศน์: ถ้าเห็นชัด (3/3) จะบวกเพิ่มได้สูงสุด 0.2 -> (0.6 + 0.2 = 0.8)
            score += (w.usSeeingEnemies / 3f) * 0.2f;

            // ระยะห่าง: ถ้าศัตรูอยู่ไกล (36/36) จะบวกเพิ่มได้สูงสุด 0.1 -> (0.8 + 0.1 = 0.9)
            score += (w.nearestEnemyDistance / 36f) * 0.1f;

            // การกระจายตัว: ถ้ากระจายตัวดี (1.0) จะบวกเพิ่มได้สูงสุด 0.1 -> (0.9 + 0.1 = 1.0)
            score += w.avgEnemyDistance * 0.1f;

            // จำนวนศัตรู: ถ้าเยอะมาก (3/3) หักออกสูงสุด 0.2
            score -= (w.enemyCountInRange / 3f) * 0.2f;

            // ภัยคุกคาม: ถ้าโดนล้อม (3/3) หักออกสูงสุด 0.4 (เพื่อให้ลงไปแตะระดับ Bad 0.2 ได้)
            score -= (w.enemiesSeeingUs / 3f) * 0.4f;

            // 3. ใช้ Clamp01 เพื่อไม่ให้คะแนนเกิน 1.0 หรือต่ำกว่า 0.0
            score = Mathf.Clamp01(score);

            if (score > bestScore)
            {
                bestScore = score;
                best = s.form;
            }
        }
        return best;
    }
}