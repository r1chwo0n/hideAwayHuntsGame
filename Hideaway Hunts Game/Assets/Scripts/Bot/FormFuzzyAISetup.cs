using UnityEngine;

public class FormFuzzyAISetup : MonoBehaviour
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
            d => Triangle(d, 0f, 30f, 70f)));
        enemyDist.AddSet(new FuzzySet("Medium",
            d => Triangle(d, 50f, 100f, 120f)));
        enemyDist.AddSet(new FuzzySet("Far",
            d => Triangle(d, 110f, 150f, 200f)));
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
        var suitability = new FuzzyVariable("FormSuitability");

        suitability.AddSet(new FuzzySet("Bad", 0.2f));
        suitability.AddSet(new FuzzySet("OK", 0.6f));
        suitability.AddSet(new FuzzySet("Good", 1.0f));

        engine.AddOutput(suitability);
    }


    void SetupRules()
    {
        // rule 1: ใกล้มาก ศัตรูเห็นเราเยอะ เราไม่เห็นศัตรู => แย่มาก
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("EnemiesSeeingUs", "Surrounded")
            .AddCondition("UsSeeingEnemies", "Blind")
            .AddConclusion("FormSuitability", "Bad"));
        // rule 2: ใกล้มาก มีศัตรูเห็นเรา => แย่อยู่นะ
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("EnemiesSeeingUs", "Alert")
            .AddConclusion("FormSuitability", "Bad"));
        // rule 3: ใกล้มาก ศัตรูเห็นเราไม่เยอะ => ดี ปะทะได้อยู่
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("UsSeeingEnemies", "Clear")
            .AddConclusion("FormSuitability", "Good"));
        // rule 4: ศัตรูกระจาย เราเห็นศัตรูบางส่วน => OK
        engine.AddRule(new FuzzyRule()
            .AddCondition("AverageEnemyDistance", "Spread")
            .AddCondition("UsSeeingEnemies", "Partial")
            .AddConclusion("FormSuitability", "OK"));
        // rule 5: ศัตรูไม่มาก เราเห็นศัตรูชัดเจน => ดี
        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Few")
            .AddCondition("UsSeeingEnemies", "Clear")
            .AddConclusion("FormSuitability", "Good"));
        // rule 6: เราถูกรุม => แย่จัง
        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Few")
            .AddConclusion("FormSuitability", "Bad"));
        // rule 7: ศัตรูอยู่ไกล เรายังไม่ถูกเห็น => OK
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Far")
            .AddCondition("EnemiesSeeingUs", "Unaware")
            .AddConclusion("FormSuitability", "OK"));
    }

}
