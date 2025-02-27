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
    [SerializeField] private float currentHealth = 100f;
    public TMP_Text healthText;
    public Slider healthBar;

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
    public Canvas deathCanvas; // Death UI αν χρειαστείς για έξτρα visuals

    private WinCondition winCondition;  // Το νέο reference

    private float chargeCooldownTimer = 0f;
    private float meleeCooldownTimer = 0f;
    private bool isCharging = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool hasSeenPlayer = false;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0f;
        healthBar.value = currentHealth;
        bossCanvas.gameObject.SetActive(false);
        deathCanvas.gameObject.SetActive(false);
        UpdateHealthText();

        // Παίρνουμε το WinCondition component από τη σκηνή (ή το συνδέεις από το Inspector)
        winCondition = FindObjectOfType<WinCondition>();

        if (winCondition == null)
        {
            Debug.LogError("WinCondition component not found in scene!");
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(10f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRange)
        {
            if (!hasSeenPlayer)
            {
                hasSeenPlayer = true;
                bossCanvas.gameObject.SetActive(true);
            }
            if (!isChasing && !isAttacking)
            {
                isChasing = true;
            }
        }

        if (hasSeenPlayer && !isAttacking)
        {
            if (distanceToPlayer >= chargeDistance && chargeCooldownTimer <= 0f && !isCharging)
            {
                StartCoroutine(ChargeAttack());
            }
            else if (distanceToPlayer <= meleeAttackRange)
            {
                if (meleeCooldownTimer <= 0f)
                {
                    StartCoroutine(MeleeAttack());
                }
                else
                {
                    meleeCooldownTimer -= Time.deltaTime;
                }
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
        if (isDead) return;

        currentHealth -= damage;
        healthBar.value = currentHealth;
        UpdateHealthText();

        animator.SetTrigger(damageAnimationTrigger);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < 45f;
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger(dieAnimationTrigger);
        StopAllCoroutines();

        // Ενημερώνει το WinCondition αν υπάρχει
        if (winCondition != null)
        {
            winCondition.TriggerWinCondition();
        }

        Destroy(gameObject, 5f);
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth.ToString("0");
        }
    }
}
