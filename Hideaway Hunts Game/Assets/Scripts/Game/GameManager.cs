using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum GameState
{
    Playing,
    Victory,
    Defeat
}

public enum BotMode
{
    Fuzzy,
    RuleBased
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Mode")]
    public static BotMode selectedMode;

    [Header("Game State")]
    public GameState currentState;

    [Header("Active Bot")]
    public BotController activeBot;   // 👈 ตัวที่ต้องฆ่า

    [Header("Post Processing")]
    public Volume globalVolume;
    private DepthOfField dof;

    [Header("UI")]
    public GameObject pausePanel;

    private bool isPaused = false;

    // =====================
    // INITIALIZE
    // =====================

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (globalVolume != null && globalVolume.profile.TryGet(out dof))
            dof.active = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    // =====================
    // GAME FLOW
    // =====================

    public void RegisterActiveBot(BotController bot)
    {
        bot.isActive = true;
    }

    public void OnBotKilled(BotController bot)
    {
        if (currentState != GameState.Playing)
            return;

        if (bot == activeBot)
            Victory();
    }

    public void OnPlayerKilled()
    {
        if (currentState != GameState.Playing)
            return;

        Defeat();
    }

    public void Victory()
    {
        currentState = GameState.Victory;

        Time.timeScale = 1f;
        UnlockCursor();

        SceneManager.LoadScene("VictoryScene");
    }

    public void Defeat()
    {
        currentState = GameState.Defeat;

        Time.timeScale = 1f;
        UnlockCursor();

        SceneManager.LoadScene("DefeatScene");
    }

    // =====================
    // PAUSE SYSTEM
    // =====================

    public void TogglePause()
    {
        if (currentState != GameState.Playing)
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;
        pausePanel?.SetActive(true);
        UnlockCursor();
    }

    void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;
        pausePanel?.SetActive(false);
        LockCursor();
    }

    // =====================
    // UTIL
    // =====================

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomePage");
    }
}