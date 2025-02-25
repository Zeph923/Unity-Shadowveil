using UnityEngine;
using TMPro;

public class EnemyHealthBarVisibility : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject healthBar; // Η μπάρα υγείας του εχθρού
    [SerializeField] private Transform player;    // Ο παίκτης
    [SerializeField] private TMP_Text enemyNameText;  // Το κείμενο με το όνομα του εχθρού (TextMeshPro)
    [SerializeField] private float visibilityRange = 10f; // Απόσταση ενεργοποίησης
    [SerializeField] private float heightOffset = 2f; // Ύψος της μπάρας πάνω από τον εχθρό
    [SerializeField] private float nameTextOffset = 0.5f; // Ύψος του κειμένου πάνω από τη μπάρα υγείας

    private Camera mainCamera;

    private void Start()
    {
        // Βρίσκουμε την κύρια κάμερα
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Υπολογισμός της απόστασης μεταξύ εχθρού και παίκτη
        float distance = Vector3.Distance(transform.position, player.position);

        // Ενεργοποίηση ή απενεργοποίηση της μπάρας υγείας και του ονόματος
        bool isVisible = distance <= visibilityRange;
        healthBar.SetActive(isVisible);
        if (enemyNameText != null)
        {
            enemyNameText.gameObject.SetActive(isVisible);
        }

        // Ενημέρωση της θέσης και της περιστροφής της μπάρας υγείας και του ονόματος
        if (isVisible)
        {
            Vector3 worldPos = transform.position + Vector3.up * heightOffset;
            healthBar.transform.position = worldPos;

            // Περιστροφή της μπάρας ώστε να κοιτάει πάντα την κάμερα
            if (mainCamera != null)
            {
                healthBar.transform.LookAt(mainCamera.transform);
                healthBar.transform.Rotate(0, 180, 0); // Αντιστροφή για σωστή όψη

                // Ενημέρωση για το κείμενο ονόματος
                if (enemyNameText != null)
                {
                    Vector3 nameWorldPos = worldPos + Vector3.up * nameTextOffset;
                    enemyNameText.transform.position = nameWorldPos;
                    enemyNameText.transform.LookAt(mainCamera.transform);
                    enemyNameText.transform.Rotate(0, 180, 0); // Αντιστροφή για σωστή όψη
                }
            }
        }
    }
}
