using UnityEngine;

public class ActionFuzzySetup : MonoBehaviour
{
    public FuzzyEngine engine;
    float Triangle(float x, float a, float b, float c)
    {
        if (a == b && b == c) return x == a ? 1f : 0f;
        if (x <= a || x >= c) return 0f;
        if (x == b) return 1f;
        if (x < b)
            return (b - a) == 0 ? 1f : (x - a) / (b - a); // ป้องกันหาร 0
        else
            return (c - b) == 0 ? 1f : (c - x) / (c - b);
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
        // อันตรายเฉพาะหน้า
        var enemyDist = new FuzzyVariable("NearestEnemyDistance");
        enemyDist.AddSet(new FuzzySet("Near", // ใกล้
            d => Triangle(d, 0f, 5f, 12f)));
        enemyDist.AddSet(new FuzzySet("Medium", // กลาง
            d => Triangle(d, 8f, 18f, 28f)));
        enemyDist.AddSet(new FuzzySet("Far", // ไกล
            d => Triangle(d, 24f, 32f, 36f)));
        engine.AddInput(enemyDist);

        // การกระจายของสนาม
        var avgDist = new FuzzyVariable("AverageEnemyDistance");
        avgDist.AddSet(new FuzzySet("Close",
            c => Triangle(c, 0f, 0f, 0.4f)));
        avgDist.AddSet(new FuzzySet("Balance", // กลาง ๆ 
            c => Triangle(c, 0.25f, 0.5f, 0.75f)));
        avgDist.AddSet(new FuzzySet("Spread", // กระจาย
            c => Triangle(c, 0.6f, 1f, 1f)));
        engine.AddInput(avgDist);

        // ปริมาณศัตรู
        var density = new FuzzyVariable("EnemyDensity");
        density.AddSet(new FuzzySet("None",
            x => x <= 0 ? 1f : 0f));
        density.AddSet(new FuzzySet("Few",
            x => Triangle(x, 0f, 1f, 2f)));
        density.AddSet(new FuzzySet("Many",
            x => Triangle(x, 1f, 3f, 3f)));
        engine.AddInput(density);

        // คุณภาพการรับรู้ของเรา (เราคุมสนามได้แค่ไหน)
        var usSeeing = new FuzzyVariable("UsSeeingEnemies");
        usSeeing.AddSet(new FuzzySet("Blind", // มองไม่เห็น
            x => x <= 0 ? 1f : 0f));
        usSeeing.AddSet(new FuzzySet("Partial", // เห็นบางส่วน
            x => Triangle(x, 0f, 1f, 2f)));
        usSeeing.AddSet(new FuzzySet("Clear", // เห็นชัดเจน
            x => Triangle(x, 1f, 3f, 3f)));
        engine.AddInput(usSeeing);

        // เรากำลังถูกจับตามองแค่ไหน
        var threat = new FuzzyVariable("EnemiesSeeingUs");
        threat.AddSet(new FuzzySet("Unaware", // ไม่รู้ตัว
            x => x <= 0 ? 1f : 0f));
        threat.AddSet(new FuzzySet("Alert", // ระวังตัว
            x => Triangle(x, 0f, 1f, 2f)));
        threat.AddSet(new FuzzySet("Surrounded", // ถูกล้อมรอบ
            x => Triangle(x, 1f, 3f, 3f)));
        engine.AddInput(threat);

        // เหลืออยู่กี่ร่างที่เล่นได้
        var formsRemain = new FuzzyVariable("FormsRemaining");
        formsRemain.AddSet(new FuzzySet("Critical",
            x => x <= 1 ? 1f : 0f));
        formsRemain.AddSet(new FuzzySet("Limited",
            x => Triangle(x, 1f, 2f, 3f)));
        formsRemain.AddSet(new FuzzySet("Plenty",
            x => x >= 3 ? 1f : 0f));
        engine.AddInput(formsRemain);

        var enemyForms = new FuzzyVariable("EnemyFormsRemaining");
        enemyForms.AddSet(new FuzzySet("Few",
            x => x <= 1 ? 1f : 0f));
        enemyForms.AddSet(new FuzzySet("Several",
            x => Triangle(x, 1f, 2f, 3f)));
        enemyForms.AddSet(new FuzzySet("Many",
            x => x >= 3 ? 1f : 0f));
        engine.AddInput(enemyForms);

        var ammo = new FuzzyVariable("AmmoRatio");
        ammo.AddSet(new FuzzySet("Low",
            x => Triangle(x, 0.0f, 0.3f, 0.5f)));
        ammo.AddSet(new FuzzySet("Enough",
            x => Triangle(x, 0.4f, 0.7f, 0.9f)));
        ammo.AddSet(new FuzzySet("Full",
            x => Triangle(x, 0.8f, 1f, 1f)));
        engine.AddInput(ammo);

    }

    void SetupOutputs()
    {
        var action = new FuzzyVariable("ActionDecision");

        action.AddSet(new FuzzySet("Idle", 0.0f));
        action.AddSet(new FuzzySet("Patrol", 0.2f)); // ลาดตระเวน
        action.AddSet(new FuzzySet("Defend", 0.4f)); // ป้องกัน
        action.AddSet(new FuzzySet("Flank", 0.6f)); 
        action.AddSet(new FuzzySet("Attack", 0.8f)); // โจมตี
        action.AddSet(new FuzzySet("Retreat", 1.0f)); // ถอย

        engine.AddOutput(action);
    }

    void SetupRules()
    {
        // ใกล้มาก + ถูกล้อม → ถอย
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("EnemiesSeeingUs", "Surrounded")
            .AddConclusion("ActionDecision", "Retreat"));

        // เหลือร่างเดียว + ศัตรูเห็นเรา → ถอย
        engine.AddRule(new FuzzyRule()
            .AddCondition("FormsRemaining", "Critical")
            .AddCondition("EnemiesSeeingUs", "Alert")
            .AddConclusion("ActionDecision", "Retreat"));

        // กระสุนต่ำ + ศัตรูเยอะ → ถอย
        engine.AddRule(new FuzzyRule()
            .AddCondition("AmmoRatio", "Low")
            .AddCondition("EnemyFormsRemaining", "Many")
            .AddConclusion("ActionDecision", "Retreat"));

        // ใกล้ + เห็นศัตรูชัด + กระสุนพอ → โจมตี
        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("UsSeeingEnemies", "Clear")
            .AddCondition("AmmoRatio", "Enough")
            .AddConclusion("ActionDecision", "Attack"));

        // เราได้เปรียบเชิงจำนวน + เห็นศัตรู → โจมตี
        engine.AddRule(new FuzzyRule()
            .AddCondition("FormsRemaining", "Plenty")
            .AddCondition("EnemyFormsRemaining", "Few")
            .AddCondition("UsSeeingEnemies", "Clear")
            .AddConclusion("ActionDecision", "Attack"));

        // กระสุนเต็ม + ศัตรูน้อย → โจมตี
        engine.AddRule(new FuzzyRule()
            .AddCondition("AmmoRatio", "Full")
            .AddCondition("EnemyDensity", "Few")
            .AddConclusion("ActionDecision", "Attack"));

        // ไม่มีศัตรูเลย → ลาดตระเวน
        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "None")
            .AddConclusion("ActionDecision", "Patrol"));

        // ศัตรูน้อย + ยังไม่ถูกเห็น → ลาดตระเวน
        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Few")
            .AddCondition("EnemiesSeeingUs", "Unaware")
            .AddConclusion("ActionDecision", "Patrol"));

        // มองไม่เห็นศัตรู → ลาดตระเวน
        engine.AddRule(new FuzzyRule()
            .AddCondition("UsSeeingEnemies", "Blind")
            .AddConclusion("ActionDecision", "Patrol"));

    }
}
