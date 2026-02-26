using UnityEngine;
using UnityEngine.AI;
using TMPro;


public class PlayerManager : MonoBehaviour
{
<<<<<<< Updated upstream
=======
    [Header("Swap Settings")]
    public float swapCooldown = 10f;
    private float swapTimer;
    public TurnTimer turnTimer;

    //[Header("Gun Settings")]
    //public float fireCooldown = 0.5f; // ระยะห่างระหว่างนัด (วินาที)
    //private float lastFireTime = -999f;

>>>>>>> Stashed changes
    public PlayerController[] players;
    public ThirdPersonCameraWithCollision cameraController;
    public PlayerController CurrentPlayer { get; private set; }
    public int sharedAmmo = 30;
    public System.Action<int> OnLifeChanged;
    public System.Action<int> OnAmmoChanged;
    public TMP_Text playerNumberText;

    private int currentIndex;

    void Start()
    {

        if (players == null || players.Length == 0)
        {
            Debug.LogError("No players assigned to PlayerManager");
            return;
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].manager = this;

            Vector3 randomSpawn = GetRandomNavMeshPosition(
                transform.position,  // ใช้ตำแหน่ง PlayerManager เป็น center
                50f                   // รัศมีสุ่ม
            );

            players[i].transform.position = randomSpawn;
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

    Vector3 GetRandomNavMeshPosition(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++) // ลองสุ่ม 30 ครั้งกันพลาด
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            randomPoint.y = center.y;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return center; // fallback
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

        // 🔥 อัปเดตสี minimap ทุกตัว
        for (int i = 0; i < players.Length; i++)
        {
            bool isActive = (players[i] == CurrentPlayer);
            players[i].SetActiveVisual(isActive);
        }

        if (playerNumberText != null)
            playerNumberText.text = "Player " + (index + 1);
<<<<<<< Updated upstream
=======

        // เริ่มนับ Cooldown ใหม่ทุกครั้งที่เปลี่ยนร่างสำเร็จ
        swapTimer = swapCooldown;
        if (turnTimer != null)
        {
            turnTimer.StartTimer();
        }
>>>>>>> Stashed changes
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

        if (deadPlayer == CurrentPlayer)
        {
            GameManager.Instance.Defeat();
        }
    }

    public int CountNearbyBots(float radius)
{
    if (CurrentPlayer == null) return 0;

    // ค้นหา Object รอบตัวในระยะ radius
    Collider[] hits = Physics.OverlapSphere(
        CurrentPlayer.transform.position,
        radius
    );

    int count = 0;

    foreach (var hit in hits)
    {
        Killable k = hit.GetComponent<Killable>();
        if (k != null && 
            k.transform != CurrentPlayer.transform && 
            !k.isPlayer && 
            !k.isDead)
        {
<<<<<<< Updated upstream
            count++;
=======
            Killable k = hit.GetComponent<Killable>();
            if (k != null &&
                k.transform != CurrentPlayer.transform &&
                !k.isPlayer &&
                !k.isDead)
            {
                count++;
            }
>>>>>>> Stashed changes
        }
    }

    return count;
}
}