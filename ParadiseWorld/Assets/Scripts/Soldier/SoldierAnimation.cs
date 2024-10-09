using UnityEngine;
using Assets.Scripts.Soldier;
using static UnityEngine.ParticleSystem;
using System.Collections;

public class SoldierAnimation : MonoBehaviour
{
    internal Animator animator;
    ParticleSystem dust;
    EmissionModule emission;
    void Awake()
    {
        dust = GetComponentInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();
        SoldierComponents.AnimationComponent = this;
        emission = dust.emission;
    }
    internal void WalkAnimation()
    {
        if (SoldierComponents.IsActionAnimation())
            return;
        animator.SetBool("Walk", !animator.GetBool("Run"));
        if (animator.GetBool("Walk"))
        {
            if (SoldierComponents.SoldierStatus == SoldierStatus.Walk)
                return;
            SoldierComponents.SoldierStatus = SoldierStatus.Walk;
            emission.rateOverTime = 3;
            animator.PlayAnimation("Walk");
        }
        else if (animator.GetBool("Run"))
        {
            if (SoldierComponents.SoldierStatus == SoldierStatus.Run)
                return;
            SoldierComponents.SoldierStatus = SoldierStatus.Run;
            emission.rateOverTime = 8;
            animator.PlayAnimation("Run");
        }
        if (!dust.isPlaying)
            dust.Play();
    }
    internal void IdleAnimation()
    {
        if (SoldierComponents.SoldierStatus == SoldierStatus.Idle)
            return;
        SoldierComponents.ControlComponent.SetDefaultSpeed();
        SoldierComponents.SoldierStatus = SoldierStatus.Idle;
        animator.SetBool("Walk", false);
        animator.PlayAnimation("Idle");
        if (dust.isPlaying)
            dust.Stop();
    }
    //Баг с с изменением скорости
    internal IEnumerator AttackAnimation()
    {
        SoldierComponents.SoldierStatus = SoldierStatus.Attack;
        emission.rateOverTime = 0;
        animator.PlayAnimation("Attack", layer: 1);
        yield return new WaitForSeconds(1.25f);
        IdleAnimation();
        SoldierComponents.ControlComponent.SetDefaultSpeed();
    }
    //Баг с резким прыжком
    //Баг с с изменением скорости
    internal IEnumerator JumpAnimation()
    {
        SoldierComponents.SoldierStatus = SoldierStatus.Jump;
        animator.PlayAnimation("Jump");
        if (AnimatorExtension.state == "")
        {
            yield return new WaitForSeconds(1f);
        }
        else if (AnimatorExtension.state == "Sword")
        {
            yield return new WaitForSeconds(0.4f);
        }
        yield return new WaitForSeconds(0.2f);
        IdleAnimation();
    }
    internal IEnumerator EquipmentAnimation()
    {
        animator.SetBool("SwordEquipped", AnimatorExtension.state == "Sword");
        SoldierComponents.SoldierStatus = SoldierStatus.SwordAnimation;
        if (!animator.GetBool("SwordEquipped"))
        {
            animator.PlayAnimation("Withdrawing", false, 1);
            yield return new WaitForSeconds(0.5f);
            SoldierComponents.CombatEquipmentComponent.SwordWithdrawing();
        }
        else
        {
            animator.PlayAnimation("Sheathing", false, 1);
            yield return new WaitForSeconds(1);
            SoldierComponents.CombatEquipmentComponent.SwordSheating();
        }
        IdleAnimation();
    }
}
