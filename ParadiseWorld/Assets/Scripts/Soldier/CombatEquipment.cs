using Assets.Scripts.Soldier;
using Unity.VisualScripting;
using UnityEngine;

public class CombatEquipment : MonoBehaviour
{
    [SerializeField] public Transform Content;
    [SerializeField] Slot[] artificalSlot;
    public static Sprite EmptySwordSlot;
    internal GameObject currentSword;
    internal Slot currentSwordSlot;
    private bool isSwordEquipped = false;
    private void Awake()
    {
        SoldierComponents.CombatEquipmentComponent = this;
        foreach (Slot slot in artificalSlot)
        {
            slot.LoadComponent();
            if (slot.CompareTag("SwordSlot"))
                EmptySwordSlot = slot.slotIcon.sprite;
        }
    }
    internal void SwordWithdrawing()
    {
        ChangeSwordTransform(SoldierComponents.ControlComponent.activeSword);
        AnimatorExtension.state = "Sword";
        SoldierComponents.SoldierStatus = SoldierStatus.SwordAnimation;
        if (currentSwordSlot.item is Sword sword)
        {
            SoldierComponents.AttackComponent.attack = sword.attack;
            SoldierComponents.AttackComponent.staminaConsumption = sword.staminaConsumption;
        }
    }
    internal void SwordSheating()
    {
        if (currentSword != null)
            ChangeSwordTransform(SoldierComponents.ControlComponent.passiveSword);
        AnimatorExtension.state = "";
        SoldierComponents.RestartAnimation();
        SoldierComponents.AttackComponent.SetDefaultAttack();
        SoldierComponents.AttackComponent.SetDefaultStaminaConsumption();
    }
    public void EquipSword(Slot swordSlot, GameObject swordobject)
    {
        if (currentSword != null)
            Destroy(currentSword.gameObject);
        currentSword = Instantiate(swordobject, SoldierComponents.ControlComponent.passiveSword);
        currentSwordSlot = swordSlot;
        ChangeSwordTransform(SoldierComponents.ControlComponent.passiveSword);
        AnimatorExtension.state = "";
        SoldierComponents.RestartAnimation();
    }
    public void ClearSword()
    {
        Destroy(currentSword);
        currentSword = null;
        SoldierComponents.AnimationComponent.animator.SetBool("SwordEquipped", false);
        SwordSheating();
    }
    private void ChangeSwordTransform(Transform sword)
    {
        currentSword.transform.SetParent(sword);
        currentSword.transform.localPosition = Vector3.zero;
        currentSword.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentSword.transform.localScale = new Vector3(1, 1, 1);
    }
}