using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class FormDecisionBrain : MonoBehaviour
{
    [Header("References")]
    public WorldPerception worldPerception;
    public ActiveFormSelector formSelector;
    public ActionDecisionBrain actionBrain;
    public ActionExecutor actionExecutor;

    [Header("Forms")]
    public List<PerceptionController> forms;

    [Header("Managers")]
    public BotFormManager botFormManager;

    [Header("Runtime")]
    public Transform activeForm;
    PerceptionController activePerception;

    [Header("Target Selection")]
    public TargetSelector targetSelector;
    public List<Transform> visibleTargets; // player forms ที่มองเห็น

    [Header("Form Swap Settings")]
    [SerializeField] float swapCooldown = 10f; // ระยะเวลาขั้นต่ำที่ต้องอยู่ในร่างนั้น
    float swapTimer; // ตัวนับเวลาถอยหลังสำหรับการเปลี่ยนร่างครั้งต่อไป

    public CinemachineCamera botCinemachineCam;

    [SerializeField] float decisionInterval = 0.5f; // คิดทุก ๆ 0.5 วินาที 
    float decisionTimer; // ถึงเวลาตัดสินใจใหม่ยัง

    void Start()
    {
        foreach (var f in forms.ToArray())
            RegisterForm(f);

        // initial form
        if (forms.Count > 0 && forms[0] != null)
        {
            activePerception = forms[0];
            activeForm = forms[0].origin;

            // เรียกใช้ Helper Function ที่คุณเขียนไว้แล้ว เพื่อกระจายค่าไปยัง Component อื่นๆ
            OnActiveFormChanged(activeForm);

            // บังคับให้เริ่มนับ Cooldown ตั้งแต่เริ่มเกมเลย
            swapTimer = swapCooldown;
        }
    }

    void RegisterForm(PerceptionController p)
    {
        if (!p || !p.origin) return;

        var k = p.origin.GetComponent<Killable>();
        if (k != null)
        {
            k.OnKilled -= OnFormKilled;
            k.OnKilled += OnFormKilled;
        }
    }

    void OnFormKilled(Transform deadForm)
    {
        Debug.Log($"Brain removing dead form {deadForm.name}");

        forms.RemoveAll(f => !f || f.origin == deadForm);

        if (activeForm == deadForm)
        {
            activeForm = null;
            activePerception = null;
            GameManager.Instance.Victory();
        }
    }

    void Update() // เรียกทุกเฟรม
    {

        forms.RemoveAll(f => !f || !f.origin);

        decisionTimer -= Time.deltaTime;
        if (swapTimer > 0) swapTimer -= Time.deltaTime;

        if (decisionTimer > 0f)
            return;

        decisionTimer = decisionInterval;

        //if (forms.Count == 1)
        //{
        //    DecideAction();
        //    return;
        //}

        //DecideAction();

        //if (swapTimer <= 0)
        //{
        //    DecideBestForm();
        //}
        // 1. ตัดสินใจ Action ของ Form ปัจจุบันก่อนเสมอ
        DecideAction();

        // 2. ถ้ามีหลายร่าง และ Cooldown หมดแล้ว ถึงจะอนุญาตให้เปลี่ยนร่าง
        if (forms.Count > 1 && swapTimer <= 0)
        {
            DecideBestForm();
        }
    }

    // ================= FORM SELECTION =================
    // ตัวอื่นควรยืนนิ่ง ถ้าไม่ได้ active
    void DecideBestForm()
    {
        if (forms == null || forms.Count == 0)
            return;

        List<FormSituation> situations = new();

        foreach (var p in forms)
        {
            if (!p || !p.origin)
                continue;

            SituationSummary world = worldPerception.SenseWorld(p);

            Debug.Log(
                $"[FormEval] {p.origin.name} | " +
                $"Nearest={world.nearestEnemyDistance:F1}, " +
                $"Avg={world.avgEnemyDistance:F2}, " +
                $"UsSee={world.usSeeingEnemies}, " +
                $"TheySee={world.enemiesSeeingUs}, " +
                $"Density={world.enemyCountInRange}"
            );

            situations.Add(new FormSituation
            {
                form = p.origin,
                world = world
            });
        }

        Transform best = formSelector.SelectBestForm(situations);

        if (best != null)
        {
            Debug.Log($"[FormDecision] Best = {best.name}");
        }

        if (best != null && best != activeForm)
        {
            activeForm = best;
            activePerception = forms.Find(f => f.origin == best);
            OnActiveFormChanged(best);
        }
    }

    void OnActiveFormChanged(Transform newForm) 
    {
        Debug.Log($"Active form changed to: {newForm.name}");

        swapTimer = swapCooldown; // เริ่มนับถอยหลังใหม่หลังจากเปลี่ยนร่าง
        foreach (var f in forms)
        {
            var ctrl = f.origin.GetComponent<BotController>();
            if (ctrl)
                ctrl.isActive = (f.origin == newForm);
        }

        //actionExecutor.SetActor(newForm);
        //targetSelector.bot = newForm;
        if (actionExecutor)
        {
            actionExecutor.SetActor(newForm);
        }

        if (targetSelector)
        {
            targetSelector.bot = newForm;
        }

        if (newForm && newForm.gameObject.activeInHierarchy)
        {
            botCinemachineCam.Follow = newForm;
            botCinemachineCam.LookAt = newForm;
        }

    }

    // ================= ACTION DECISION =================

    void DecideAction()
    {
        //if (activePerception == null)
        //    return;

        if (!activeForm)
        {
            activePerception = null;
            return;
        }

        // list ศัตรูที่เรามองเห็น
        visibleTargets =
            worldPerception.GetVisibleEnemies(activePerception);
        // snapshot สถานการณ์ปัจจุบัน
        SituationSummary world =
            worldPerception.SenseWorld(activePerception);

        if (botFormManager)
            world.formsRemaining = botFormManager.AliveFormsCount;
        else
            world.formsRemaining = 0;

        world.enemyFormsRemaining = worldPerception.enemies.Count;

        if (actionExecutor && actionExecutor.gun)
            world.ammoRatio = actionExecutor.gun.AmmoRatio;
        else
            world.ammoRatio = 0f;

        Debug.Log(
                $"Nearest={world.nearestEnemyDistance:F1}, " +
                $"Avg={world.avgEnemyDistance:F2}, " +
                $"UsSee={world.usSeeingEnemies}, " +
                $"TheySee={world.enemiesSeeingUs}, " +
                $"Density={world.enemyCountInRange}" + 
                $"Form={world.formsRemaining}" +
                $"Enemy={world.enemyFormsRemaining}" +
                $"Ammo={world.ammoRatio}" 
            );

        // ถ้าเห็นศัตรูอย่างน้อย 1 ตัว
        if (visibleTargets.Count > 0)
        {
            Transform target =
                targetSelector.SelectTarget(visibleTargets);

            if (target == null)
                Debug.Log("ไม่มี target จ้า");

            if (actionExecutor.target != target)
            {
                actionExecutor.SetTarget(target);
            }
        }

        //Debug.Log("Target" + actionExecutor.target);

        ActionType action =
            actionBrain.DecideAction(world);

        if (action == ActionType.Idle)
            action = ActionType.Patrol;

        Debug.Log(action);

        actionExecutor.Execute(action);
    }

}