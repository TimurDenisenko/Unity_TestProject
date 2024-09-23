using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Transform Content;
    [SerializeField] public Tooltip tooltip;
    [SerializeField] int SlotsCount;
    [SerializeField] Slot SlotPrefab;
    List<Slot> slots = new List<Slot>();
    private void Awake()
    {
        StaticSoldier.Inventory = this;
        for (int i = 0; i < SlotsCount; i++)
        {
            AddItem(i);
            slots[i].LoadComponent();
        }
    }
    internal void AddItem(Item item)
    {
        for (int i = 0;i < slots.Count;i++)
        {
            if (slots[i].item == null)
            {
                slots[i].UpdateSlot(item);
                break;
            }
            //else if (slots.Where(x => x.item == item).First().item.IsStackable )
            //{

            //}
        }
    }
    internal void AddItem(int i)
    {
        SlotPrefab.item = null;
        SlotPrefab.id = i;
        slots.Add(Instantiate(SlotPrefab, Content));
    }
}
