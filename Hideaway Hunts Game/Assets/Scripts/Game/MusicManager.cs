using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;

    [Header("Music")]
    public AudioClip mainMusic;      // ใช้ทุกหน้า
    public AudioClip kingdomMusic;   // ใช้เฉพาะ KingdomScene

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMainMusic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "KingdomScene")
        {
            PlayKingdomMusic();
        }
        else
        {
            PlayMainMusic();
        }
    }

    void PlayMainMusic()
    {
        if (audioSource.clip == mainMusic) return;

        audioSource.clip = mainMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    void PlayKingdomMusic()
    {
        if (audioSource.clip == kingdomMusic) return;

        audioSource.clip = kingdomMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}