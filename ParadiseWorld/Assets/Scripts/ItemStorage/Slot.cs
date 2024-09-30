using System;
using System.Reflection;
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
    Image slotIcon;
    TMP_Text slotCount;
    Image eventTarget;
    bool isEquip = false;
    Button btn;
    Transform canvasForDraggingItem;
    Transform childrenSlot;
    Transform bodySlot;
    Transform parent;
    private void Start()
    {
        canvasForDraggingItem = StaticSoldier.Inventory.canvasForDraggingItem;
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
                StaticSoldier.AttackComponent.RestoreHealth(potion.recovery);
                break;
            case PotionType.Stamina:
                StaticSoldier.AttackComponent.RestoreStamina(potion.recovery);
                break;
            case PotionType.Mana:
                StaticSoldier.AttackComponent.RestoreMana(potion.recovery);
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
            StaticSoldier.ControlComponent.ClearSword();
            isEquip = false;
        }
        else
        {
            if (StaticSoldier.ControlComponent.currentSword != null)
                StaticSoldier.ControlComponent.currentSwordSlot.isEquip = false;
            StaticSoldier.ControlComponent.EquipSword(this);
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
        if (item.IsStackable)
            slotCount.text = count.ToString();
    } 

    internal void UpdateSlot() 
    {
        SetIcon();
        SetCount();
    }
    private void SetIcon() => slotIcon.sprite = item.Icon;

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
        bodySlot.SetParent(canvasForDraggingItem);
        eventTarget.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null) return;
        Slot drop = eventData.pointerEnter?.GetComponent<Slot>() ?? null;
        if (drop != null)
        {
            if (drop.gameObject.CompareTag("ChestSlot") && gameObject.CompareTag("InventorySlot") || drop.gameObject.CompareTag("InventorySlot") && gameObject.CompareTag("ChestSlot"))
            {
                int thisIndex = transform.GetSiblingIndex();
                int dropIndex = drop.transform.GetSiblingIndex();

                if (drop.gameObject.CompareTag("ChestSlot") && gameObject.CompareTag("InventorySlot"))
                {
                    Transform tempContent = StaticSoldier.Inventory.Content;
                    transform.SetParent(StaticSoldier.CurrentChest.Content);
                    drop.transform.SetParent(tempContent);
                }
                else
                {
                    Transform tempContent = StaticSoldier.CurrentChest.Content;
                    transform.SetParent(StaticSoldier.Inventory.Content);
                    drop.transform.SetParent(tempContent);
                }

                transform.SetSiblingIndex(dropIndex);
                drop.transform.SetSiblingIndex(thisIndex);

                string thisTag = gameObject.tag;
                gameObject.tag = drop.gameObject.tag;
                drop.gameObject.tag = thisTag;
            }
            else
            {
                int index = transform.GetSiblingIndex();
                transform.SetSiblingIndex(drop.transform.GetSiblingIndex());
                drop.transform.SetSiblingIndex(index);
            }
        }
        StopDragging();
    }
    internal static void StopDragging()
    {
        draggingSlot.bodySlot.SetParent(draggingSlot.parent);
        draggingSlot.eventTarget.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null )
            return;
        StaticSoldier.Inventory.tooltip.ShowTooltip(item.Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StaticSoldier.Inventory.tooltip.HideTooltip();
    }
}
