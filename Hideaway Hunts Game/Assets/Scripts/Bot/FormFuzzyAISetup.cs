using UnityEngine;

public class FormFuzzyAISetup : MonoBehaviour
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
        // อันตรายเฉพาะหน้า (in range)
        var enemyDist = new FuzzyVariable("NearestEnemyDistance");
        enemyDist.AddSet(new FuzzySet("Near", // ใกล้
            d => Triangle(d, 0f, 5f, 12f)));
        enemyDist.AddSet(new FuzzySet("Medium", // กลาง
            d => Triangle(d, 8f, 20f, 28f)));
        enemyDist.AddSet(new FuzzySet("Far", // ไกล
            d => Triangle(d, 24f, 36f, 40f)));
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
            x => Triangle(x, 0f, 1f, 2.5f)));
        density.AddSet(new FuzzySet("Many",
            x => Triangle(x, 1f, 3f, 4f)));
        engine.AddInput(density);

        // คุณภาพการรับรู้ของเรา (เราคุมสนามได้แค่ไหน)
        var usSeeing = new FuzzyVariable("UsSeeingEnemies");
        usSeeing.AddSet(new FuzzySet("Blind", // มองไม่เห็น
            x => x <= 0 ? 1f : 0f));
        usSeeing.AddSet(new FuzzySet("Partial", // เห็นบางส่วน
            x => Triangle(x, 0f, 1f, 2.5f)));
        usSeeing.AddSet(new FuzzySet("Clear", // เห็นชัดเจน
            x => Triangle(x, 1f, 3f, 4f)));
        engine.AddInput(usSeeing);

        // เรากำลังถูกจับตามองแค่ไหน
        var threat = new FuzzyVariable("EnemiesSeeingUs");
        threat.AddSet(new FuzzySet("Unaware", // ไม่รู้ตัว
            x => x <= 0 ? 1f : 0f));
        threat.AddSet(new FuzzySet("Alert", // ระวังตัว
            x => Triangle(x, 0f, 1f, 2.5f)));
        threat.AddSet(new FuzzySet("Surrounded", // ถูกล้อมรอบ
            x => Triangle(x, 1f, 3f, 4f)));
        engine.AddInput(threat);

        //การเคลื่อนไหวของศัตรู(movement intensity)
        var enemyMove = new FuzzyVariable("EnemyMovementIntensity");
        enemyMove.AddSet(new FuzzySet("Still",
            x => Triangle(x, 0f, 0f, 0.2f)));
        enemyMove.AddSet(new FuzzySet("Moving",
            x => Triangle(x, 0.3f, 0.5f, 0.6f)));
        enemyMove.AddSet(new FuzzySet("Fast",
            x => Triangle(x, 0.5f, 1f, 1.5f)));
        engine.AddInput(enemyMove);
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
        //// rule 1 ศัตรูอยู่ใกล้มาก เราโดนหลายร่างเล็ง แต่เราไม่เห็นพวกเขาเลย => แย่
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("NearestEnemyDistance", "Near")
        //    .AddCondition("EnemiesSeeingUs", "Surrounded")
        //    .AddCondition("UsSeeingEnemies", "Blind")
        //    .AddConclusion("FormSuitability", "Bad"));

        //// rule 2 ศัตรูอยู่ใกล้ เขาเห็นเรา แต่เราไม่เห็นเขา => แย่
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("NearestEnemyDistance", "Near")
        //    .AddCondition("EnemiesSeeingUs", "Alert")
        //    .AddCondition("UsSeeingEnemies", "Blind")
        //    .AddConclusion("FormSuitability", "Bad"));

        //// rule 3 ศัตรูอยู่ใกล้ เราเห็นเขาเยอะ เขายังไม่รู้ตัว => ดี
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("NearestEnemyDistance", "Near")
        //    .AddCondition("UsSeeingEnemies", "Clear")
        //    .AddCondition("EnemiesSeeingUs", "Unaware")
        //    .AddConclusion("FormSuitability", "Good"));

        //// rule 4 ศัตรูกระจายตัว เราเห็นบางส่วน เขายังไม่เห็นเรา => พอใช้ได้
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("AverageEnemyDistance", "Spread")
        //    .AddCondition("UsSeeingEnemies", "Partial")
        //    .AddCondition("EnemiesSeeingUs", "Unaware")
        //    .AddConclusion("FormSuitability", "OK"));

        //// rule 5 ศัตรูน้อย เราเห็นชัด เขาเห็นเรา แต่ยังไม่ใกล้มาก => ดี
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("EnemyDensity", "Few")
        //    .AddCondition("UsSeeingEnemies", "Clear")
        //    .AddCondition("EnemiesSeeingUs", "Alert")
        //    .AddCondition("NearestEnemyDistance", "Medium")
        //    .AddConclusion("FormSuitability", "Good"));

        //// rule 6 ศัตรูอยู่ในรัศมีเยอะ เราโดนล้อม => แย่ 
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("EnemyDensity", "Many")
        //    .AddCondition("EnemiesSeeingUs", "Surrounded")
        //    .AddConclusion("FormSuitability", "Bad"));

        //// rule 7 ศัตรูใกล้ เราไม่เห็น แต่เขาก็ยังไม่เห็นเรา => พอใช้ได้
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("NearestEnemyDistance", "Near")
        //    .AddCondition("UsSeeingEnemies", "Blind")
        //    .AddCondition("EnemiesSeeingUs", "Unaware")
        //    .AddConclusion("FormSuitability", "OK"));

        //// rule 8 ศัตรูมีหลายร่าง แต่กระจายตัว และยังไม่เห็นเรา => พอใช้ได้
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("EnemyDensity", "Many")
        //    .AddCondition("AverageEnemyDistance", "Spread")
        //    .AddCondition("EnemiesSeeingUs", "Unaware")
        //    .AddConclusion("FormSuitability", "OK"));

        //// rule 9 ศัตรูกระจุกอยู่ในรัศมีเยอะ => แย่
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("AverageEnemyDistance", "Close")
        //    .AddCondition("EnemyDensity", "Many")
        //    .AddConclusion("FormSuitability", "Bad"));

        //// rule 10 ศัตรูกระจาย แต่เรามองไม่เห็น => พอใช้ได้
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("AverageEnemyDistance", "Spread")
        //    .AddCondition("UsSeeingEnemies", "Blind")
        //    .AddConclusion("FormSuitability", "OK"));

        //// rule 11 ศัตรูกระจาย เราเห็นทั้งหมด => ดี
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("AverageEnemyDistance", "Spread")
        //    .AddCondition("UsSeeingEnemies", "Clear")
        //    .AddConclusion("FormSuitability", "Good"));

        //// rule 12 ศัตรูอยู่ไกล เราเห็นหมด => ดี
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("NearestEnemyDistance", "Far")
        //    .AddCondition("UsSeeingEnemies", "Clear")
        //    .AddConclusion("FormSuitability", "Good"));

        //// rule 13 ศัตรูอยู่ไกล เห็นบางส่วน => พอใช้ได้
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("NearestEnemyDistance", "Far")
        //    .AddCondition("UsSeeingEnemies", "Partial")
        //    .AddConclusion("FormSuitability", "OK"));

        //// rule 14 ศัตรูกระจายระดับพอดี เราเห็นชัดเจน => ดี
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("AverageEnemyDistance", "Balance")
        //    .AddCondition("UsSeeingEnemies", "Clear")
        //    .AddConclusion("FormSuitability", "Good"));

        //// rule 15 ศัตรูกระจายระดับพอดี แต่ศัตรูเริ่มเห็นเรา => พอใช้ได้
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("AverageEnemyDistance", "Balance")
        //    .AddCondition("EnemiesSeeingUs", "Alert")
        //    .AddConclusion("FormSuitability", "OK"));

        //// rule 16 ไม่มีศัตรูในรัศมี => ดี 
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("EnemyDensity", "None")
        //    .AddConclusion("FormSuitability", "Good"));

        //// ถ้าโดนล้อม ไม่ว่าระยะหรือการมองเห็นเป็นยังไง => Bad
        //engine.AddRule(new FuzzyRule()
        //    .AddCondition("EnemiesSeeingUs", "Surrounded")
        //    .AddConclusion("FormSuitability", "Bad"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Far")
            .AddCondition("EnemyDensity", "None")
            .AddConclusion("FormSuitability", "OK"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("AverageEnemyDistance", "Close")
            .AddCondition("EnemyDensity", "Many")
            .AddCondition("EnemiesSeeingUs", "Alert")
            .AddConclusion("FormSuitability", "Bad"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Medium")
            .AddCondition("EnemyDensity", "Few")
            .AddCondition("UsSeeingEnemies", "Partial")
            .AddCondition("EnemiesSeeingUs", "Unaware")
            .AddConclusion("FormSuitability", "Good"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("EnemyDensity", "Many")
            .AddConclusion("FormSuitability", "Bad"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Few")
            .AddCondition("UsSeeingEnemies", "Partial")
            .AddCondition("EnemyMovementIntensity", "Still")
            .AddConclusion("FormSuitability", "Good"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("NearestEnemyDistance", "Near")
            .AddCondition("UsSeeingEnemies", "Blind")
            .AddCondition("EnemiesSeeingUs", "Alert")
            .AddCondition("EnemyMovementIntensity", "Fast")
            .AddConclusion("FormSuitability", "Bad"));

        engine.AddRule(new FuzzyRule()
            //.AddCondition("NearestEnemyDistance", "Medium")
            //.AddCondition("AverageEnemyDistance", "Balance")
            .AddCondition("UsSeeingEnemies", "Partial")
            .AddCondition("EnemyMovementIntensity", "Moving")
            .AddConclusion("FormSuitability", "Good"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Few")
            .AddCondition("UsSeeingEnemies", "Blind")
            .AddCondition("EnemiesSeeingUs", "Unaware")
            .AddConclusion("FormSuitability", "OK"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("EnemyDensity", "Many")
            .AddCondition("EnemiesSeeingUs", "Surrounded")
            .AddConclusion("FormSuitability", "Bad"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("UsSeeingEnemies", "Clear")
            .AddCondition("EnemiesSeeingUs", "Alert")
            .AddConclusion("FormSuitability", "Good"));

        engine.AddRule(new FuzzyRule()
            .AddCondition("UsSeeingEnemies", "Partial")
            .AddCondition("EnemyMovementIntensity", "Fast")
            .AddConclusion("FormSuitability", "Good"));
    }

}
