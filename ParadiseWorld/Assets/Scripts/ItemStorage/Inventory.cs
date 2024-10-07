using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : Storage
{
    [SerializeField] RectTransform view;
    [SerializeField] float secondPos;
    [SerializeField] float secondScale;
    List<Item> defaultItemsInInventory;
    float firstScale;
    float firstPos;

    private void Awake()
    {
        StaticSoldier.Inventory = this;
        SlotsCreating("InventorySlot");
        firstScale = view.localPosition.x;
        firstPos = view.rect.width;
    }
    internal void SetFirstUI()
    {
        view.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, firstPos);
        view.SetLocalPositionAndRotation(new Vector3(firstScale, view.localPosition.y, 0), Quaternion.identity);
    }
    internal void SetSecondUI(bool isLeft)
    {
        view.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, secondScale);
        view.SetLocalPositionAndRotation(new Vector3(isLeft ? secondPos : -secondPos, view.localPosition.y, 0), Quaternion.identity);
    }
    internal void SortBy(Type type)
    {
        defaultItemsInInventory = slots.Select(x => x.item?.DeepCopy() ?? null).ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].item = slots[i].item != null && slots[i].item.GetType() == type ? slots[i].item : null;
        }
        for (int i = 0;i < slots.Count; i++)
        {
            if (slots[i].item != null)
            {
                Slot emptySlot = slots.First(x => x.item == null);
                Item tempItem = emptySlot.item;
                emptySlot.item = slots[i].item;
                slots[i].item = tempItem;
            }
        }
        UpdateStorage();
    }
    internal void SetDefaultInventory()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].item = defaultItemsInInventory[i];
        }
        UpdateStorage();
    } 
}
