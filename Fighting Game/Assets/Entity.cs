using MichaelWolfGames;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private int teamNumber;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool hasKnockback = true;
    [SerializeField] private float invincibilityFrameTime;

    [Header("Ground Check")]
    [SerializeField] protected GameObject foot;
    [SerializeField] protected float footRadius;
    [SerializeField] protected LayerMask groundMask;
    protected ContactFilter2D groundContactFilter = new();
    protected Collider2D[] groundContactList = new Collider2D[5];

    private int health;

    private bool isAlive = true;
    private bool isInvincible = false;
    protected Rigidbody2D m_rigidbody;
    protected Collider2D m_collider;


    /// <summary>
    /// Awake
    /// </summary>
    /// 
    private void Awake()
    {
        health = maxHealth;
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();

        InitGroundContactFilter();

        OnAwake();
    }


    /// <summary>
    /// Hurt and knockback entity.
    /// </summary>
    /// <param name="damage">The amount of damage to apply to entity.</param>
    /// <param name="addVelocity">The knockback velocity to apply to entity.</param>
    public void HurtKnockback(int damage, Vector2 addVelocity)
    {
        if (!isInvincible)
        {
            if (invincibilityFrameTime > 0)
            {
                isInvincible = true;
                this.StartTimer(invincibilityFrameTime, () => isInvincible = false);
            }

            Knockback(addVelocity);
            Hurt(damage);
        }
    }


    /// <summary>
    /// Apply knockback to entity using addition of velocity.
    /// </summary>
    /// <param name="addVelocity">The knockback velocity to apply to entity.</param>
    public void Knockback(Vector2 addVelocity)
    {
        if (hasKnockback)
        {
            m_rigidbody.velocity += addVelocity;
        }
    }


    /// <summary>
    /// Apply damage to entity.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    private void Hurt(int damage)
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


    /// <summary>
    /// Kill the entity.
    /// </summary>
    public void Kill()
    {
        isAlive = false;
        health = 0;

        OnDeath();
    }


    /// <summary>
    /// Heal the entity.
    /// </summary>
    /// <param name="amount">The amount of health points to heal.</param>
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        OnHeal();
    }


    /// <summary>
    /// Heal entity to max health.
    /// </summary>
    public void HealMax()
    {
        health = maxHealth;
        OnHeal();
    }




    public bool IsOnSameTeam(Entity otherEntity)
    {
        return teamNumber == otherEntity.teamNumber;
    }


    /// <summary>
    /// If the entity is alive or not.
    /// </summary>
    /// <returns></returns>
    public bool IsAlive()
    {
        return isAlive;
    }

    public Vector2 GetVelocity()
    {
        return m_rigidbody.velocity;
    }

    public int GetTeam()
    {
        return teamNumber;
    }

    protected virtual void InitGroundContactFilter()
    {
        // Set up ground contact filter
        groundContactFilter.useLayerMask = true;
        groundContactFilter.layerMask = groundMask;
    }

    // Default ground check. Can be overriden
    protected virtual bool IsOnGround()
    {
        // Check if touching ground
        int results = Physics2D.OverlapCircle(foot.transform.position, footRadius, groundContactFilter, groundContactList);
        return results > 0;
    }

    public abstract void Attack();
    protected abstract void OnDeath();
    protected abstract void OnHurt();
    protected abstract void OnAwake();
    protected abstract void OnHeal();
}
