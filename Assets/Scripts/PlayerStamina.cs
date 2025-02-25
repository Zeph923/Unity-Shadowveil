using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private float recoveryModeTrigger = 1f; // Trigger για Recovery Mode
    [SerializeField] private float recoveryModeExitThreshold = 25f; // Ποσοστό για έξοδο από Recovery Mode

    private float currentStamina;
    private float lastStaminaUseTime;
    private bool isInRecoveryMode = false; // Flag για Recovery Mode
    private float recoveryModeTimer = 0f; // Χρονόμετρο για την καθυστέρηση αναγέννησης

    private void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
    }

    private void Update()
    {
        // Αν είναι σε Recovery Mode, διαχειρίζεται μόνο επαναφόρτιση
        if (isInRecoveryMode)
        {
            HandleStaminaRecoveryMode();
            return;
        }

        // Επαναφορά stamina με την πάροδο του χρόνου
        HandleStaminaRegeneration();
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaBar.value = currentStamina;
        lastStaminaUseTime = Time.time;

        // Ενεργοποίηση Recovery Mode αν η stamina πέσει κάτω από το trigger
        if (currentStamina < recoveryModeTrigger && !isInRecoveryMode)
        {
            EnterRecoveryMode();
        }
    }

    private void HandleStaminaRegeneration()
    {
        if (currentStamina < maxStamina && (Time.time - lastStaminaUseTime >= 1f) && recoveryModeTimer <= 0f)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaBar.value = currentStamina;
        }
    }

    public void EnterRecoveryMode()
    {
        isInRecoveryMode = true;
        recoveryModeTimer = 2f; // Αρχικοποιούμε το χρονόμετρο για την καθυστέρηση
        Debug.Log("Entered Energy Recovery Mode");
    }

    private void HandleStaminaRecoveryMode()
    {
        // Ανάγνωση καθυστέρησης
        recoveryModeTimer -= Time.deltaTime;

        // Αν η καθυστέρηση έχει περάσει, ξεκινάμε την αναγέννηση stamina
        if (recoveryModeTimer <= 0f)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaBar.value = currentStamina;
        }

        // Έξοδος από το Recovery Mode αν η stamina φτάσει πάνω από το exit threshold
        if (currentStamina >= maxStamina * (recoveryModeExitThreshold / 100f))
        {
            ExitRecoveryMode();
        }
    }

    private void ExitRecoveryMode()
    {
        isInRecoveryMode = false;
        recoveryModeTimer = 0f;
        Debug.Log("Exited Recovery Mode");
    }

    public bool IsInRecoveryMode()
    {
        return isInRecoveryMode;
    }

    public bool CanPerformAction(float amount)
    {
        return currentStamina >= amount && !isInRecoveryMode;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    internal bool TryConsumeStamina(float cost)
    {
        if (CanPerformAction(cost))
        {
            UseStamina(cost);
            return true;
        }

        UseStamina(GetCurrentStamina()); // Use all remaining stamina          
        Debug.Log("Not enough stamina for action!");
        return false;
    }
}
