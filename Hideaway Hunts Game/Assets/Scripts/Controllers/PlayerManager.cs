using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public PlayerController[] players;
    public ThirdPersonCameraWithCollision cameraController;
    public PlayerController CurrentPlayer { get; private set; }
    public int sharedAmmo = 30;
    public System.Action<int> OnLifeChanged;
    public System.Action<int> OnAmmoChanged;

    private int currentIndex;

    void Start()
    {

        if (players == null || players.Length == 0)
        {
            Debug.LogError("No players assigned to PlayerManager");
            return;
        }

        // 1️⃣ setup ทุก player ก่อน
        for (int i = 0; i < players.Length; i++)
        {
            players[i].manager = this;
            // players[i].SetActive(false);
        }
        ActivatePlayer(0);



    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryActivatePlayer(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryActivatePlayer(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryActivatePlayer(2);

        // DEBUG kill current only
        if (Input.GetKeyDown(KeyCode.X) && CurrentPlayer != null)
        {
            CurrentPlayer.TakeDamage(999);
        }
    }

    void TryActivatePlayer(int index)
    {
        if (index < 0 || index >= players.Length) return;
        int num = index + 1;
        if (players[index].IsDead())
        {
            Debug.Log("Player" + num + "is dead. Cannot switch.");
            return;
        }

        ActivatePlayer(index);
    }


    public void UseAmmo()
    {
        if (sharedAmmo <= 0) return;

        sharedAmmo--;
        Debug.Log("Ammo after shoot: " + sharedAmmo +
              " | Active: " + CurrentPlayer.name);

        OnAmmoChanged?.Invoke(sharedAmmo);
    }

    public void ActivatePlayer(int index)
    {
        if (index < 0 || index >= players.Length) return;
        if (players[index].IsDead()) return;

        CurrentPlayer = players[index];
        cameraController.SetTarget(CurrentPlayer.transform);

        Debug.Log("Active Player: " + CurrentPlayer.name);
    }


    public int AliveCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].IsDead())
                    count++;
            }
            return count;
        }
    }

    public void ForceClearCurrentPlayer(PlayerController player)
    {
        if (CurrentPlayer == player)
            CurrentPlayer = null;
    }
    // public void NotifyPlayerDeath(PlayerController player)
    // {
    //     int alive = AliveCount;

    //     Debug.Log("Alive Players: " + alive);

    //     OnLifeChanged?.Invoke(alive);

    //     // 👇 ตรงนี้แหละที่ต้องใส่
    //     if (alive <= 0)
    //     {
    //         GameManager.Instance.Defeat();
    //     }
    // }


    public void OnPlayerDead(PlayerController deadPlayer)
    {
        Debug.Log(deadPlayer.name + " is Dead");

        int alive = AliveCount;
        OnLifeChanged?.Invoke(alive);

        if (alive <= 0)
        {
            Debug.Log("Calling Defeat");
            Debug.Log(GameManager.Instance);
            GameManager.Instance.Defeat();
            return;
        }

        // ถ้าตัวที่ตายคือคนที่เราควบคุมอยู่
        if (deadPlayer == CurrentPlayer)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].IsDead())
                {
                    ActivatePlayer(i);
                    return;
                }
            }
        }
    }
}
