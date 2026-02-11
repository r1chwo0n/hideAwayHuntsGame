using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    [Header("References")]
    public Transform bot;

    [Header("Scoring Weights")]
    public float distanceWeight = 2.0f;
    public float facingWeight = 1.5f;
    public float visibilityWeight = 1.5f;
    public float activityWeight = 1.0f;

    [Header("Tuning")]
    public float maxEffectiveDistance = 25f;

    public Transform SelectTarget(List<Transform> targets)
    {
        if (targets == null || targets.Count == 0 || bot == null)
            return null;

        Transform best = null;
        float bestScore = float.MinValue;

        foreach (var t in targets)
        {
            float score = EvaluateTarget(t);

            if (score > bestScore)
            {
                bestScore = score;
                best = t;
            }
        }

        return best;
    }

    float EvaluateTarget(Transform target)
    {
        if (!target)
            return float.MinValue;

        float score = 0f;

        // ===== 1. Distance (ใกล้ = อันตราย) =====
        float dist = Vector3.Distance(bot.position, target.position);
        float distScore = Mathf.Clamp01(1f - dist / maxEffectiveDistance);
        score += distScore * distanceWeight;

        // ===== 2. Facing (เล็งมาหาเราไหม) =====
        Vector3 toBot = (bot.position - target.position).normalized;
        float facing = Vector3.Dot(target.forward, toBot); // -1 .. 1
        float facingScore = Mathf.Clamp01((facing + 1f) * 0.5f);
        score += facingScore * facingWeight;

        // ===== 3. Visibility (ยิงได้จริงไหม) =====
        float visibilityScore = HasLineOfSight(target) ? 1f : 0f;
        score += visibilityScore * visibilityWeight;

        // ===== 4. Activity / Threat likelihood =====
        float activityScore = EstimateActivity(target);
        score += activityScore * activityWeight;

        return score;
    }

    bool HasLineOfSight(Transform target)
    {
        Vector3 origin = bot.position + Vector3.up * 1.5f;
        Vector3 dest = target.position + Vector3.up * 1.5f;
        Vector3 dir = dest - origin;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude))
        {
            return hit.transform == target;
        }

        return false;
    }

    float EstimateActivity(Transform target)
    {
        float score = 0f;

        // ใกล้มาก = เสี่ยง
        float dist = Vector3.Distance(bot.position, target.position);
        if (dist < 8f)
            score += 0.4f;

        // เล็งมาทางเรา
        Vector3 toBot = (bot.position - target.position).normalized;
        float facing = Vector3.Dot(target.forward, toBot);
        if (facing > 0.6f)
            score += 0.3f;

        // เคลื่อนไหวผิดปกติ (นิ่ง = อาจเล็ง)
        var rb = target.GetComponent<Rigidbody>();
        if (rb && rb.linearVelocity.magnitude < 0.1f)
            score += 0.3f;

        return Mathf.Clamp01(score);
    }
}
