using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;  // Max health of the player
    [SerializeField] private int healingPotions = 0; // Number of healing potions
    [SerializeField] private TextMeshProUGUI potionsText;
    [SerializeField] private Slider healthBar;  // UI element to display health
    [SerializeField] private CanvasGroup deathMessageCanvas;  // Canvas for death message UI
    private float currentHealth;  // Current health of the player

    [Header("Damage Feedback")]
    [SerializeField] private float knockbackForce = 5f;  // Strength of knockback when the player takes damage
    [SerializeField] private float stunDuration = 0.5f;  // Duration of stun when the player takes damage

    private bool isStunned = false;  // If the player is stunned and unable to move
    private Renderer playerRenderer;  // Reference to the player's renderer to change color

    private void Start()
    {
        // Initialize health and set the health bar
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Initially hide death message UI
        deathMessageCanvas.alpha = 0f;

        // Get the player's renderer component for color change when taking damage
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // When you press the Q key
        {
            UsePotion(); // Use the healing potion
        }

        potionsText.text = healingPotions.ToString(); // Update the UI with the number of potions
    }

    public void PickupPotion()
    {
        healingPotions++; // Add healing potions
    }

    public void UsePotion()
    {
        if (healingPotions > 0)
        {
            healingPotions--;
            HealPlayer();
            healthBar.value = currentHealth;
        }
    }

    void HealPlayer()
    {
        // Add healing to the player
        currentHealth = Mathf.Min(currentHealth + 20, maxHealth); // Set the healing amount (e.g., 20)
    }

    public void TakeDamage(float damage)
    {
        // Check if the player is stunned, in which case they cannot take damage
        if (isStunned) return;

        // Decrease health by damage and clamp it between 0 and maxHealth
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        // Change the color of the player to indicate damage
        ChangeColorToDamaged();

        // Apply knockback force (if Rigidbody is available)
        ApplyKnockback();

        // Start stun coroutine to prevent player movement for a short time
        StartCoroutine(StunCoroutine());

        // If health reaches 0, handle death
        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private void ChangeColorToDamaged()
    {
        // Change the player's color to red when they take damage
        playerRenderer.material.color = Color.red;
        CancelInvoke("ChangeColorToNormal");  // Cancel any previous color change
        Invoke("ChangeColorToNormal", 0.2f);  // Reset the color back to normal after 0.2 seconds
    }

    private void ChangeColorToNormal()
    {
        // Reset the player's color back to white after the damage effect
        playerRenderer.material.color = Color.white;
    }

    private void ApplyKnockback()
    {
        // Calculate the knockback direction (opposite of where the player is facing)
        Vector3 knockbackDirection = -transform.forward;  // Can adjust this to be based on the enemy direction
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply knockback force to the player in the calculated direction
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    private IEnumerator StunCoroutine()
    {
        // Set player as stunned and prevent movement for the stun duration
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);  // Wait for the stun duration
        isStunned = false;  // Allow movement again after stun duration
    }

    private IEnumerator HandleDeath()
    {
        // Display the "YOU DIED!" message on the UI
        deathMessageCanvas.alpha = 1f;

        // Pause the game using Time.timeScale to stop everything except UI
        Time.timeScale = 0f;

        // Wait for 5 seconds before restarting the game
        yield return new WaitForSecondsRealtime(5f);

        // Resume the game by setting timeScale back to 1
        Time.timeScale = 1f;

        // Reset health and reload the current scene
        deathMessageCanvas.alpha = 0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}