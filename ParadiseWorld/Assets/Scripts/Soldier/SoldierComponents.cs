
using Assets.Scripts.Soldier;

internal static class SoldierComponents
{
    internal static SoldierControl ControlComponent;
    internal static SoldierAttack AttackComponent;
    internal static SoldierAnimation AnimationComponent;
    internal static CameraControl CameraComponent;
    internal static Inventory InventoryComponent;
    internal static SoldierUI InterfaceComponent;
    internal static CombatEquipment CombatEquipmentComponent;
    internal static Chest CurrentChestComponent;
    internal static SoldierStatus SoldierStatus;
    internal static UIType CurrentUI;
    internal static bool IsActionAnimation() =>
        SoldierStatus == SoldierStatus.Jump || SoldierStatus == SoldierStatus.Attack || SoldierStatus == SoldierStatus.SwordAnimation;
    internal static void RestartAnimation() => 
        AnimatorExtension.StopAnimation(AnimationComponent.animator);
}
