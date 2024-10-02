
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Inventory/Potion")]
public class Potion : Item
{
    public PotionType potionType;
    public float recovery = 5f;
    public override Item DeepCopy()
    {
        Potion newItem = CreateInstance<Potion>();
        CopyBaseParameters(newItem);
        newItem.potionType = potionType;
        newItem.recovery = recovery;
        return newItem;
    }
}
