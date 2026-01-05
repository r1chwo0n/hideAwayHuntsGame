using UnityEngine;

public class BotController : MonoBehaviour
{
    public bool isActive;
}

public class BotDebugHUD : MonoBehaviour
{
    public static BotDebugHUD Instance;

    public void OnActiveFormChanged(string formName)
    {
        Debug.Log($"[HUD] Active Bot: {formName}");
    }

    public void OnActionChanged(ActionType action)
    {
        Debug.Log($"[HUD] Action: {action}");
    }
}
