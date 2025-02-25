using StarterAssets;
using UnityEngine;
using System.Collections;

public class DodgeManager : MonoBehaviour
{
    private ThirdPersonController _controller;
    private CharacterController _characterController;
    private float _lastDodgeTime;
    private bool _isDodging;

    [Header("Dodge Settings")]
    public float dodgeCooldown = 1f;
    public float dodgeSpeed = 10f;
    public float dodgeTime = 0.3f;
    public float dodgeDistance = 5f;

    private void Awake()
    {
        _controller = GetComponent<ThirdPersonController>();  // Get the ThirdPersonController reference
        _characterController = GetComponent<CharacterController>();  // Ensure we have CharacterController
    }

    private void Update()
    {
        HandleDodge();
    }

    private void HandleDodge()
    {
        if (_isDodging || Time.time - _lastDodgeTime < dodgeCooldown) return;

        // Check for dodge input (sprint and move input)
        if (Input.GetKeyDown(KeyCode.Space)) // You can replace with your specific dodge key/input
        {
            _lastDodgeTime = Time.time;
            _isDodging = true;

            // Determine dodge direction based on character's forward direction
            Vector3 dodgeDirection = _controller.transform.forward;
            Vector3 dodgeVector = dodgeDirection.normalized * dodgeDistance;

            // Apply dodge movement
            StartCoroutine(DodgeMovement(dodgeVector));
        }
    }

    private IEnumerator DodgeMovement(Vector3 dodgeVector)
    {
        float startTime = Time.time;
        while (Time.time - startTime < dodgeTime)
        {
            // Move the character with dodge speed over the time interval
            _characterController.Move(dodgeVector * dodgeSpeed * Time.deltaTime);
            yield return null;
        }

        // Reset dodge flag after dodge time
        _isDodging = false;
    }
}
