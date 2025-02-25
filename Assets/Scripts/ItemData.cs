using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public ItemName itemName;      
    public ItemCategory itemCategory; 
    public string displayName;     
}

public enum ItemName
{
    HealingPotion = 1000,
    AladdinsScimitar = 0101,
    LeatherArmor = 0202
}

public enum ItemCategory
{
    Consumables,
    Weapons,
    Armors
}
