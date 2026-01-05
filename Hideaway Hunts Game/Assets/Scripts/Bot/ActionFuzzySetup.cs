using UnityEngine;
public enum ActionType
{
    Idle,
    Patrol,
    Attack,
    Retreat,
    Flank,
    Defend
}

public class ActionFuzzySetup : MonoBehaviour
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

    void SetupInputs()
    {
        var enemyDist = new FuzzyVariable("NearestEnemyDistance");
        enemyDist.AddSet(new FuzzySet("Near",
            d => Triangle(d, 0f, 8f, 15f)));
        enemyDist.AddSet(new FuzzySet("Medium",
            d => Triangle(d, 12f, 20f, 30f)));
        enemyDist.AddSet(new FuzzySet("Far",
            d => Triangle(d, 25f, 40f, 60f)));
        engine.AddInput(enemyDist);


        var avgDist = new FuzzyVariable("AverageEnemyDistance");
        avgDist.AddSet(new FuzzySet("Close",
            c => Triangle(c, 0f, 0f, 0.4f)));
        avgDist.AddSet(new FuzzySet("Balance",
            c => Triangle(c, 0.25f, 0.5f, 0.75f)));
        avgDist.AddSet(new FuzzySet("Spread",
            c => Triangle(c, 0.6f, 1f, 1f)));
        engine.AddInput(avgDist);

        var density = new FuzzyVariable("EnemyDensity");
        density.AddSet(new FuzzySet("None",
            x => x <= 0 ? 1f : 0f));
        density.AddSet(new FuzzySet("Few",
            x => Triangle(x, 0f, 1f, 2f)));
        density.AddSet(new FuzzySet("Many",
            x => Triangle(x, 1f, 3f, 3f)));
        engine.AddInput(density);

        var usSeeing = new FuzzyVariable("UsSeeingEnemies");
        usSeeing.AddSet(new FuzzySet("Blind",
            x => x <= 0 ? 1f : 0f));
        usSeeing.AddSet(new FuzzySet("Partial",
            x => Triangle(x, 0f, 1f, 2f)));
        usSeeing.AddSet(new FuzzySet("Clear",
            x => Triangle(x, 1f, 3f, 3f)));
        engine.AddInput(usSeeing);

        var threat = new FuzzyVariable("EnemiesSeeingUs");
        threat.AddSet(new FuzzySet("Unaware",
            x => x <= 0 ? 1f : 0f));
        threat.AddSet(new FuzzySet("Alert",
            x => Triangle(x, 0f, 1f, 2f)));
        threat.AddSet(new FuzzySet("Surrounded",
            x => Triangle(x, 1f, 3f, 3f)));
        engine.AddInput(threat);

    }

    void SetupOutputs()
    {
        var action = new FuzzyVariable("ActionDecision");

        action.AddSet(new FuzzySet("Idle", 0.0f));
        action.AddSet(new FuzzySet("Patrol", 0.2f));
        action.AddSet(new FuzzySet("Defend", 0.4f));
        action.AddSet(new FuzzySet("Flank", 0.6f));
        action.AddSet(new FuzzySet("Attack", 0.8f));
        action.AddSet(new FuzzySet("Retreat", 1.0f));

        engine.AddOutput(action);
    }

    void SetupRules()
    {
        // ใกล้ + โดนรุม => ถอย
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("EnemiesSeeingUs", "Surrounded")
            .AddConclusion("ActionDecision", "Retreat"));
        // ใกล้ + เห็นศัตรูชัด => โจมตี
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("UsSeeingEnemies", "Clear")
            .AddConclusion("ActionDecision", "Attack"));
        // กลาง + ศัตรูน้อย => โจมตีด้านข้าง
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Medium")
            .AddCondition("EnemyDensity", "Few")
            .AddConclusion("ActionDecision", "Flank"));
        // ศัตรูน้อย + ยังไม่ถูกเห็น => ลาดตระเวน
        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Few")
            .AddCondition("EnemiesSeeingUs", "Unaware")
            .AddConclusion("ActionDecision", "Patrol"));
        // ศัตรูเยอะ + เราไม่เห็นศัตรู => ป้องกัน ตั้งรับ
        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Many")
            .AddCondition("UsSeeingEnemies", "Blind")
            .AddConclusion("ActionDecision", "Defend"));
    }
}
