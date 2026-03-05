using System.Collections.Generic;
using UnityEngine;

public class ActionDecisionBrain : MonoBehaviour, IActionDecisionBrain
{
    public ActionFuzzySetup fuzzySetup;

    public ActionType DecideAction(SituationSummary s)
    {

        if (fuzzySetup == null || fuzzySetup.engine == null)
            return ActionType.Idle;

        var inputs = new Dictionary<string, float>
        {
            { "NearestEnemyDistance", s.nearestEnemyDistance },
            //{ "AverageEnemyDistance", s.avgEnemyDistance },
            { "EnemyDensity", s.enemyCountInRange },
            { "UsSeeingEnemies", s.usSeeingEnemies },
            { "EnemiesSeeingUs", s.enemiesSeeingUs },
            { "FormsRemaining", s.formsRemaining },
            { "EnemyFormsRemaining", s.enemyFormsRemaining },
            { "AmmoRatio", s.ammoRatio },
        };

        var result = fuzzySetup.engine.Evaluate(inputs);

        float v = result["ActionDecision"];
        Debug.Log(v);

        return DecodeAction(v);
    }

    //ActionType DecodeAction(float v)
    //{
    //    if (v < 0.15f) return ActionType.Patrol; // default
    //    if (v < 0.35f) return ActionType.Defend;
    //    if (v < 0.55f) return ActionType.Flank;
    //    if (v < 0.75f) return ActionType.Attack;
    //    return ActionType.Retreat;
    //}

    //ActionType DecodeAction(float v)
    //{
    //    if (v < 0.33f)
    //        return ActionType.Patrol;

    //    if (v < 0.66f)
    //        return ActionType.Attack;

    //    return ActionType.Retreat;
    //}

    //private ActionType DecodeAction(float v)
    //{
    //    if (v < 0.125f) return ActionType.Idle;
    //    if (v < 0.375f) return ActionType.Patrol;
    //    if (v < 0.625f) return ActionType.Defend;
    //    if (v < 0.875f) return ActionType.Attack;

    //    return ActionType.Retreat;
    //}

    //private ActionType DecodeAction(float v)
    //{
    //    if (v < 0.10f) return ActionType.Idle;
    //    if (v < 0.30f) return ActionType.Patrol;
    //    if (v < 0.58f) return ActionType.Defend;
    //    if (v < 0.88f) return ActionType.Attack;
    //    return ActionType.Retreat;
    //}

    private ActionType DecodeAction(float v)
    {
        float minDist = Mathf.Abs(v - 0.0f);
        ActionType best = ActionType.Idle;

        if (Mathf.Abs(v - 0.25f) < minDist)
        {
            minDist = Mathf.Abs(v - 0.25f);
            best = ActionType.Patrol;
        }

        if (Mathf.Abs(v - 0.5f) < minDist)
        {
            minDist = Mathf.Abs(v - 0.5f);
            best = ActionType.Defend;
        }

        if (Mathf.Abs(v - 0.75f) < minDist)
        {
            minDist = Mathf.Abs(v - 0.75f);
            best = ActionType.Attack;
        }

        if (Mathf.Abs(v - 1.0f) < minDist)
        {
            best = ActionType.Retreat;
        }

        return best;
    }
}
