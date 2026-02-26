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

            float score = 0f;

            score += w.usSeeingEnemies * 2f;

            // ศัตรูใกล้ → form นี้ useful
            score += Mathf.Clamp(20f - w.nearestEnemyDistance, 0, 20);

            // โดนเล็งเยอะ → ลดคะแนน
            score -= w.enemiesSeeingUs * 3f;

            // ทีมได้เปรียบ → aggressive form
            if (w.formsRemaining > w.enemyFormsRemaining)
                score += 3f;

            // กระสุนต่ำ → ไม่ควรใช้
            if (w.ammoRatio < 0.2f)
                score -= 5f;

            if (score > bestScore)
            {
                bestScore = score;
                best = s.form;
            }
        }

        return best;
    }
}