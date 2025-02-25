using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private ActionData lightAttack;
    [SerializeField] private ActionData heavyAttack;

    private PlayerStamina playerStamina;  // Reference to PlayerStamina
    private ThirdPersonController player;
    private void Start()
    {
        // Find the PlayerStamina script
        playerStamina = GetComponent<PlayerStamina>();
        player = GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        
        // Check for light attack (Left Click)
        TryStartAction(lightAttack);

        // Check for heavy attack (Right Click)
        TryStartAction(heavyAttack);
 
    }



    [Serializable]
    struct ActionData
    {
        public string animationTrigger;
        public float Damage;
        public int buttonId;
        public float cost;
        public string name;
        public float range;
    }
    public void DoCurrentAction()
    {
        if (actionToPerform == null)
        {
            Debug.LogError("no action to perform");
            return;
        }
        DoAction(actionToPerform.Value);
        //actionToPerform.Value.action();
        actionToPerform = null;
    }

    

    ActionData? actionToPerform;
    
    private void TryStartAction(ActionData aD)
    {
        if (Input.GetMouseButtonDown(aD.buttonId)) // Check for action button
        {
            if (actionToPerform != null)
            {
                Debug.Log("already perfoming other action");
                return;
            }
            if (playerStamina.TryConsumeStamina(aD.cost))
            {
                player.SetTrigger(aD.animationTrigger);
                actionToPerform = aD;
            }

        }
    }
    private void DoAction(ActionData value)
    {
    
        // If there is an enemy or not, the attack happens and stamina is consumed
        if (IsEnemyInRange(value.range))
        {

            Enemy enemy = GetEnemyInRange(value.range);
            if (enemy != null)
            {
                enemy.TakeDamage(value.Damage); // Apply damage to the enemy
                Debug.Log("Attack performed!");
            }
        }
        else
        {
            // Return "Miss" if no enemy is in the range
            Debug.Log("Missed attack.");
        }
    }

    private bool IsEnemyInRange(float range)
    {
        Enemy enemy = GetEnemyInRange(range);
        return enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= range;
    }

    private Enemy GetEnemyInRange(float range)
    {
        RaycastHit hit;
        // Set the ray origin one unit higher on the Y axis
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 forwardDirection = transform.forward;

        // Visualize the ray in the Scene view starting from the new origin
        Debug.DrawRay(origin, forwardDirection * range, Color.red);

        // Perform the raycast from the updated origin
        if (Physics.Raycast(origin, forwardDirection, out hit, range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                return hit.collider.GetComponent<Enemy>();
            }
        }

        // Return null if no enemy is found within range
        return null;
    }
    
}
