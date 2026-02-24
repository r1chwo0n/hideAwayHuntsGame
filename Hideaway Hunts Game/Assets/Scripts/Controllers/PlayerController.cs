using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    public bool isActivePlayer = false;
    private bool isDead = false;
    //private bool isSitting = false;

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

    private Killable myKillable;
    public LayerMask shootMask;

    // =========================
    // Unity
    // =========================

    void Start()
    {
        Debug.Log("Start");
        Debug.Log($"{name} started at {transform.position}");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        if (myKillable != null)
            myKillable.OnKilled += (t) => Die();

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
        Debug.Log($"{name} is the active player.");
        if (isDead) return;
        Debug.Log($"{name} is alive and can move.");

        Move();
        HandleInput();
    }

 public bool IsDead() => myKillable != null && myKillable.isDead;

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetMouseButtonDown(0))
            Shoot();

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

    void Shoot()
    {
        if (Time.timeScale == 0f) return;

        if (manager.CanShoot())
        {
            // เรียก UseAmmo() เพื่อหักกระสุนและบันทึกเวลา Cooldown
            manager.UseAmmo();

            // เล่น Animation และเสียง
            animator.SetTrigger("Shoot");

            if (shootSound != null)
                audioSource.PlayOneShot(shootSound);

            // การยิง Raycast
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f, shootMask))
            {
                Killable target = hit.transform.GetComponent<Killable>();
                if (target != null)
                {
                    target.TakeHit();
                }
            }
        }
        else
        {
            // (Optional) อาจจะใส่เสียง "แก๊ก" เวลาปืนติด Cooldown หรือกระสุนหมดตรงนี้ได้ค่ะ
            Debug.Log("Cannot shoot: Cooldown or Out of ammo");
        }
    }

    // =========================
    // Damage & Death
    // =========================

    public void TakeDamage(int amount = 0)
    {
        if (IsDead()) return;
        
        // เมื่อโดนยิง ให้เรียก TakeHit ของตัวเอง
        myKillable?.TakeHit();
        
        if (crosshairController != null)
            crosshairController.FlashDamage();
    }
    void Die()
    {
        // แจ้ง Manager เมื่อตาย
        manager.OnPlayerDead(this);

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        // ไม่ต้องใช้ Coroutine Disappear แล้ว เพราะ Killable จัดการให้เองตามที่ตั้งค่า
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