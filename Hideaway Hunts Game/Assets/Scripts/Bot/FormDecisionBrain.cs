using System.Collections.Generic;
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

    void Update()
    {
        DecideBestForm();
        DecideAction();
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

            situations.Add(new FormSituation
            {
                form = p.origin,
                world = world
            });
        }

        Transform best = formSelector.SelectBestForm(situations);

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

        //BotDebugHUD.Instance?.OnActiveFormChanged(newForm.name);

        actionExecutor.actor = newForm;
    }


    // ================= ACTION DECISION =================

    void DecideAction()
    {
        if (activePerception == null)
            return;

        SituationSummary world =
            worldPerception.SenseWorld(activePerception);

        ActionType action =
            actionBrain.DecideAction(world);

        actionExecutor.Execute(action);
    }
}
