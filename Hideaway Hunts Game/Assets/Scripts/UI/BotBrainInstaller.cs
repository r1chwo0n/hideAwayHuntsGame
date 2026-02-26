using UnityEngine;

public class BotBrainInstaller : MonoBehaviour
{
    public FormDecisionBrain brain;

    [Header("Fuzzy")]
    public MonoBehaviour fuzzyFormSelector;
    public MonoBehaviour fuzzyActionBrain;

    [Header("Rule")]
    public MonoBehaviour ruleFormSelector;
    public MonoBehaviour ruleActionBrain;

    void Start()
    {
        if (GameManager.selectedMode == BotMode.Fuzzy)
        {
            brain.formSelector = fuzzyFormSelector;
            brain.actionBrain = fuzzyActionBrain;

            Debug.Log("BOT MODE = FUZZY");
        }
        else
        {
            brain.formSelector = ruleFormSelector;
            brain.actionBrain = ruleActionBrain;

            Debug.Log("BOT MODE = RULE");
        }
        brain.RefreshBrains();
    }
}