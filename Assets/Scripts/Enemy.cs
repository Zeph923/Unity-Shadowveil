using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float chargeDamage = 20f;
    [SerializeField] private Slider healthBar;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chargeRange = 5f;
    [SerializeField] private float fieldOfView = 90f; // Initial FOV is 90 degrees

    [Header("Attack Cooldowns")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float chargeCooldown = 20f;

    [Header("Charge Attack Settings")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 10f;

    private float currentHealth;
    private Transform player;
    private bool isAlive = true;
    private bool isStunned = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool canCharge = true;
    private bool hasSeenPlayer = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isAlive && player != null && !isStunned)
        {
            // If the enemy sees the player, start chasing and set FOV to 360
            if (CanSeePlayer())
            {
                isChasing = true;
                hasSeenPlayer = true;
                fieldOfView = 360f; // Change FOV to 360 degrees once the enemy sees the player
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (hasSeenPlayer)
            {
                if (distanceToPlayer <= attackRange)
                {
                    isChasing = false;
                }
                else if (isChasing && !isAttacking)
                {
                    MoveTowardsPlayer();
                }
            }

            // Charge attack if the player is far enough
            if (hasSeenPlayer && distanceToPlayer > chargeRange && canCharge)
            {
                StartCoroutine(EnemyChargeAttack());
            }
            // Melee attack if the player is within attack range
            else if (hasSeenPlayer && distanceToPlayer <= attackRange && canAttack)
            {
                StartCoroutine(EnemyMeleeAttack());
            }
        }
    }

    // Checks if the enemy can see the player
    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 origin = transform.position + Vector3.down;
        Vector3 directionToPlayer = (player.position - origin).normalized;
        float distanceToPlayer = Vector3.Distance(origin, player.position);

        // If the player is out of detection range, return false
        if (distanceToPlayer > detectionRange) return false;

        // If FOV is 360, no need to check angle
        if (fieldOfView == 360f)
        {
            // Raycast to check if the player is in line of sight
            if (Physics.Raycast(origin, directionToPlayer, distanceToPlayer))
            {
                return false;
            }
            return true;
        }

        // Check if the player is within the field of view
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2) return false;

        // If the player is blocked by an obstacle, return false
        if (Physics.Raycast(origin, directionToPlayer, distanceToPlayer))
        {
            return false;
        }

        return true;
    }

    // Move the enemy towards the player
    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Make the enemy face the player in any direction (even if the player is behind)
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * moveSpeed);
    }

    // The enemy takes damage
    public void TakeDamage(float damage)
    {
        if (!isAlive || isStunned) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        ChangeColorToDamaged();
        ApplyKnockback();
        StartCoroutine(StunCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Apply knockback effect when the enemy takes damage
    private void ApplyKnockback()
    {
        if (player == null) return;

        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
        }
    }

    // Stun the enemy for a short duration after taking damage
    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(0.5f);
        isStunned = false;
    }

    // Change the enemy's color to red when damaged
    private void ChangeColorToDamaged()
    {
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ChangeColorToNormal", 0.2f);
    }

    // Revert the enemy's color to normal
    private void ChangeColorToNormal()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    // The enemy dies when health reaches 0
    private void Die()
    {
        isAlive = false;
        Destroy(gameObject, 2f);
    }

    // Coroutine for the enemy's melee attack
    private IEnumerator EnemyMeleeAttack()
    {
        if (!canAttack) yield break;

        isAttacking = true;
        canAttack = false;
        GetComponent<Renderer>().material.color = Color.yellow;

        yield return new WaitForSeconds(1f);

        // If the player is within attack range, deal damage
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        ChangeColorToNormal();
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Coroutine for the enemy's charge attack
    private IEnumerator EnemyChargeAttack()
    {
        if (!canCharge) yield break;

        isAttacking = true;
        canCharge = false;
        GetComponent<Renderer>().material.color = Color.yellow;

        yield return new WaitForSeconds(1f);

        // Charge towards the player
        Vector3 chargeDirection = (player.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + chargeDirection * chargeDistance;
        targetPosition.y += 1f;
        float chargeSpeedAdjusted = chargeSpeed * Time.deltaTime;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, chargeSpeedAdjusted);
            transform.LookAt(targetPosition);

            // If the player is hit, deal damage
            if (Vector3.Distance(transform.position, player.position) < 1.5f)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(chargeDamage);
                }
                break;
            }

            yield return null;
        }

        ChangeColorToNormal();
        isAttacking = false;

        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }
}
