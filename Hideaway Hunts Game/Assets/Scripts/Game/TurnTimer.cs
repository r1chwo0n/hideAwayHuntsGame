using UnityEngine;
using TMPro;

public class TurnTimer : MonoBehaviour
{
    public float duration = 10f;
    private float currentTime;
    private bool isRunning;

    public TMP_Text timeText;

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        timeText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0)
        {
            isRunning = false;
            timeText.text = "0";
        }
    }

    public void StartTimer()
    {
        currentTime = duration;
        isRunning = true;
    }
}