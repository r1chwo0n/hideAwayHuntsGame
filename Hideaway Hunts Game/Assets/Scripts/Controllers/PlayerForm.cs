using UnityEngine;

public class PlayerForm : MonoBehaviour
{
    public bool isActiveForm;   // ร่างจริง = true

    public void OnShot()
    {
        if (isActiveForm)
        {
            Debug.Log("💀 Active form shot → GAME OVER");
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.Log("❌ Decoy form destroyed");
            Destroy(gameObject);   // ตัดกำลังถาวร
        }
    }
}
