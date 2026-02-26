using UnityEngine;
using UnityEngine.SceneManagement;

public class BotModeSelector : MonoBehaviour
{
    public void PlayFuzzy()
    {
        GameManager.selectedMode = BotMode.Fuzzy;
        SceneManager.LoadScene("GameScene");
    }

    public void PlayRule()
    {
        GameManager.selectedMode = BotMode.RuleBased;
        SceneManager.LoadScene("GameScene");
    }
}