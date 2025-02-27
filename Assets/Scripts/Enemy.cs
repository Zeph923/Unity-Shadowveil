using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private Slider healthBar;

    [Header("Attack Cooldowns")]
    [SerializeField] private float attackCooldown = 2f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 40000f;

    [Header("Damage Delay Settings")]
    [SerializeField] private float damageDelay = 0.5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private LayerMask obstacleLayer;

    private float currentHealth;
    private Transform player;
    private CharacterController playerController;
    private Animator animator;

    private bool isAlive = true;
    private bool isStunned = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isChasing = false;
    private bool hasSeenPlayer = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerController = playerObject.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
            }
            if (!isChasing && !isAttacking) isChasing = true;
        }

        if (hasSeenPlayer && !isAttacking)
        {
            if (distanceToPlayer <= meleeAttackRange && canAttack)
            {
                StartCoroutine(EnemyMeleeAttack());
            }
            else if (!isAttacking)
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        Vector3 direction = (player.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position, direction, 1f, obstacleLayer))
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            animator.SetFloat("Speed", moveSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100f);
        }
    }

    private IEnumerator EnemyMeleeAttack()
    {
        if (!canAttack) yield break;

        isAttacking = true;
        isChasing = false;

        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Attack");

        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackDuration * 0.2f);

        if (Vector3.Distance(transform.position, player.position) <= meleeAttackRange)
        {
            ApplyDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackDuration * 0.8f);
        canAttack = true;
        isAttacking = false;
        if (hasSeenPlayer) isChasing = true;
    }

    private void ApplyDamage(float damage)
    {
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

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

    private void ApplyKnockback()
    {
        if (player == null) return;

        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        knockbackDirection.y = 0f;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(0.5f);
        isStunned = false;
    }

    private void ChangeColorToDamaged()
    {
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ChangeColorToNormal", 0.2f);
    }

    private void ChangeColorToNormal()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f);
    }
}
