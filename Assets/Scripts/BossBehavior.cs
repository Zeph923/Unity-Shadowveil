using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;
    public string playerTag = "Player";
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float chaseSpeed = 5f;
    public float chargeSpeed = 10f;
    public float chargeDistance = 15f;
    public float meleeAttackRange = 2f;
    public float detectionRange = 15f;
    public LayerMask obstacleLayer;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f; // The current health, visible in the Inspector
    public TMP_Text healthText; // The TMP_Text that will display the health
    public Slider healthBar; // The health slider

    [Header("Damage Settings")]
    public float chargeDamage = 20f;
    public float meleeDamage = 10f;

    [Header("Attack Settings")]
    public float chargeCooldown = 5f;
    public float meleeCooldown = 2f;

    [Header("Animation Settings")]
    public Animator animator;
    public string rageAnimationTrigger = "Rage";
    public string chargeAnimationTrigger = "Charge";
    public string attackAnimationTrigger = "Hit";
    public string damageAnimationTrigger = "Damage";
    public string dieAnimationTrigger = "Die";
    public string walkSpeedParam = "Speed";

    [Header("UI Settings")]
    public Canvas bossCanvas;
    public TMP_Text bossNameText;
    public Canvas winConditionCanvas; // The canvas that appears when the boss dies
    public TMP_Text winConditionText; // The TextMeshPro that will display the death message of the boss
    public Canvas deathCanvas; // The canvas displayed when the boss dies

    private float chargeCooldownTimer = 0f;
    private float meleeCooldownTimer = 0f;
    private bool isCharging = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool hasSeenPlayer = false;
    private bool isDead = false; // A flag to check if the boss is dead

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth; // Set the max value of the slider
        healthBar.minValue = 0f; // Set the min value of the slider
        healthBar.value = currentHealth; // Set the initial value of the slider based on currentHealth
        bossCanvas.gameObject.SetActive(false);
        winConditionCanvas.gameObject.SetActive(false);
        deathCanvas.gameObject.SetActive(false); // Initially, the deathCanvas is deactivated
        UpdateHealthText(); // Update the health in the UI
    }

    private void Update()
    {
        if (isDead) return; // If the boss is dead, stop all other actions

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRange)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                bossCanvas.gameObject.SetActive(true);
            }
            if (!isChasing && !isAttacking) isChasing = true;
        }

        if (hasSeenPlayer && !isAttacking)
        {
            if (distanceToPlayer >= chargeDistance && chargeCooldownTimer <= 0f && !isCharging)
            {
                StartCoroutine(ChargeAttack());
            }
            else if (distanceToPlayer <= meleeAttackRange)
            {
                if (meleeCooldownTimer <= 0f) StartCoroutine(MeleeAttack());
                else meleeCooldownTimer -= Time.deltaTime;
            }
            else if (!isCharging)
            {
                ChasePlayer();
            }
        }
        chargeCooldownTimer -= Time.deltaTime;
    }

    private void ChasePlayer()
    {
        if (isCharging || isAttacking) return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position, direction, 1f, obstacleLayer))
        {
            transform.position += direction * chaseSpeed * Time.deltaTime;
            animator.SetFloat(walkSpeedParam, chaseSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100f);
        }
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        isChasing = false;

        animator.SetFloat(walkSpeedParam, 0f);
        animator.SetTrigger(rageAnimationTrigger);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetTrigger(attackAnimationTrigger);
        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackDuration * 0.2f);

        if (IsPlayerInFront() && Vector3.Distance(transform.position, player.transform.position) <= meleeAttackRange)
        {
            ApplyDamage(meleeDamage);
        }

        yield return new WaitForSeconds(attackDuration * 0.8f);
        meleeCooldownTimer = meleeCooldown;
        isAttacking = false;
        if (hasSeenPlayer) isChasing = true;
    }

    private IEnumerator ChargeAttack()
    {
        isCharging = true;
        isChasing = false;

        animator.SetTrigger(rageAnimationTrigger);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetTrigger(rageAnimationTrigger);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetTrigger(chargeAnimationTrigger);
        float chargeDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            if (!Physics.Raycast(transform.position, direction, 1f, obstacleLayer))
            {
                transform.position += direction * chargeSpeed * Time.deltaTime;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100f);
            }
            if (Vector3.Distance(transform.position, player.transform.position) <= meleeAttackRange)
            {
                ApplyDamage(chargeDamage);
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        chargeCooldownTimer = chargeCooldown;
        isCharging = false;
        if (hasSeenPlayer) isChasing = true;
    }

    private void ApplyDamage(float damage)
    {
        if (player.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // If the boss is already dead, we do nothing

        currentHealth -= damage;
        healthBar.value = currentHealth; // Update the slider with the current health value
        UpdateHealthText(); // Update the health text

        animator.SetTrigger(damageAnimationTrigger);

        if (currentHealth <= 0 && !isDead) Die(); // If health reaches 0 and the boss is not already dead
    }

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < 45f;
    }

    public void Die()
    {
        isDead = true; // Set the dead flag to true

        animator.SetTrigger(dieAnimationTrigger); // Play the death animation
        StopAllCoroutines(); // Stop all active coroutines (such as movement and attacking)

        // Activate the win condition canvas with the message
        winConditionCanvas.gameObject.SetActive(true);
        winConditionText.text = "THESPESIA"; // Update the text with the message "THESPESIA"

        // Destroy the boss after 5 seconds
        Destroy(gameObject, 5f);
    }

    // Method to update the health text in the UI
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth.ToString("0");
        }
    }
}
