using UnityEngine;

public class RuleActionDecisionBrain : MonoBehaviour, IActionDecisionBrain
{
    public ActionType DecideAction(SituationSummary world)
    {
        // =====================
        //   NO ENEMY → SEARCH
        // =====================
        if (world.usSeeingEnemies == 0)
            return ActionType.Patrol;

        // =====================
        //   SURVIVAL CHECK
        // =====================

        // กระสุนต่ำ
        if (world.ammoRatio < 0.15f)
            return ActionType.Retreat;

        // โดนหลายตัวเล็ง
        if (world.enemiesSeeingUs >= 2)
            return ActionType.Retreat;

        // ทีมเสียเปรียบ
        if (world.formsRemaining < world.enemyFormsRemaining &&
            world.enemiesSeeingUs > 0)
            return ActionType.Defend;

        // =====================
        //   DISTANCE DECISION
        // =====================

        // ศัตรูใกล้ → ยิง
        if (world.nearestEnemyDistance < 7f)
            return ActionType.Attack;

        // ระยะกลาง → คุมตำแหน่ง
        if (world.nearestEnemyDistance < 15f)
            return ActionType.Defend;

        // ไกล → เดินหา
        return ActionType.Patrol;
    }
}