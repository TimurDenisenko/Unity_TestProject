using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public Item item;
    internal int id;
    Image slotIcon;
    Image eventTarget;
    TMP_Text slotName;
    bool isEquip = false;
    Button btn;
    Transform childrenSlot;
    Transform bodySlot;
    Transform parent;
    internal Slot(Slot slot) 
    {
        item = slot.item;
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
        DeleteSlot();
    }

    private void DeleteSlot()
    {
        item = null;    
        slotIcon.sprite = null;
        slotName.text = null;
        btn.onClick.RemoveAllListeners();
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
        slotName = childrenSlot.GetChild(1).GetComponentInChildren<TMP_Text>();
        if (item != null)
            UpdateSlot();
    }
    internal void UpdateSlot(Item newItem)
    {
        item = newItem;
        SetIcon();
        SetText();
    }
    internal void UpdateSlot()
    {
        SetIcon();
        SetText();
    }
    private void SetIcon() => slotIcon.sprite = item.Icon;
    private void SetText() => slotName.text = item.Name;

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        bodySlot.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        parent = bodySlot.parent;
        bodySlot.SetParent(bodySlot.parent.parent);
        eventTarget.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null) return;
        Slot drop = eventData.pointerEnter?.GetComponent<Slot>() ?? null;
        if (drop != null)
        {
            int index = transform.GetSiblingIndex();
            transform.SetSiblingIndex(drop.transform.GetSiblingIndex());
            drop.transform.SetSiblingIndex(index);
        }
        bodySlot.SetParent(parent);
        eventTarget.raycastTarget = true;
    }
}
