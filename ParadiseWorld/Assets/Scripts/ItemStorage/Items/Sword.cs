using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Inventory/Sword")]
public class Sword : Item
{
    public float attack = 5f; 
    public float staminaConsumption = 5f;
    public override Item DeepCopy()
    {
        Sword newSword = CreateInstance<Sword>();
        CopyBaseParameters(newSword);
        newSword.attack = attack;
        newSword.staminaConsumption = staminaConsumption;
        newSword.ItemObject = ItemObject;
        return newSword;
    }
}
