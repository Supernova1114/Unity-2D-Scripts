using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private int maxHealth;
    [SerializeField] private bool hasKnockback = true;
    private int health;

    private bool isAlive = true;
    protected Rigidbody2D m_rigidbody;


    private void Awake()
    {
        health = maxHealth;
        m_rigidbody = GetComponent<Rigidbody2D>();
        OnAwake();
    }

    public void HurtKnockback(int damage, Vector2 addVelocity)
    {
        Knockback(addVelocity);
        Hurt(damage);
    }

    public void Knockback(Vector2 addVelocity)
    {
        if (hasKnockback)
        {
            m_rigidbody.velocity += addVelocity;
        }
    }

    public void Hurt(int damage)
    {
        if (isAlive)
        {
            health -= damage;

            if (health <= 0)
            {
                Kill();
            }
            else
            {
                OnHurt();
            }
        }
    }

    public void Kill()
    {
        isAlive = false;
        health = 0;
        
        OnDeath();
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        OnHeal();
    }

    public void HealMax()
    {
        health = maxHealth;
        OnHeal();
    }
    
    public bool IsAlive()
    {
        return isAlive;
    }

    public abstract void Attack();
    protected abstract void OnDeath();
    protected abstract void OnHurt();
    protected abstract void OnAwake();
    protected abstract void OnHeal();

}
