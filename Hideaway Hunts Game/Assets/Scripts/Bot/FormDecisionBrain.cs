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

    [Header("Runtime")]
    public Transform activeForm;
    PerceptionController activePerception;

    [Header("Target Selection")]
    public TargetSelector targetSelector;
    public List<Transform> visibleTargets; // player forms ที่มองเห็น

    public CinemachineCamera botCinemachineCam;

    [SerializeField] float decisionInterval = 0.5f;
    float decisionTimer;
    void Update()
    {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer > 0f)
            return;

        decisionTimer = decisionInterval;

        if (forms.Count <= 1)
        {
            //DecideAction();
            return;
        }

        DecideBestForm();
        //DecideAction();
    }

    // ================= FORM SELECTION =================

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

        foreach (var f in forms)
        {
            var ctrl = f.origin.GetComponent<BotController>();
            if (ctrl)
                ctrl.isActive = (f.origin == newForm);
        }

        //actionExecutor.SetActor(newForm);
        //targetSelector.bot = newForm;

        if (botCinemachineCam)
        {
            botCinemachineCam.Follow = newForm;
            botCinemachineCam.LookAt = newForm;
        }


    }


    // ================= ACTION DECISION =================

    void DecideAction()
    {
        if (activePerception == null)
            return;

        visibleTargets =
            worldPerception.GetVisibleEnemies(activePerception);

        SituationSummary world =
            worldPerception.SenseWorld(activePerception);

        ActionType action =
            actionBrain.DecideAction(world);

        if (action == ActionType.Idle)
            action = ActionType.Patrol;

        if (action == ActionType.Attack)
        {
            Transform target =
                targetSelector.SelectTarget(visibleTargets);

            actionExecutor.SetTarget(target);
        }

        actionExecutor.Execute(action);
    }

}
