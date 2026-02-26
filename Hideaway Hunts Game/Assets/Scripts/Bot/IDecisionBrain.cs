using System.Collections.Generic;
using UnityEngine;

public interface IActionDecisionBrain
{
    ActionType DecideAction(SituationSummary world);
}

public interface IFormSelector
{
    Transform SelectBestForm(List<FormSituation> situations);
}