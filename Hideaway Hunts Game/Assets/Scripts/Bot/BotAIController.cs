using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotAIController : MonoBehaviour
{
    public FuzzyEngine fuzzyEngine;

    // ข้อมูลของ Bot
    public int currentAmmo = 5;
    public int remainingForms = 3;

    // ระยะการนับศัตรูใกล้เคียง
    public float nearbyEnemyRadius = 10f;

    // Tag ของศัตรู
    public string enemyTag = "Player";

    void Start()
    {
        InitializeFuzzyEngine();
    }

    void Update()
    {
        // 1️ อ่าน input จริง
        float distanceToNearestEnemy = GetNearestEnemyDistance();
        int numberOfNearbyEnemies = CountNearbyEnemies();
        bool isEnemyAimingAtMe = CheckIfEnemyIsAiming(); // Optional → ให้ false ได้
        float angleToNearestEnemy = GetAngleToNearestEnemy();

        // 2️ Prepare input Dictionary
        var inputs = new Dictionary<string, float>
        {
            { "currentAmmo", currentAmmo },
            { "enemyDistance", distanceToNearestEnemy },
            { "enemyCountNearby", numberOfNearbyEnemies },
            { "remainingForms", remainingForms },
            { "lineOfSight", isEnemyAimingAtMe ? 1f : 0f },
            { "enemyAngle", angleToNearestEnemy }
        };

        // 3️⃣ Evaluate FuzzyEngine
        var decision = fuzzyEngine.Evaluate(inputs);

        // 4️⃣ ทำ Action ตาม Output
        if (decision.TryGetValue("Action", out string action))
        {
            switch (action)
            {
                case "Attack": Attack(); break;
                case "Retreat": Retreat(); break;
                case "ChangeForm": ChangeForm(); break;
                case "Defend": Defend(); break;
            }
        }
    }

    void InitializeFuzzyEngine()
    {
        fuzzyEngine = new FuzzyEngine();

        var currentAmmoVar = new FuzzyVariable("currentAmmo");
        currentAmmoVar.AddSet(new FuzzySet("น้อย", x => Mathf.Clamp01(1 - x / 2f)));
        currentAmmoVar.AddSet(new FuzzySet("กลาง", x => Mathf.Clamp01(1 - Mathf.Abs(x - 2.5f) / 1.5f)));
        currentAmmoVar.AddSet(new FuzzySet("เยอะ", x => Mathf.Clamp01((x - 3f) / 2f)));

        var enemyDistanceVar = new FuzzyVariable("enemyDistance");
        enemyDistanceVar.AddSet(new FuzzySet("ใกล้", x => Mathf.Clamp01(1 - x / 5f)));
        enemyDistanceVar.AddSet(new FuzzySet("กลาง", x => Mathf.Clamp01(1 - Mathf.Abs(x - 7.5f) / 2.5f)));
        enemyDistanceVar.AddSet(new FuzzySet("ไกล", x => Mathf.Clamp01((x - 10f) / 5f)));

        var enemyCountNearbyVar = new FuzzyVariable("enemyCountNearby");
        enemyCountNearbyVar.AddSet(new FuzzySet("น้อย", x => Mathf.Clamp01(1 - x / 1f)));
        enemyCountNearbyVar.AddSet(new FuzzySet("กลาง", x => Mathf.Clamp01(1 - Mathf.Abs(x - 2f) / 1f)));
        enemyCountNearbyVar.AddSet(new FuzzySet("มาก", x => Mathf.Clamp01((x - 3f) / 2f)));

        var remainingFormsVar = new FuzzyVariable("remainingForms");
        remainingFormsVar.AddSet(new FuzzySet("เหลือน้อย", x => Mathf.Clamp01(1 - x / 1f)));
        remainingFormsVar.AddSet(new FuzzySet("ปานกลาง", x => Mathf.Clamp01(1 - Mathf.Abs(x - 2f) / 1f)));
        remainingFormsVar.AddSet(new FuzzySet("เหลือเยอะ", x => Mathf.Clamp01((x - 3f) / 1f)));

        var lineOfSightVar = new FuzzyVariable("lineOfSight");
        lineOfSightVar.AddSet(new FuzzySet("ใช่", x => x));
        lineOfSightVar.AddSet(new FuzzySet("ไม่ใช่", x => 1 - x));

        var enemyAngleVar = new FuzzyVariable("enemyAngle");
        enemyAngleVar.AddSet(new FuzzySet("หน้า", x => Mathf.Clamp01(1 - Mathf.Abs(Mathf.DeltaAngle(x, 0f)) / 45f)));
        enemyAngleVar.AddSet(new FuzzySet("ซ้าย", x => Mathf.Clamp01(1 - Mathf.Abs(Mathf.DeltaAngle(x, 270f)) / 45f)));
        enemyAngleVar.AddSet(new FuzzySet("ขวา", x => Mathf.Clamp01(1 - Mathf.Abs(Mathf.DeltaAngle(x, 90f)) / 45f)));
        enemyAngleVar.AddSet(new FuzzySet("หลัง", x => Mathf.Clamp01(1 - Mathf.Abs(Mathf.DeltaAngle(x, 180f)) / 45f)));

        // 🔸 Add Input Variables
        fuzzyEngine.AddInput(currentAmmoVar);
        fuzzyEngine.AddInput(enemyDistanceVar);
        fuzzyEngine.AddInput(enemyCountNearbyVar);
        fuzzyEngine.AddInput(remainingFormsVar);
        fuzzyEngine.AddInput(lineOfSightVar);
        fuzzyEngine.AddInput(enemyAngleVar);

        // 🔸 Output Variable
        var actionVar = new FuzzyVariable("Action");
        actionVar.AddSet(new FuzzySet("Attack", x => x));
        actionVar.AddSet(new FuzzySet("Retreat", x => x));
        actionVar.AddSet(new FuzzySet("ChangeForm", x => x));
        actionVar.AddSet(new FuzzySet("Defend", x => x));

        fuzzyEngine.AddOutput(actionVar);

        // 🔸 ใส่ Rule ตัวอย่าง
        fuzzyEngine.AddRule(new FuzzyRule()
            .AddCondition("currentAmmo", "น้อย")
            .AddCondition("remainingForms", "เหลือเยอะ")
            .AddConclusion("Action", "ChangeForm"));

        fuzzyEngine.AddRule(new FuzzyRule()
            .AddCondition("currentAmmo", "เยอะ")
            .AddCondition("enemyDistance", "ใกล้")
            .AddCondition("lineOfSight", "ไม่ใช่")
            .AddConclusion("Action", "Attack"));

        fuzzyEngine.AddRule(new FuzzyRule()
            .AddCondition("enemyCountNearby", "มาก")
            .AddConclusion("Action", "Retreat"));
    }

    // 🔹 ฟังก์ชันอ่านค่าจากเกม (ยังเป็นตัวอย่าง → คุณเติม logic จริงได้เลย)

    float GetNearestEnemyDistance()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
                minDistance = dist;
        }

        return minDistance == float.MaxValue ? 999f : minDistance;
    }

    int CountNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, nearbyEnemyRadius);
        int count = colliders.Count(c => c.CompareTag(enemyTag));
        return count;
    }

    bool CheckIfEnemyIsAiming()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float aimingThresholdAngle = 10f; // ถ้าเล็งมาในมุมไม่เกิน 10 องศา → ถือว่าเล็ง

        foreach (var enemy in enemies)
        {
            Vector3 toBot = (transform.position - enemy.transform.position).normalized;
            Vector3 enemyForward = enemy.transform.forward;

            // Debug Line → enemyForward (สีฟ้า)
            Debug.DrawRay(enemy.transform.position, enemyForward * 5f, Color.cyan);

            // Debug Line → ทิศไปหา Bot (สีเหลือง)
            Debug.DrawLine(enemy.transform.position, transform.position, Color.yellow);

            float dot = Vector3.Dot(enemyForward, toBot); // 1 = หันตรงมา, -1 = หันไปทางอื่น
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            // Debug.Log($"Enemy {enemy.name} aiming angle: {angle}");

            if (angle < aimingThresholdAngle)
            {
                // Enemy ตัวนี้กำลังเล็งมา → Return true ทันที
                Debug.Log($"Enemy {enemy.name} is aiming at Bot! Angle: {angle}");
                return true;
            }
        }

        // ไม่มี enemy ตัวไหนเล็งมา → Return false
        return false;
    }



    float GetAngleToNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0)
            return 0f;

        GameObject nearestEnemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
        Vector3 dirToEnemy = nearestEnemy.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, dirToEnemy, Vector3.up);

        return (angle + 360f) % 360f;
    }

    // 🔹 Behavior ของ Bot → ใส่ logic จริงได้เลย

    void Attack()
    {
        Debug.Log("Bot Action: Attack!");
        // ใส่ logic ยิง
    }

    void Retreat()
    {
        Debug.Log("Bot Action: Retreat!");
        // ใส่ logic ถอย
    }

    void ChangeForm()
    {
        Debug.Log("Bot Action: Change Form!");
        // ใส่ logic เปลี่ยนร่าง
    }

    void Defend()
    {
        Debug.Log("Bot Action: Defend!");
        // ใส่ logic ตั้งรับ
    }
}
