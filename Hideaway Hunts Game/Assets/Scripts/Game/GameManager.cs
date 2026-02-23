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
    public static BotMode selectedMode;
    public Volume globalVolume;
    private DepthOfField dof;
    public static GameManager Instance;

    public GameState currentState;

    [Header("UI")]
    public GameObject pausePanel;

    private bool isPaused = false;

    void Awake()
    {
        Debug.Log("GameManager Awake Called");

        //if (Instance == null)
        //{
        //    Instance = this;
        //    // DontDestroyOnLoad(gameObject);
        //}
        //else if (Instance != this)
        //{
        //    Destroy(gameObject);
        //}
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (globalVolume != null && globalVolume.profile.TryGet(out dof))
        {
            dof.active = false;
        }
        StartGame();
    }

    void Update()
    {
        // กด ESC เพื่อ Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        Debug.Log("Game Started");
    }


    public void Victory()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Victory;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("VictoryScene");
    }

    public void Defeat()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Defeat;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("DefeatScene");
    }

    // =====================
    // 🎮 Pause System
    // =====================

    public void TogglePause()
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =====================
    // 🔄 Restart
    // =====================

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // =====================
    // 🚪 Quit ไป Home
    // =====================

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomePage");
    }
}