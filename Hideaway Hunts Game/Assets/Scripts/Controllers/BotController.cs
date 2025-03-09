using UnityEngine;

public class BotController : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float detectionRange = 10f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void Patrol()
    {
        // Add patrol behavior here
    }
}
