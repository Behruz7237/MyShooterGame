using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Jump Settings")]
    [Tooltip("Set to 2 for Double Jump")]
    [SerializeField] private int maxJumps = 2;

    private CharacterController controller;
    private Vector3 velocity;
    private int jumpCount;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // -------- Movement --------
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // -------- Ground Check --------
        if (controller.isGrounded)
        {
            // Keep player grounded
            if (velocity.y < 0)
                velocity.y = -2f;

            // Reset jumps when touching ground
            jumpCount = 0;
        }

        // -------- Jump (Double Jump Logic) --------
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < maxJumps)
            {
                // Reset vertical velocity before jumping (important!)
                velocity.y = 0f;

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount++;
            }
        }

        // -------- Gravity --------
        velocity.y += gravity * Time.deltaTime;

        // -------- Apply Movement --------
        controller.Move(velocity * Time.deltaTime);
    }
}