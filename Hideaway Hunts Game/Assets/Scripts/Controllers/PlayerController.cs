using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

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


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

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
    bool isMoving = horizontal != 0 || vertical != 0;

    MoveWithCameraDirection();

    if (Input.GetKeyDown(KeyCode.Space) )
    {
        Jump();
    }

    if (Input.GetMouseButtonDown(0))
    {
        animator.SetTrigger("Shoot");
    }

    if (Input.GetKeyDown(KeyCode.Q) )
    {
        Sit();
    }
    if (isMoving && animator.GetBool("isSitting"))
    {
        Stand();
    }
}

    void Sit()
    {
        Debug.Log("Sit");
        animator.SetBool("isSitting", true);
    }
    void Stand()
    {
        animator.SetBool("isSitting", false);
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
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
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(rb.position + moveDir.normalized * speed * inputSpeed * Time.deltaTime);

            animator.SetFloat("Speed", inputSpeed); // Run = 1, Walk = 0.5
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }


     bool IsGrounded()
    {
       return Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

}