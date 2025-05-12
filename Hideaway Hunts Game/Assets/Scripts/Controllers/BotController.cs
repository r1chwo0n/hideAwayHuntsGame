using UnityEngine;

public class BotController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public Transform cameraTransform;
    private Rigidbody rb;
    private Animator animator;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundDistance = 0.6f;
    private bool isGrounded;

    private float decisionTimer = 0f;
    private string currentAction = "Idle";

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
        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0f)
        {
            PickNewAction();
            decisionTimer = Random.Range(2f, 5f); // เปลี่ยนพฤติกรรมทุกๆ 2-5 วิ
        }

        PerformAction();
    }

    void PickNewAction()
    {
        int rand = Random.Range(0, 4); // เลือก 0-3

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
}
