using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float turnSmoothTime = 0.1f;
    public int health = 100;
    private float turnSmoothVelocity;
    private bool isSitting = false;
    private bool isDead = false;
    public CrosshairController crosshairController;


    //Rigid body ใช้ในการขยับตัว
    private Rigidbody rb;


    //Animator ใช้ในการควบคุมการเคลื่อนไหวของตัวละคร
    private Animator animator;

    public Transform cameraTransform;

    //check if player is on the ground
    public LayerMask groundLayer;//บอกว่าตรงที่ไหนเป็นพื้น
    public Transform groundCheck;//จุดcheckพื้นวางใต้ตัวละคร
    public float groundDistance = 0.6f;//รัศมีของ sphree ที่ใช้ตรวจสอบพื้น
    private bool isGrounded;//เช็คว่าตัวละครอยู่บนพื้นหรือไม่

    public float mouseSensitivity = 2f;

    float yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false; // 🛑 Prevent animation from moving the character

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }


    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        // Debug.Log($"Grounded: {isGrounded}");
        animator.SetBool("isGrounded", isGrounded);
    }


    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveWithCameraDirection();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Sit();
        }
    }

    void Shoot()
    {
        animator.SetTrigger("Shoot");
    }

    void Sit()
    {
        isSitting = !isSitting; // Toggle sitting state

        animator.SetBool("isSitting", isSitting);
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
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
        else
        {
            // Flash crosshair red on damage
            if (crosshairController != null)
                crosshairController.FlashDamage();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Dead");
        this.enabled = false; // disable player control or movement scripts here
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    void MoveWithCameraDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float inputSpeed = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Calculate movement direction relative to camera
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // 👉 Rotate character to face movement direction
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(rb.position + moveDirection.normalized * speed * inputSpeed * Time.deltaTime);

            animator.SetFloat("Speed", inputSpeed); // 0.5f = walk, 1f = run
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // ✅ Mouse look around (camera already follows the player)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;
        cameraTransform.RotateAround(transform.position, Vector3.up, mouseX); // makes camera orbit if needed
    }
}