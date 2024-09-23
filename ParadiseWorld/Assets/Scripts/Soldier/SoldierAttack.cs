using Assets.Scripts.Soldier;
using UnityEngine;
using UnityEngine.UI;

public class SoldierAttack : MonoBehaviour
{
    [SerializeField] public float attack = 1f;
    [SerializeField] public float health = 10f;
    [SerializeField] public float restoreHealthSpeed = 0.01f;
    [SerializeField] public float stamina = 10f;
    [SerializeField] public float restoreStaminaSpeed = 0.1f;
    [SerializeField] public float mana = 10f;
    [SerializeField] public float restoreManaSpeed = 0.1f;
    [SerializeField] public Image healthUI;
    [SerializeField] public Image staminaUI;
    [SerializeField] public Image manaUI;
    float totalHealth, totalStamina, totalMana;
    internal bool isAlive = true;
    private void Awake()
    {
        totalHealth = health;
        totalStamina = stamina;
        totalMana = mana;
        StaticSoldier.AttackComponent = this;
    }
    internal void UseMana(float usedMana)
    {
        mana -= usedMana;
        manaUI.color = new Color(1 - mana / totalMana * 100 / 255, 1 - mana / totalMana * 100 / 255, 1, 1);
    }
    internal void RestoreMana()
    {
        if (mana < totalMana)
        {
            mana += restoreManaSpeed;
            manaUI.color = new Color(1 - mana / totalMana * 100 / 255, 1 - mana / totalMana * 100 / 255, 1, 1);
        }
    }
    internal void RestoreMana(float recovery)
    {
        mana += recovery;
        Mathf.Clamp(mana, 0, totalMana);
        manaUI.color = new Color(1 - mana / totalMana * 100 / 255, 1 - mana / totalMana * 100 / 255, 1, 1);
    }
    internal void RestoreHealth()
    {
        if (health < totalHealth)
        {
            health += restoreHealthSpeed;
            healthUI.fillAmount = health / totalStamina;
        }
    }
    internal void RestoreHealth(float recovery)
    {
        health += recovery;
        Mathf.Clamp(health, 0, totalHealth);
        healthUI.fillAmount = health / totalStamina;
    }
    internal void RestoreStamina()
    {
        if (stamina < totalStamina)
        {
            stamina += restoreStaminaSpeed;
            staminaUI.fillAmount = stamina / totalStamina;
        }
    }
    internal void RestoreStamina(float recovery)
    {
        stamina += recovery;
        Mathf.Clamp(stamina, 0, totalStamina);
        staminaUI.fillAmount = stamina / totalStamina;
    }
    internal void UseStamina(float usedStamina)
    {
        stamina -= usedStamina;
        staminaUI.fillAmount = stamina / totalStamina;
    }
    internal void DealDamage(GameObject collision)
    {
        if (collision.CompareTag("Enemy") && StaticSoldier.SoldierStatus == SoldierStatus.Attack)
        {
            EnemyAttack enemy = collision.GetComponent<EnemyAttack>();
            enemy.ReceiveDamage(attack);
        }
    }
    internal void ReceiveDamage(float damage)
    {
        health -= damage;
        healthUI.fillAmount = health / totalHealth;
        if (health <= 0f)
            Death();
    }
    private void Death()
    {
        isAlive = false;
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DealDamage(collision.gameObject);
    }
}
