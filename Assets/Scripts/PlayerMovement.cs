using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseMoveSpeed = 5f; // Normal movement speed
    [SerializeField] private float sprintSpeed = 7f; // Sprinting speed
    [SerializeField] private float recoveryModeSpeed = 3f; // Speed when in stamina recovery mode
    [SerializeField] private float jumpForce = 5f; // Force applied when jumping
    [SerializeField] private float dodgeDistance = 5f; // Distance covered in a dodge
    [SerializeField] private float maxDodgePressTime = 0.2f; // Max time to register a dodge
    [SerializeField] private float jumpStaminaCost = 10f; // Stamina cost for jumping
    [SerializeField] private float sprintStaminaDrain = 5f; // Stamina drain per second while sprinting

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f; // Mouse sensitivity for looking around

    private Rigidbody rb;
    private Transform cameraTransform;
    private Vector3 moveDirection;
    private bool isDodging = false;
    private bool isSprinting = false;
    private bool allowBasicMovementOnly = false;
    private float verticalRotation = 0f;
    private float shiftPressTime = 0f;

    private PlayerStamina playerStamina;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        playerStamina = GetComponent<PlayerStamina>(); // Get the PlayerStamina component
        cameraTransform = Camera.main.transform; // Get the main camera's transform
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    private void Update()
    {
        // If the player is in recovery mode, restrict movement
        if (playerStamina.IsInRecoveryMode())
        {
            isSprinting = false;
            HandleBasicMovement(recoveryModeSpeed);
            HandleMouseRotation();
            return;
        }

        if (allowBasicMovementOnly || isDodging) return;

        HandleMovementInput(); // Capture movement input
        HandleMouseRotation(); // Handle mouse movement

        // Handle shift press for sprinting or dodging
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            float pressDuration = Time.time - shiftPressTime;
            if (pressDuration <= maxDodgePressTime)
            {
                PerformDodge(); // Perform dodge if shift was tapped
            }
        }

        // Sprinting logic
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (playerStamina.CanPerformAction(sprintStaminaDrain * Time.deltaTime) && playerStamina.GetCurrentStamina() > 1f)
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            playerStamina.UseStamina(sprintStaminaDrain * Time.deltaTime); // Drain stamina while sprinting
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(); // Perform jump when space is pressed
        }
    }

    private void FixedUpdate()
    {
        if (allowBasicMovementOnly || isDodging) return;

        // Movement relative to the camera direction
        Vector3 move = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
        move.y = 0f; // Ensure no unintended vertical movement

        float speed = isSprinting ? sprintSpeed : baseMoveSpeed;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down

        moveDirection = new Vector3(horizontal, 0, vertical).normalized; // Normalize to prevent faster diagonal movement
    }

    private void HandleMouseRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX); // Rotate player horizontally

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // Clamp vertical rotation
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f); // Apply vertical rotation
    }

    private void Jump()
    {
        if (Mathf.Abs(rb.velocity.y) < 0.01f && playerStamina.CanPerformAction(jumpStaminaCost))
        {
            playerStamina.UseStamina(jumpStaminaCost);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
        }
    }

    private void PerformDodge()
    {
        if (playerStamina.CanPerformAction(dodgeDistance))
        {
            playerStamina.UseStamina(dodgeDistance);
            isDodging = true;

            Vector3 dodgeDirection = moveDirection != Vector3.zero
                ? transform.forward * moveDirection.z + transform.right * moveDirection.x // Move in movement direction
                : -transform.forward; // If no movement, dodge backwards

            Vector3 newPosition = transform.position + dodgeDirection.normalized * dodgeDistance;
            rb.MovePosition(newPosition);

            Invoke(nameof(EndDodge), 0.1f); // End dodge after a short delay
        }
    }

    private void EndDodge()
    {
        isDodging = false;
    }

    private void HandleBasicMovement(float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 move = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
        move.y = 0f;

        rb.MovePosition(rb.position + move * speed * Time.deltaTime); // Move at basic speed
    }
}
