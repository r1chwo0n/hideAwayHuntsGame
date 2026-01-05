using System.Collections.Generic;
using UnityEngine;

public class ActionDecisionBrain : MonoBehaviour
{
    public ActionFuzzySetup fuzzySetup;

    public ActionType DecideAction(SituationSummary s)
    {
        if (fuzzySetup == null || fuzzySetup.engine == null)
            return ActionType.Idle;

        var inputs = new Dictionary<string, float>
        {
            { "NearestEnemyDistance", s.nearestEnemyDistance },
            { "AverageEnemyDistance", s.avgEnemyDistance },
            { "EnemyDensity", s.enemyCountInRange },
            { "UsSeeingEnemies", s.usSeeingEnemies },
            { "EnemiesSeeingUs", s.enemiesSeeingUs }
        };

        var result = fuzzySetup.engine.Evaluate(inputs);

        float v = result["ActionDecision"];

        return DecodeAction(v);
    }

    ActionType DecodeAction(float v)
    {
        if (v < 0.1f) return ActionType.Idle;
        if (v < 0.3f) return ActionType.Patrol;
        if (v < 0.5f) return ActionType.Defend;
        if (v < 0.7f) return ActionType.Flank;
        if (v < 0.9f) return ActionType.Attack;
        return ActionType.Retreat;
    }
}
