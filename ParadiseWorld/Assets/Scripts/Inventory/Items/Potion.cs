
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Inventory/Potion")]
public class Potion : Item
{
    public PotionType potionType;
    public float recovery = 5f;
}
