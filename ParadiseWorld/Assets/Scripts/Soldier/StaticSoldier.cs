
using Assets.Scripts.Soldier;

internal static class StaticSoldier
{
    internal static SoldierControl ControlComponent;
    internal static SoldierAttack AttackComponent;
    internal static SoldierAnimation AnimationComponent;
    internal static CameraControl CameraComponent;
    internal static Inventory Inventory;
    internal static Chest CurrentChest;
    internal static SoldierStatus SoldierStatus;
    internal static bool IsActionAnimation() =>
        SoldierStatus == SoldierStatus.Jump || SoldierStatus == SoldierStatus.Attack || SoldierStatus == SoldierStatus.SwordAnimation;
    internal static void RestartAnimation() => 
        AnimatorExtension.StopAnimation(AnimationComponent.animator);
}
