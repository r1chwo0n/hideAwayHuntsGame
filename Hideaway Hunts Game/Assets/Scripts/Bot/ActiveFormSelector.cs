using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FormSituation // ร่างหนึ่งกับการรับรู้โลกเมื่อใช้ร่างนี้เป็นต้นทาง
{
    public Transform form;
    public SituationSummary world; // world perception เมื่อ origin = ร่างนี้
}

public class ActiveFormSelector : MonoBehaviour
{
    public FormFuzzyAISetup fuzzySetup;

    public Transform SelectBestForm(List<FormSituation> forms)
    {
        if (forms == null || forms.Count == 0)
            return null;

        Transform best = null;
        float bestScore = -1f;

        foreach (var f in forms)
        {
            float score = Evaluate(f.world);
            //Debug.Log($"{f.form.name} score = {score}");
            if (score > bestScore)
            {
                bestScore = score;
                best = f.form;
            }
        }

        return best;
    }

    public float Evaluate(SituationSummary s)
    {
        var inputs = new Dictionary<string, float>
        {
            { "NearestEnemyDistance", s.nearestEnemyDistance }, 
            { "UsSeeingEnemies", s.usSeeingEnemies },
            { "EnemiesSeeingUs", s.enemiesSeeingUs },
            { "EnemyDensity", s.enemyCountInRange },
            { "AverageEnemyDistance", s.avgEnemyDistance },
            //{ "EnemyMovementIntensity", s.maxEnemyMovementIntensity}
        };

        return fuzzySetup.engine.Evaluate(inputs)["FormSuitability"];
    }

}
