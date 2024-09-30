﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] public Transform Content;
    [SerializeField] public Tooltip tooltip;
    [SerializeField] int SlotsCount;
    [SerializeField] Slot SlotPrefab;
    internal List<Slot> slots = new List<Slot>();
    internal void SlotsCreating(string tag)
    {
        for (int i = 0; i < SlotsCount; i++)
        {
            AddItem(i);
            slots[i].LoadComponent();
            slots[i].tag = tag;
        }
    }

    internal void AddItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (item.IsStackable)
            {
                IEnumerable<Slot> items = slots.Where(x => x.item == item && item.Capacity > x.count);
                if (items.Count() > 0)
                {
                    Slot slot = items.First();
                    slot.count++;
                    slot.UpdateSlot(item);
                    break;
                }
            }
            if (slots[i].item == null)
            {
                slots[i].UpdateSlot(item);
                break;
            }
        }
    }
    internal void AddItem(int i)
    {
        SlotPrefab.item = null;
        SlotPrefab.id = i;
        Debug.Log(SlotPrefab.id); 
        slots.Add(Instantiate(SlotPrefab, Content));
    }
}
