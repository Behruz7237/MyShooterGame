using Assets.Scripts.Player;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private GunHolder _gunHolder;

    private float _xRotation = 0f;
    private float _groundDistance = .25f, yVelocity;
    private bool isGrounded;
    private float nextTimeToFire = 0f;
    private int _fireRate = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _gunHolder.OnGunChanged += ChangeFireRate;
    }

    private void OnDisable()
    {
        _gunHolder.OnGunChanged -= ChangeFireRate;
    }

    private void Update()
    {
        //JUMP logic
        isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);

        if (isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }
        yVelocity += gravity * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Get input 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        // Create movement direction 
        Vector3 move = transform.right * x + transform.forward * z;

        Vector3 finalMove = move * moveSpeed;
        finalMove.y = yVelocity;


        // Move player 
        controller.Move(finalMove * Time.deltaTime);

        //Debug.Log("MouseSensiticity" + mouseSensitivity);
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (camera) 
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        cameraPoint.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Horizontal rotation (player) 
        transform.Rotate(Vector3.up * mouseX);

        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / _fireRate;
            _gunHolder.Shot();
        }
    }

    private void Jump()
    {
        yVelocity = jumpForce;
    }

    private void ChangeFireRate(int fireRate)
    {
        _fireRate = fireRate;
    }
}
