using Assets.Scripts;
using Assets.Scripts.Soldier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class SoldierControl : MonoBehaviour
{
    [Header("Camera"), Space(5)]
    [SerializeField] Transform lookAt;
    [SerializeField][Tooltip("X and Z")] Vector2 offset;
    [Space(10)]
    [Header("Inputs"), Space(5)]
    [SerializeField] InputAction movemenet;
    [SerializeField] InputAction jump;
    [SerializeField] InputAction fastRun;
    [SerializeField] InputAction attack;
    [SerializeField] InputAction equipment;
    [SerializeField] InputAction inventoryWindow;
    [SerializeField] InputAction equipmentWindow;
    [SerializeField] InputAction spell;
    [Space(10)]
    [Header("Inventory"), Space(5)]
    [SerializeField] internal GameObject equipmentCanvas;
    [SerializeField] internal GameObject inventoryCanvas;
    [SerializeField] internal GameObject currentSword;
    [SerializeField] internal Slot currentSwordSlot;
    [SerializeField] Transform activeSword;
    [SerializeField] Transform passiveSword;
    [Space(10)]
    [Header("Characteristics"), Space(5)]
    [SerializeField] float characterSlowing = 10;
    [SerializeField] float characterAcceleration = 2;
    [SerializeField] internal float jumpHeight = 1f;
    [SerializeField] float rotateSpeed = 0.1f;
    internal Rigidbody rb;
    float acceleration = 1f;
    float bAcceleration = 1f;
    float hitStamina = 1f;
    float defaultAttack;
    void Awake()
    {
        StaticSoldier.ControlComponent = this;
        rb = GetComponent<Rigidbody>();
        AnimatorExtension.state = "";
        EventsAssigment();
    }
    private void Start()
    {
        defaultAttack = StaticSoldier.AttackComponent.attack;
    }
    void Update()
    {
        if (StaticSoldier.SoldierStatus == SoldierStatus.Run)
        {
            StaticSoldier.AttackComponent.UseStamina(0.01f);
            if (StaticSoldier.AttackComponent.stamina < 1)
            {
                StopRunning();
            }
        }
        else
        {
            StaticSoldier.AttackComponent.RestoreStamina();
        }
        StaticSoldier.AttackComponent.RestoreHealth();
        StaticSoldier.AttackComponent.RestoreMana();
    }
    void LateUpdate()
    {
        Vector2 action = movemenet.ReadValue<Vector2>();
        if (action == Vector2.zero && !StaticSoldier.IsActionAnimation())
        {
            StaticSoldier.AnimationComponent.IdleAnimation();
        }
        else
        {
            Movement(action);
            RotateCharacter(action);
        }
    }
    void OnEnable()
    {
        EnableControlInputs();
    }
    void OnDisable()
    {
        DisableControlInputs();
    }
    private void DisableControlInputs()
    {
        switch (StaticSoldier.CurrentUI)
        {
            case UIType.None:
                Array.ForEach(new InputAction[] { equipmentWindow, inventoryWindow }, x => x.Disable());
                break;
            case UIType.Inventory:
                Array.ForEach(new InputAction[] { equipmentWindow }, x => x.Disable());
                break;
            case UIType.Chest:
                Array.ForEach(new InputAction[] { equipmentWindow, inventoryWindow }, x => x.Disable());
                break;
            case UIType.Equipment:
                Array.ForEach(new InputAction[] { inventoryWindow }, x => x.Disable());
                break;
            default:
                break;
        }
        Array.ForEach(new InputAction[] { movemenet, fastRun, attack, equipment, jump, spell }, x => x.Disable());
    }
    private void EnableControlInputs()
    {
        Array.ForEach(new InputAction[] { movemenet, fastRun, attack, equipment, jump, spell, equipmentWindow, inventoryWindow }, x => x.Enable());
    }
    private void Movement(Vector2 action)
    {
        StaticSoldier.AnimationComponent.WalkAnimation();
        ChangeCharacterPosition(action / characterSlowing);
        ChangeLookPosition();
    }

    private void ChangeLookPosition() =>
        lookAt.position = new Vector3(transform.position.x + offset.x, lookAt.position.y, transform.position.z + offset.y);

    private void ChangeCharacterPosition(Vector2 speed)
    {
        Vector3 movement = acceleration * (new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * speed.y + Camera.main.transform.right * speed.x);
        rb.velocity = movement;
    }
    private void RotateCharacter(Vector2 action)
    {
        if (action == Vector2.zero && StaticSoldier.SoldierStatus == SoldierStatus.Attack)
            return;
        int xAction = Mathf.RoundToInt(action.x);
        int yAction = Mathf.RoundToInt(action.y);
        float rotate = Camera.main.transform.rotation.eulerAngles.y;
        if (yAction != 0 && xAction != 0)
        {
            if (yAction == -1)
                rotate += 135 * xAction;
            else
                rotate += 45 * xAction;
        }
        else
        {
            if (yAction != 0)
                rotate += Mathf.RoundToInt(yAction) == 1 ? 0 : 180;
            if (xAction != 0)
                rotate += 90 * xAction;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, rotate, transform.rotation.z), rotateSpeed);
    }
    private void EventsAssigment()
    {
        fastRun.started += FastRun_started;
        fastRun.canceled += (obj) => StopRunning();
        attack.started += Attack_started;
        equipment.started += Equipment_started;
        jump.started += Jump_started;
        inventoryWindow.started += InventoryWindow_started;
        spell.started += Spell_started;
        equipmentWindow.started += EquipmentWindow_started;
    }

    private void EquipmentWindow_started(InputAction.CallbackContext obj)
    {
        StaticSoldier.CurrentUI = UIType.Equipment;
        StorageUI(equipmentCanvas);
        StorageUI(StaticSoldier.ControlComponent.inventoryCanvas);
        if (!equipmentCanvas.activeSelf)
        {
            StaticSoldier.Inventory.SetFirstUI();
            StaticSoldier.Inventory.SetDefaultInventory();
        }
        else
        {
            StaticSoldier.Inventory.SetSecondUI(false);
            StaticSoldier.Inventory.SortBy(typeof(Sword));
        }
    }

    internal void SetDefaultSpeed()
    {
        acceleration = bAcceleration;
    }
    private void Spell_started(InputAction.CallbackContext obj)
    {
        StaticSoldier.AttackComponent.UseMana(1);
    }

    private void InventoryWindow_started(InputAction.CallbackContext obj)
    {
        StaticSoldier.CurrentUI = UIType.Inventory;
        StorageUI(inventoryCanvas);
    }

    internal void StorageUI(GameObject canvas)
    {
        if (canvas.activeSelf)
        {
            StaticSoldier.CurrentUI = UIType.None;
            canvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            StaticSoldier.CameraComponent.enabled = true;
            StorageSetting.SlotTooltip.HideTooltip();
            EnableControlInputs();
        }
        else
        {
            canvas.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            StaticSoldier.CameraComponent.enabled = false;
            DisableControlInputs();
        }
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        if (!StaticSoldier.IsActionAnimation())
        {
            bAcceleration = acceleration;
            acceleration = 0.5f;
            StartCoroutine(StaticSoldier.AnimationComponent.JumpAnimation());
        }
    }
    private void Equipment_started(InputAction.CallbackContext obj)
    {
        if (!StaticSoldier.IsActionAnimation() && currentSword != null)
        {
            bAcceleration = acceleration;
            acceleration = 0.5f;
            StartCoroutine(StaticSoldier.AnimationComponent.EquipmentAnimation());
        }
        acceleration = bAcceleration;
    }
    internal void SwordWithdrawing()
    {
        ChangeSwordTransform(activeSword);
        AnimatorExtension.state = "Sword";
        StaticSoldier.SoldierStatus = SoldierStatus.SwordAnimation;
        if (currentSwordSlot.item is Sword sword)
        {
            StaticSoldier.AttackComponent.attack = sword.attack;
            hitStamina = sword.staminaConsumption;
        }
    }
    internal void SwordSheating()
    {
        if (currentSword != null)
            ChangeSwordTransform(passiveSword);
        AnimatorExtension.state = "";
        StaticSoldier.RestartAnimation();
        StaticSoldier.AttackComponent.attack = defaultAttack;
        hitStamina = 1f;
    }
    public void EquipSword(Slot swordSlot)
    {
        if (currentSword != null)
            Destroy(currentSword.gameObject);
        currentSword = Instantiate(((Sword)swordSlot.item).SwordObject, passiveSword);
        currentSwordSlot = swordSlot;
        ChangeSwordTransform(passiveSword);
        AnimatorExtension.state = "";
        StaticSoldier.RestartAnimation();
    }
    public void ClearSword()
    {
        Destroy(currentSword);
        currentSword = null;
        StaticSoldier.AnimationComponent.animator.SetBool("SwordEquipped", false);
        SwordSheating();
    }
    private void ChangeSwordTransform(Transform sword)
    {
        currentSword.transform.SetParent(sword);
        currentSword.transform.localPosition = Vector3.zero;
        currentSword.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentSword.transform.localScale = new Vector3(1, 1, 1);
    }

    private void Attack_started(InputAction.CallbackContext obj)
    {
        if (StaticSoldier.AttackComponent.stamina < 1 || (StaticSoldier.AttackComponent.stamina-5) < 1)
            return;
        StaticSoldier.AttackComponent.UseStamina(hitStamina);
        bAcceleration = acceleration;
        acceleration = 0.5f;
        StartCoroutine(StaticSoldier.AnimationComponent.AttackAnimation());
    }
    private void StopRunning()
    {
        StaticSoldier.AnimationComponent.animator.SetBool("Run", false);
        acceleration = 1;
    }

    private void FastRun_started(InputAction.CallbackContext obj)
    {
        StaticSoldier.AnimationComponent.animator.SetBool("Run", true);
        acceleration = characterAcceleration;
    }
}