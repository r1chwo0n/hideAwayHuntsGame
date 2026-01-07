using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public TargetFuzzySetup fuzzySetup;
    public Transform bot;

    public Transform SelectTarget(List<Transform> targets)
    {
        if (targets == null || targets.Count == 0)
            return null;

        Transform best = null;
        float bestScore = -1f;

        foreach (var t in targets)
        {
            float score = Evaluate(t);
            if (score > bestScore)
            {
                bestScore = score;
                best = t;
            }
        }

        return best;
    }

    //float Evaluate(Transform target)
    //{
    //    float dist = Vector3.Distance(bot.position, target.position);

    //    Vector3 dir = (bot.position - target.position).normalized;
    //    float facing = Vector3.Dot(target.forward, dir); // -1..1

    //    float visibility = 1f; // TODO: raycast

    //    PlayerForm form = target.GetComponent<PlayerForm>();
    //    float isActive = (form && form.isActiveForm) ? 1f : 0f;

    //    var inputs = new Dictionary<string, float>
    //{
    //    { "TargetDistance", dist },
    //    { "FacingUs", facing },
    //    { "Visibility", visibility },
    //    { "IsActive", isActive }
    //};

    //    return fuzzySetup.engine.Evaluate(inputs)["TargetPriority"];
    //}
    float Evaluate(Transform target)
    {
        float dist = Vector3.Distance(bot.position, target.position);

        Vector3 dir = (bot.position - target.position).normalized;
        float facing = Vector3.Dot(target.forward, dir);

        float visibility = 1f; // ต่อยอด raycast ภายหลังได้
        float activeChance = EstimateActiveLikelihood(target);

        var inputs = new Dictionary<string, float>
    {
        { "TargetDistance", dist },
        { "FacingUs", facing },
        { "Visibility", visibility },
        { "ActiveLikelihood", activeChance }
    };

        return fuzzySetup.engine.Evaluate(inputs)["TargetPriority"];
    }

    float EstimateActiveLikelihood(Transform target)
    {
        float likelihood = 0f;

        float dist = Vector3.Distance(bot.position, target.position);
        Vector3 dir = (bot.position - target.position).normalized;
        float facing = Vector3.Dot(target.forward, dir);

        // ใกล้ = เสี่ยง
        if (dist < 8f)
            likelihood += 0.3f;

        // เล็งมาทางเรา
        if (facing > 0.6f)
            likelihood += 0.3f;

        // ยืนโล่ง / ไม่หลบ
        if (!Physics.Raycast(
            target.position + Vector3.up,
            -dir,
            dist,
            LayerMask.GetMask("Cover")))
        {
            likelihood += 0.2f;
        }

        // นิ่งผิดปกติ
        var rb = target.GetComponent<Rigidbody>();
        if (rb && rb.linearVelocity.magnitude < 0.1f)
            likelihood += 0.2f;

        return Mathf.Clamp01(likelihood);
    }

}
