using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ใช้กับปุ่ม Play ใน HomePage → WarningScene
    public void GoToWarningScene()
    {
        SceneManager.LoadScene("WarningScene");
    }

    // ใช้กับปุ่ม Play ใน WarningScene → KingdomScene
    public void GoToGameScene()
    {
        SceneManager.LoadScene("KingdomScene");
    }

    // เผื่ออนาคตอยากกลับหน้า Home
    public void GoToHome()
    {
        SceneManager.LoadScene("HomePage");
    }

    // ออกจากเกม
    public void QuitGame()
    {
        Application.Quit();
    }
}