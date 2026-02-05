using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerController[] players;
    public ThirdPersonCameraWithCollision cameraController;

    private int currentIndex = -1;

    void Start()
    {
        if (players.Length == 0) return;

        for (int i = 0; i < players.Length; i++)
        {
            players[i].manager = this;
            players[i].SetActive(false);
        }

        ActivatePlayer(0); // player 1 เป็น default
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivatePlayer(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivatePlayer(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivatePlayer(2);
    }

    public void ActivatePlayer(int index)
    {
        if (index < 0 || index >= players.Length) return;
        if (index == currentIndex) return;
        if (!players[index].gameObject.activeSelf) return;

        if (currentIndex != -1)
            players[currentIndex].SetActive(false);

        currentIndex = index;
        players[currentIndex].SetActive(true);

        cameraController.SetTarget(players[currentIndex].transform);

        Debug.Log("Active Player: " + players[currentIndex].name);
    }

    public void OnPlayerDead(PlayerController deadPlayer)
    {
        if (players[currentIndex] != deadPlayer) return;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject.activeSelf && !players[i].IsDead())
            {
                ActivatePlayer(i);
                return;
            }
        }

        Debug.Log("All players dead");
    }
}
