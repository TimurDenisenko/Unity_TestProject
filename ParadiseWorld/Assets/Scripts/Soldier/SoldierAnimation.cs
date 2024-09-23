using UnityEngine;
using Assets.Scripts.Soldier;
using static UnityEngine.ParticleSystem;
using System.Collections;

public class SoldierAnimation : MonoBehaviour
{
    internal Animator animator;
    float jumpHeight;
    Rigidbody rb;
    ParticleSystem dust;
    Animator animatorCombat;
    EmissionModule emission;
    void Awake()
    {
        dust = GetComponentInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();
        animatorCombat = GetComponent<Animator>();
        StaticSoldier.AnimationComponent = this;
        emission = dust.emission;
    }
    void Start()
    {
        rb = StaticSoldier.ControlComponent.rb;
    }
    internal void WalkAnimation()
    {
        if (StaticSoldier.IsActionAnimation())
            return;
        animator.SetBool("Walk", !animator.GetBool("Run"));
        if (animator.GetBool("Walk"))
        {
            if (StaticSoldier.SoldierStatus == SoldierStatus.Walk)
                return;
            StaticSoldier.SoldierStatus = SoldierStatus.Walk;
            emission.rateOverTime = 3;
            animator.PlayAnimation("Walk");
        }
        else if (animator.GetBool("Run"))
        {
            if (StaticSoldier.SoldierStatus == SoldierStatus.Run)
                return;
            StaticSoldier.SoldierStatus = SoldierStatus.Run;
            emission.rateOverTime = 8;
            animator.PlayAnimation("Run");
        }
        if (!dust.isPlaying)
            dust.Play();
    }
    internal void IdleAnimation()
    {
        if (StaticSoldier.SoldierStatus == SoldierStatus.Idle)
            return;
        StaticSoldier.ControlComponent.SetDefaultSpeed();
        StaticSoldier.SoldierStatus = SoldierStatus.Idle;
        animator.SetBool("Walk", false);
        animator.PlayAnimation("Idle");
        if (dust.isPlaying)
            dust.Stop();
    }
    //Баг с с изменением скорости
    internal IEnumerator AttackAnimation()
    {
        StaticSoldier.SoldierStatus = SoldierStatus.Attack;
        emission.rateOverTime = 0;
        animator.PlayAnimation("Attack", layer: 1);
        yield return new WaitForSeconds(1.25f);
        IdleAnimation();
        StaticSoldier.ControlComponent.SetDefaultSpeed();
    }
    //Баг с резким прыжком
    //Баг с с изменением скорости
    internal IEnumerator JumpAnimation()
    {
        jumpHeight = StaticSoldier.ControlComponent.jumpHeight;
        StaticSoldier.SoldierStatus = SoldierStatus.Jump;
        animator.PlayAnimation("Jump");
        if (AnimatorExtension.state == "")
        {
            yield return new WaitForSeconds(0.6f);
            rb.AddForce(Vector3.up * jumpHeight);
            rb.velocity = new Vector3(0, rb.velocity.y * jumpHeight);
            yield return new WaitForSeconds(0.4f);
            rb.AddForce(Vector3.down * jumpHeight);
        }
        else if (AnimatorExtension.state == "Sword")
        {
            yield return new WaitForSeconds(0.2f);
            rb.AddForce(Vector3.up * jumpHeight);
            yield return new WaitForSeconds(0.2f);
            rb.AddForce(Vector3.down * jumpHeight);
        }
        yield return new WaitForSeconds(0.2f);
        IdleAnimation();
    }
    internal IEnumerator EquipmentAnimation()
    {
        animator.SetBool("SwordEquipped", AnimatorExtension.state == "Sword");
        StaticSoldier.SoldierStatus = SoldierStatus.SwordAnimation;
        if (!animator.GetBool("SwordEquipped"))
        {
            animator.PlayAnimation("Withdrawing", false, 1);
            yield return new WaitForSeconds(0.5f);
            StaticSoldier.ControlComponent.SwordWithdrawing();
        }
        else
        {
            animator.PlayAnimation("Sheathing", false, 1);
            yield return new WaitForSeconds(1);
            StaticSoldier.ControlComponent.SwordSheating();
        }
        IdleAnimation();
    }
}
