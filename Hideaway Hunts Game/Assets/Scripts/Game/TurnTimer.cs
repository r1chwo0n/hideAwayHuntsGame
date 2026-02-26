using UnityEngine;
using TMPro;
using System.Collections;

public class TurnTimer : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float countdownTime = 10f;

    float currentTime;
    Coroutine timerCoroutine;

    public void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        currentTime = countdownTime;

        while (currentTime > 0)
        {
            timeText.text = Mathf.Ceil(currentTime).ToString();
            currentTime -= Time.deltaTime;
            yield return null;
        }

        timeText.text = "0";
    }
}