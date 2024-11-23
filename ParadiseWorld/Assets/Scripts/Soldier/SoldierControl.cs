using Assets.Scripts.Soldier;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoldierControl : MonoBehaviour
{
    [Header("Camera"), Space(5)]
    [SerializeField] Transform lookAt;
    [SerializeField, Tooltip("X and Z")] Vector2 offset;
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
    [Header("Sword"), Space(5)]
    [SerializeField] public Transform activeSword;
    [SerializeField] public Transform passiveSword;
    [Space(10)]
    [Header("Characteristics"), Space(5)]
    [SerializeField] float characterSlowing = 10;
    [SerializeField] float characterAcceleration = 2;
    [SerializeField] float rotateSpeed = 0.1f;
    internal Rigidbody rb;
    float acceleration = 1f;
    float bAcceleration = 1f;
    void Awake()
    {
        SoldierComponents.ControlComponent = this;
        rb = GetComponent<Rigidbody>();
        AnimatorExtension.state = "";
        EventsAssigment();
    }
    void Update()
    {
        if (SoldierComponents.SoldierStatus == SoldierStatus.Run)
        {
            SoldierComponents.AttackComponent.UseStamina(0.01f);
            if (SoldierComponents.AttackComponent.stamina < 1)
            {
                StopRunning();
            }
        }
        else
        {
            SoldierComponents.AttackComponent.RestoreStamina();
        }
        SoldierComponents.AttackComponent.RestoreHealth();
        SoldierComponents.AttackComponent.RestoreMana();
    }
    void LateUpdate()
    {
        Vector2 action = movemenet.ReadValue<Vector2>();
        if (action == Vector2.zero && !SoldierComponents.IsActionAnimation())
        {
            SoldierComponents.AnimationComponent.IdleAnimation();
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
    internal void DisableControlInputs()
    {
        switch (SoldierComponents.CurrentUI)
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
    internal void EnableControlInputs()
    {
        Array.ForEach(new InputAction[] { movemenet, fastRun, attack, equipment, jump, spell, equipmentWindow, inventoryWindow }, x => x.Enable());
    }
    private void Movement(Vector2 action)
    {
        SoldierComponents.AnimationComponent.WalkAnimation();
        ChangeCharacterPosition(action / characterSlowing);
        ChangeLookPosition();
    }

    private void ChangeLookPosition() =>
        lookAt.position = new Vector3(transform.position.x + offset.x, lookAt.position.y, transform.position.z + offset.y);

    private void ChangeCharacterPosition(Vector2 speed)
    {
        Vector3 movement = acceleration * (new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * speed.y + Camera.main.transform.right * speed.x);
        rb.linearVelocity = movement;
    }
    private void RotateCharacter(Vector2 action)
    {
        if (action == Vector2.zero && SoldierComponents.SoldierStatus == SoldierStatus.Attack)
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
        spell.started += Spell_started;
        inventoryWindow.started += (obj) => SoldierComponents.InterfaceComponent.InventoryWindow();
        equipmentWindow.started += (obj) => SoldierComponents.InterfaceComponent.CombatEquipmentWindow();
    }

    internal void SetDefaultSpeed()
    {
        acceleration = bAcceleration;
    }
    private void Spell_started(InputAction.CallbackContext obj)
    {
        SoldierComponents.AttackComponent.UseMana(1);
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        if (!SoldierComponents.IsActionAnimation())
        {
            bAcceleration = acceleration;
            acceleration = 0.5f;
            StartCoroutine(SoldierComponents.AnimationComponent.JumpAnimation());
        }
    }
    private void Equipment_started(InputAction.CallbackContext obj)
    {
        if (!SoldierComponents.IsActionAnimation() && SoldierComponents.CombatEquipmentComponent.currentSword != null)
        {
            bAcceleration = acceleration;
            acceleration = 0.5f;
            StartCoroutine(SoldierComponents.AnimationComponent.EquipmentAnimation());
        }
        acceleration = bAcceleration;
    }
    

    private void Attack_started(InputAction.CallbackContext obj)
    {
        if (SoldierComponents.AttackComponent.stamina < 1 || (SoldierComponents.AttackComponent.stamina-5) < 1)
            return;
        SoldierComponents.AttackComponent.UseStamina(SoldierComponents.AttackComponent.staminaConsumption);
        bAcceleration = acceleration;
        acceleration = 0.5f;
        StartCoroutine(SoldierComponents.AnimationComponent.AttackAnimation());
    }
    private void StopRunning()
    {
        SoldierComponents.AnimationComponent.animator.SetBool("Run", false);
        acceleration = 1;
    }

    private void FastRun_started(InputAction.CallbackContext obj)
    {
        SoldierComponents.AnimationComponent.animator.SetBool("Run", true);
        acceleration = characterAcceleration;
    }
}