using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    public void SelectFuzzy()
    {
        GameManager.selectedMode = BotMode.Fuzzy;
        SceneManager.LoadScene("WarningScene");
    }

    public void SelectRuleBased()
    {
        GameManager.selectedMode = BotMode.RuleBased;
        SceneManager.LoadScene("WarningScene");
    }
}