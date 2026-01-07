using UnityEngine;

public class TargetFuzzySetup : MonoBehaviour
{
    public FuzzyEngine engine;

    float Triangle(float x, float a, float b, float c)
    {
        if (x <= a || x >= c) return 0f;
        if (x == b) return 1f;
        if (x < b) return (x - a) / (b - a);
        return (c - x) / (c - b);
    }

    void Awake()
    {
        engine = new FuzzyEngine();
        SetupInputs();
        SetupOutputs();
        SetupRules();
    }

    // ================= INPUTS =================

    void SetupInputs()
    {
        // ===== Distance =====
        var dist = new FuzzyVariable("TargetDistance");
        dist.AddSet(new FuzzySet("Near",
            d => Triangle(d, 0f, 5f, 10f)));
        dist.AddSet(new FuzzySet("Medium",
            d => Triangle(d, 8f, 15f, 25f)));
        dist.AddSet(new FuzzySet("Far",
            d => Triangle(d, 20f, 40f, 60f)));
        engine.AddInput(dist);

        // ===== Facing =====
        // dot product -1..1 (เป้าหมายกำลังหันมาทางเราไหม)
        var facing = new FuzzyVariable("FacingUs");
        facing.AddSet(new FuzzySet("No",
            f => Triangle(f, -1f, -0.5f, 0f)));
        facing.AddSet(new FuzzySet("Partial",
            f => Triangle(f, -0.2f, 0.3f, 0.8f)));
        facing.AddSet(new FuzzySet("Yes",
            f => Triangle(f, 0.5f, 1f, 1f)));
        engine.AddInput(facing);

        // ===== Visibility =====
        var visible = new FuzzyVariable("Visibility");
        visible.AddSet(new FuzzySet("Low",
            x => Triangle(x, 0f, 0f, 0.5f)));
        visible.AddSet(new FuzzySet("High",
            x => Triangle(x, 0.5f, 1f, 1f)));
        engine.AddInput(visible);

        var activeChance = new FuzzyVariable("ActiveLikelihood");
        activeChance.AddSet(new FuzzySet("Unlikely",
            x => Triangle(x, 0f, 0f, 0.4f)));
        activeChance.AddSet(new FuzzySet("Possible",
            x => Triangle(x, 0.2f, 0.5f, 0.8f)));
        activeChance.AddSet(new FuzzySet("Likely",
            x => Triangle(x, 0.6f, 1f, 1f)));
        engine.AddInput(activeChance);

    }

    // ================= OUTPUT =================

    void SetupOutputs()
    {
        var priority = new FuzzyVariable("TargetPriority");

        priority.AddSet(new FuzzySet("Low", 0.2f));
        priority.AddSet(new FuzzySet("Medium", 0.5f));
        priority.AddSet(new FuzzySet("High", 0.9f));

        engine.AddOutput(priority);
    }

    // ================= RULES =================

    void SetupRules()
    {
        // น่าจะเป็นร่างจริง + ใกล้ + เห็นชัด
        engine.AddRule(new FuzzyRule()
            .AddCondition("ActiveLikelihood", "Likely")
            .AddCondition("TargetDistance", "Near")
            .AddCondition("Visibility", "High")
            .AddConclusion("TargetPriority", "High"));

        // อาจเป็นร่างจริง + เล็งเรา
        engine.AddRule(new FuzzyRule()
            .AddCondition("ActiveLikelihood", "Possible")
            .AddCondition("FacingUs", "Yes")
            .AddConclusion("TargetPriority", "Medium"));

        // ไกล + ไม่น่าใช่
        engine.AddRule(new FuzzyRule()
            .AddCondition("ActiveLikelihood", "Unlikely")
            .AddCondition("TargetDistance", "Far")
            .AddConclusion("TargetPriority", "Low"));

        // ไกล หรือ มองไม่ชัด → ไม่สน
        engine.AddRule(new FuzzyRule()
            .AddCondition("TargetDistance", "Far")
            .AddConclusion("TargetPriority", "Low"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("Visibility", "Low")
            .AddConclusion("TargetPriority", "Low"));
    }
}
