using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private float recoveryModeTrigger = 1f; // Trigger ��� Recovery Mode
    [SerializeField] private float recoveryModeExitThreshold = 25f; // ������� ��� ����� ��� Recovery Mode

    private float currentStamina;
    private float lastStaminaUseTime;
    private bool isInRecoveryMode = false; // Flag ��� Recovery Mode
    private float recoveryModeTimer = 0f; // ���������� ��� ��� ����������� �����������

    private void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
    }

    private void Update()
    {
        // �� ����� �� Recovery Mode, ������������� ���� ������������
        if (isInRecoveryMode)
        {
            HandleStaminaRecoveryMode();
            return;
        }

        // ��������� stamina �� ��� ������ ��� ������
        HandleStaminaRegeneration();
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaBar.value = currentStamina;
        lastStaminaUseTime = Time.time;

        // ������������ Recovery Mode �� � stamina ����� ���� ��� �� trigger
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
        recoveryModeTimer = 2f; // ������������� �� ���������� ��� ��� �����������
        Debug.Log("Entered Energy Recovery Mode");
    }

    private void HandleStaminaRecoveryMode()
    {
        // �������� ������������
        recoveryModeTimer -= Time.deltaTime;

        // �� � ����������� ���� �������, �������� ��� ���������� stamina
        if (recoveryModeTimer <= 0f)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaBar.value = currentStamina;
        }

        // ������ ��� �� Recovery Mode �� � stamina ������ ���� ��� �� exit threshold
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
