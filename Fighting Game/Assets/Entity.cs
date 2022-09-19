using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int health;

    private bool isAlive = true;


    private void Start()
    {
        health = maxHealth;
        OnStart();
    }

    public void Knockback(Vector2 force)
    {

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

    protected abstract void OnDeath();
    protected abstract void OnHurt();
    protected abstract void OnStart();
    protected abstract void OnHeal();

}
