using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    public bool isActivePlayer = false;
    private bool isDead = false;
    private bool isSitting = false;

    [Header("Stats")]
    public float speed = 5f;
    public float jumpForce = 5f;
    public int health = 100;

    [Header("Rotation")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundDistance = 0.8f;
    private bool isGrounded;

    [Header("UI")]
    public CrosshairController crosshairController;

    [HideInInspector] public PlayerManager manager;

    private Rigidbody rb;
    private Animator animator;

    public AudioSource audioSource;
    public AudioClip shootSound;

    // =========================
    // Unity
    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );

        animator.SetBool("isGrounded", isGrounded);
    }

    void Update()
    {
        if (manager.CurrentPlayer != this) return;
        if (isDead) return;

        Move();
        HandleInput();
    }

    public bool IsDead()
    {
        return isDead;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetMouseButtonDown(0))
            Shoot();

        if (Input.GetKeyDown(KeyCode.Q))
            Sit();


    }

    // =========================
    // Movement
    // =========================

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v);
        float inputSpeed = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;

        if (inputDir.sqrMagnitude > 0f)
        {
            float targetAngle =
                Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
                + cameraTransform.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir =
                Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            rb.linearVelocity = new Vector3(
                moveDir.x * speed * inputSpeed,
                rb.linearVelocity.y,
                moveDir.z * speed * inputSpeed
            );

            animator.SetFloat("Speed", inputSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    // =========================
    // Actions
    // =========================

    void Jump()
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Jump");
    }

    void Sit()
    {
        isSitting = !isSitting;
        animator.SetBool("isSitting", isSitting);
    }

    void Shoot()
    {
        if (Time.timeScale == 0f) return;
        if (manager.sharedAmmo <= 0) return;

        manager.UseAmmo();

        animator.SetTrigger("Shoot");

        // 🔊 เล่นเสียงยิง
        if (shootSound != null)
            audioSource.PlayOneShot(shootSound);

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        // if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        // {
        //     BotBody bot = hit.collider.GetComponent<BotBody>();
        //     if (bot != null)
        //         bot.TakeDamage(25);
        // }
    }


    // =========================
    // Damage & Death
    // =========================

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0)
        {
            health = 0;
            Die();
        }
        else
        {
            if (crosshairController != null)
                crosshairController.FlashDamage();
        }
    }





    void Die()
    {
        if (isDead) return;

        isDead = true;
        manager.OnPlayerDead(this);

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;



        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    // =========================
    // Gizmos
    // =========================

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
