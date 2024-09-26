using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Inventory/Sword")]
public class Sword : Item
{
    public float attack = 5f; 
    public float staminaConsumption = 5f;
    public GameObject SwordObject;
}
