using UnityEngine;

public class BotController : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTransform;
    private Rigidbody rb;
    private Animator animator;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundDistance = 0.6f;
    private bool isGrounded;

    private float decisionTimer = 0f;
    private string currentAction = "Idle";
    public int health = 100;
    private bool isDead = false;

    public Transform shootPoint; // Where bot shoots from (assign in inspector)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
    }

    void Update()
    {
        if (isDead) return;

        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0f)
        {
            PickNewAction();
            decisionTimer = Random.Range(2f, 5f);
        }

        PerformAction();
    }

    void PickNewAction()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: currentAction = "Walk"; break;
            case 1: currentAction = "Sit"; break;
            case 2: currentAction = "Jump"; break;
            case 3: currentAction = "Shoot"; break;
        }
    }

    void PerformAction()
    {
        switch (currentAction)
        {
            case "Walk":
                MoveForward();
                animator.SetFloat("Speed", 0.5f);
                animator.SetBool("isSitting", false);
                break;

            case "Sit":
                animator.SetBool("isSitting", true);
                animator.SetFloat("Speed", 0f);
                break;

            case "Jump":
                if (isGrounded)
                {
                    animator.SetTrigger("Jump");
                }
                currentAction = "Idle";
                break;

            case "Shoot":
                animator.SetTrigger("Shoot");
                TryShootPlayer();
                currentAction = "Idle";
                break;

            default:
                animator.SetFloat("Speed", 0f);
                break;
        }
    }

    void MoveForward()
    {
        Vector3 forward = transform.forward;
        rb.MovePosition(rb.position + forward.normalized * speed * 0.5f * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Bot died!");
        isDead = true;
        animator.SetTrigger("Dead");
        this.enabled = false;
        rb.isKinematic = true; // Stop physics if you want
    }

    void TryShootPlayer()
    {
        if (shootPoint == null)
        {
            Debug.LogWarning("shootPoint not assigned on BotController!");
            return;
        }

        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        RaycastHit hit;
        Debug.DrawRay(shootPoint.position, shootPoint.forward * 100f, Color.red, 1f);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            PlayerController player = hit.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(25);
            }
        }
    }
}
