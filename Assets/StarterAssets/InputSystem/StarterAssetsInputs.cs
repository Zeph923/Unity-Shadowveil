using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;  // Stores the player's movement input (x, y)
        public Vector2 look;  // Stores the player's look input (x, y)
        public bool jump;  // Whether the player is jumping
        public bool sprint;  // Whether the player is sprinting

        [Header("Movement Settings")]
        public bool analogMovement;  // Whether to use analog input for movement

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;  // Whether the mouse cursor is locked
        public bool cursorInputForLook = true;  // Whether the cursor controls the camera look

        private PlayerStamina playerStamina;  // Reference to the PlayerStamina component
        private CharacterController characterController;  // Reference to the CharacterController component
        private float sprintStaminaCost = 1f;  // Stamina cost per second when sprinting
        private float jumpStaminaCost = 20f;  // Stamina cost for a jump
        private float dodgeStaminaCost = 20f;  // Stamina cost for a dodge
        private float sprintCostInterval = 0.25f;  // Interval for stamina cost while sprinting
        private float lastSprintCostTime;  // Last time sprint stamina was used
        private float sprintHoldTime = 0.5f;  // Time threshold to detect sprint hold
        private float sprintStartTime;  // Time when the sprint was first pressed
        private bool sprintHeld;  // Whether sprint input is being held down
        private bool sprintReleasedEarly;  // Whether sprint input was released early
        private float dodgeSpeed = 10f;  // Speed of the dodge movement
        private float dodgeDuration = 0.2f;  // Duration of the dodge

        private Renderer playerRenderer;  // For making the player invisible
        private bool isDodging = false;  // For preventing damage during dodge

        // Add jump cooldown variables
        private float jumpCooldown = 1f;  // Time in seconds before another jump can be performed
        private float lastJumpTime = -1f;  // Last jump time

        private void Start()
        {
            playerStamina = GetComponent<PlayerStamina>();  // Get the PlayerStamina component
            characterController = GetComponent<CharacterController>();  // Get the CharacterController component
            playerRenderer = GetComponentInChildren<Renderer>();  // Get the renderer of the player model
        }

#if ENABLE_INPUT_SYSTEM
        // Called when movement input changes
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());  // Update the movement input
        }

        // Called when look input changes
        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)  // If cursor controls look
            {
                LookInput(value.Get<Vector2>());  // Update the look input
            }
        }

        // Called when jump input changes
        public void OnJump(InputValue value)
        {
            // Check if enough time has passed since the last jump
            if (playerStamina != null && playerStamina.CanPerformAction(jumpStaminaCost) && Time.time - lastJumpTime >= jumpCooldown)
            {
                playerStamina.UseStamina(jumpStaminaCost);  // Use stamina for jump
                JumpInput(value.isPressed);  // Perform jump action

                // Update the time of the last jump
                lastJumpTime = Time.time;
            }
            else
            {
                JumpInput(false);  // Prevent jumping if cooldown has not passed
            }
        }

        // Called when sprint input changes
        public void OnSprint(InputValue value)
        {
            if (value.isPressed)  // If sprint is pressed
            {
                sprintStartTime = Time.time;  // Record the time sprint started
                sprintHeld = true;  // Set sprint as held
                sprintReleasedEarly = false;  // Reset early release flag
            }
            else
            {
                sprintHeld = false;  // Sprint is released
                if (Time.time - sprintStartTime < sprintHoldTime)  // If sprint was held for less than required time
                {
                    sprintReleasedEarly = true;  // Mark sprint as released early
                    PerformDodge();  // Perform dodge if sprint was released early
                }
                else
                {
                    SprintInput(false);  // Stop sprint if it was not held long enough
                }
            }
        }
#endif

        // Update is called once per frame
        private void Update()
        {
            if (sprintHeld && Time.time - sprintStartTime >= sprintHoldTime)
            {
                if (playerStamina != null && playerStamina.CanPerformAction(sprintStaminaCost))
                {
                    SprintInput(true);  // Start sprint if stamina is available
                }
                else
                {
                    SprintInput(false);  // Stop sprint if stamina is not available
                    sprintHeld = false;  // Reset sprint held flag
                }
            }

            if (sprint && playerStamina != null && Time.time - lastSprintCostTime >= sprintCostInterval)
            {
                if (playerStamina.CanPerformAction(sprintStaminaCost))
                {
                    playerStamina.UseStamina(sprintStaminaCost);  // Use stamina for sprinting
                    lastSprintCostTime = Time.time;  // Update last sprint cost time
                }
                else
                {
                    SprintInput(false);  // Stop sprint if stamina is insufficient
                }
            }
        }

        // Perform a dodge action
        private void PerformDodge()
        {
            // If no movement input, do nothing
            if (move == Vector2.zero || playerStamina == null || !playerStamina.CanPerformAction(dodgeStaminaCost))
                return;

            playerStamina.UseStamina(dodgeStaminaCost);  // Use stamina for dodge

            // Calculate dodge direction based on movement input
            Vector3 dodgeDirection = Vector3.zero;
            Transform cameraTransform = Camera.main.transform;

            if (move.y > 0)
                dodgeDirection = cameraTransform.forward;
            else if (move.y < 0)
                dodgeDirection = -cameraTransform.forward;
            else if (move.x > 0)
                dodgeDirection = cameraTransform.right;
            else if (move.x < 0)
                dodgeDirection = -cameraTransform.right;

            dodgeDirection.y = 0;  // Ensure there's no vertical movement

            if (dodgeDirection != Vector3.zero)
            {
                StartCoroutine(DodgeCoroutine(dodgeDirection.normalized));  // Start dodge coroutine
            }
        }

        // Coroutine for dodge movement
        private System.Collections.IEnumerator DodgeCoroutine(Vector3 direction)
        {
            isDodging = true;  // The player is dodging

            // Disable all renderers in the player (including child objects)
            SetRenderersEnabled(false);

            float elapsed = 0f;
            while (elapsed < dodgeDuration)  // While dodge duration hasn't elapsed
            {
                characterController.Move(direction * dodgeSpeed * Time.deltaTime);  // Move character in dodge direction
                elapsed += Time.deltaTime;  // Update elapsed time
                yield return null;  // Wait for the next frame
            }

            // Enable all renderers again
            SetRenderersEnabled(true);

            isDodging = false;  // End dodge
        }

        // Method to disable or enable all renderers in the player (including children)
        private void SetRenderersEnabled(bool isEnabled)
        {
            // Get all renderers in the player and its children
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                renderer.enabled = isEnabled;  // Set each renderer's enabled state
            }
        }

        // Method to check if the player can take damage
        public bool CanTakeDamage()
        {
            return !isDodging;  // The player can't take damage if they are dodging
        }

        // Update the move direction
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        // Update the look direction
        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        // Update the jump state
        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        // Update the sprint state
        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        // Called when application focus changes
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        // Set the cursor lock state
        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
