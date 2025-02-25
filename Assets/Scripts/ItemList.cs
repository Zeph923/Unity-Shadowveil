// Χρησιμοποιείς τα ItemName και ItemCategory που αποθηκεύονται στο ItemEnums.cs
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public ItemName itemName; // Όνομα αντικειμένου
        public ItemCategory itemCategory; // Κατηγορία αντικειμένου
    }

    [SerializeField]
    private List<Item> items; // Λίστα αντικειμένων

    private void Start()
    {
        // Εμφάνιση όλων των αντικειμένων και των κατηγοριών τους
        foreach (var item in items)
        {
            Debug.Log($"Item: {item.itemName}, Category: {item.itemCategory}");
        }
    }
}
