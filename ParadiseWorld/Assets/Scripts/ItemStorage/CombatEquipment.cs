using UnityEngine;

public class CombatEquipment : Storage
{
    [SerializeField] Slot[] artificalSlots;
    private void Awake()
    {
        StaticSoldier.CombatEquipment = this;
        foreach (Slot slot in artificalSlots)
        {
            slot.LoadComponent();
        }
    }
}