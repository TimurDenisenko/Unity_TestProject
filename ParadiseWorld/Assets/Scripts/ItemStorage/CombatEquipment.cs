﻿using UnityEngine;

public class CombatEquipment : MonoBehaviour
{
    [SerializeField] public Transform Content;
    [SerializeField] Slot[] artificalSlot;
    public static Sprite EmptySwordSlot;
    private void Awake()
    {
        StaticSoldier.CombatEquipment = this;
        foreach (Slot slot in artificalSlot)
        {
            slot.LoadComponent();
            if (slot.CompareTag("SwordSlot"))
                EmptySwordSlot = slot.slotIcon.sprite;
        }
    }
}