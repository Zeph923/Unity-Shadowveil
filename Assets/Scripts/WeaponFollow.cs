using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    public Transform handSocket; // Σύνδεσμος με το HandSocket (το GameObject στο χέρι του εχθρού)
    public Vector3 weaponOffset; // Η μετατόπιση του όπλου από το χέρι

    private void Update()
    {
        // Το όπλο ακολουθεί τη θέση και την περιστροφή του χεριού του εχθρού
        transform.position = handSocket.position + weaponOffset;
        transform.rotation = handSocket.rotation; // Εξασφαλίζει ότι το όπλο περιστρέφεται με το χέρι
    }
}
