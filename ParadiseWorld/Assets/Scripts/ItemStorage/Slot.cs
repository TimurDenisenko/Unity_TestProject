using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Item item;
    [SerializeField] public int count = 1;
    static Slot draggingSlot;
    internal int id;
    internal Image slotIcon;
    TMP_Text slotCount;
    Image eventTarget;
    bool isEquip = false;
    Button btn;
    Transform childrenSlot;
    Transform bodySlot;
    Transform parent;
    GridLayoutGroup gridLayoutGroup;
    RectTransform rectTransform;
    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }
    public void OnClick()
    {
        if (item is Sword sword)
            SwordAction();
        else if (item is Potion potion)
        {
            PotionAction(potion);
        }
    }

    private void PotionAction(Potion potion)
    {
        switch (potion.potionType)
        {
            case PotionType.Health:
                SoldierComponents.AttackComponent.RestoreHealth(potion.recovery);
                break;
            case PotionType.Stamina:
                SoldierComponents.AttackComponent.RestoreStamina(potion.recovery);
                break;
            case PotionType.Mana:
                SoldierComponents.AttackComponent.RestoreMana(potion.recovery);
                break;
        }
        if (count > 1)
        {
            --count;
            UpdateSlot();
        }
        else
        {
            DeleteSlot();
        }
    }

    private void DeleteSlot()
    {
        item = null;    
        slotIcon.sprite = null;
        slotCount.text = null;
    }

    private void SwordAction()
    {
        if (isEquip)
        {
            SoldierComponents.ControlComponent.ClearSword();
            isEquip = false;
        }
        else
        {
            if (SoldierComponents.ControlComponent.currentSword != null)
                SoldierComponents.ControlComponent.currentSwordSlot.isEquip = false;
            SoldierComponents.ControlComponent.EquipSword(this);
            isEquip = true;
        }
    }
    internal void LoadComponent()
    {
        eventTarget = transform.GetComponent<Image>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
        bodySlot = transform.GetChild(0);
        childrenSlot = bodySlot.GetChild(0);
        slotIcon = childrenSlot.GetChild(0).GetComponent<Image>();
        slotCount = childrenSlot.GetChild(1).GetComponent<TMP_Text>();
        if (item != null)
            UpdateSlot();
    }
    internal void UpdateSlot(Item newItem)
    {
        item = newItem;
        UpdateSlot();
    }

    private void SetCount() 
    {
        if (item?.IsStackable ?? false)
            slotCount.text = count.ToString();
        else
            slotCount.text = "";
    } 

    internal void UpdateSlot() 
    {
        SetIcon();
        SetCount();
    }
    private void SetIcon() => slotIcon.sprite = item?.Icon ?? null;

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        bodySlot.position = Input.mousePosition;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        draggingSlot = this;
        parent = bodySlot.parent;
        bodySlot.SetParent(StorageSetting.CanvasForDraggingItem);
        eventTarget.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null) return;
        Slot drop = eventData.pointerEnter?.GetComponent<Slot>() ?? null;
        if (drop != null)
        {
            ActionWithDraggingSlot(drop);
        }
        StopDragging();
    }

    private void ActionWithDraggingSlot(Slot drop)
    {
        if (drop.gameObject.CompareTag("ChestSlot") && gameObject.CompareTag("InventorySlot") || drop.gameObject.CompareTag("InventorySlot") && gameObject.CompareTag("ChestSlot"))
        {
            DraggingBetweenChestAndInventory(drop);
        }
        else if (drop.gameObject.CompareTag("SwordSlot") && gameObject.CompareTag("InventorySlot") || drop.gameObject.CompareTag("InventorySlot") && gameObject.CompareTag("SwordSlot"))
        {
            DraggingBetweenSwordAndInventory(drop);
        }
        else
        {
            DraggingInStorage(drop);
        }
    }

    private void DraggingBetweenSwordAndInventory(Slot drop)
    {
        if (drop.gameObject.CompareTag("SwordSlot"))
        {
            transform.SetParent(SoldierComponents.CombatEquipmentComponent.Content);
            rectTransform.SetLocalPositionAndRotation(drop.rectTransform.localPosition, Quaternion.identity);
            drop.gridLayoutGroup.enabled = true;
            drop.transform.SetParent(SoldierComponents.InventoryComponent.Content);
            drop.SetIcon();
        }
        else
        {
            drop.gridLayoutGroup.enabled = false;
            int index = drop.transform.GetSiblingIndex();
            drop.transform.SetParent(SoldierComponents.CombatEquipmentComponent.Content);
            drop.rectTransform.SetLocalPositionAndRotation(rectTransform.localPosition, Quaternion.identity);
            transform.SetParent(SoldierComponents.InventoryComponent.Content);
            transform.SetSiblingIndex(index);
            drop.slotIcon.sprite = CombatEquipment.EmptySwordSlot;
        }
        string thisTag = gameObject.tag;
        gameObject.tag = drop.gameObject.tag;
        drop.gameObject.tag = thisTag;
    }

    private void DraggingInStorage(Slot drop)
    {
        int index = transform.GetSiblingIndex();
        transform.SetSiblingIndex(drop.transform.GetSiblingIndex());
        drop.transform.SetSiblingIndex(index);
    }

    private void DraggingBetweenChestAndInventory(Slot drop)
    {
        int thisIndex = transform.GetSiblingIndex();
        int dropIndex = drop.transform.GetSiblingIndex();

        if (drop.gameObject.CompareTag("ChestSlot"))
        {
            transform.SetParent(SoldierComponents.CurrentChestComponent.Content);
            drop.transform.SetParent(SoldierComponents.InventoryComponent.Content);
        }
        else
        {
            transform.SetParent(SoldierComponents.InventoryComponent.Content);
            drop.transform.SetParent(SoldierComponents.CurrentChestComponent.Content);
        }

        transform.SetSiblingIndex(dropIndex);
        drop.transform.SetSiblingIndex(thisIndex);

        string thisTag = gameObject.tag;
        gameObject.tag = drop.gameObject.tag;
        drop.gameObject.tag = thisTag;
    }

    internal static void StopDragging()
    {
        try
        {
            draggingSlot.bodySlot.SetParent(draggingSlot.parent);
            draggingSlot.eventTarget.raycastTarget = true;
        }
        catch (Exception)
        {
            return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null )
            return;
        StorageSetting.SlotTooltip.ShowTooltip(item.Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StorageSetting.SlotTooltip.HideTooltip();
    }
}
