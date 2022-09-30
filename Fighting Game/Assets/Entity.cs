using MichaelWolfGames;
using UnityEngine;
using Unity.Netcode;

public abstract class Entity : NetworkBehaviour
{
    [Header("Entity")]
    [SerializeField] private int teamNumber;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool hasKnockback = true;
    [SerializeField] private float invincibilityFrameTime;

    [Header("Ground Check")]
    [SerializeField] private GameObject foot;
    [SerializeField] private float m_footRadius; // Radius of the ground check
    [SerializeField] protected LayerMask m_groundMask;
    

    protected ContactFilter2D m_groundContactFilter = new ContactFilter2D();
    private Collider2D[] m_groundContactList = new Collider2D[1];

    private int health;

    private bool isAlive = true;
    private bool isInvincible = false;
    protected Rigidbody2D m_rigidbody;
    protected Collider2D m_collider;


    /// <summary>
    /// OnNetworkSpawn for Entity
    /// </summary>
    public override void OnNetworkSpawn()
    {
        health = maxHealth;
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();

        InitGroundContactFilter();
        OnAwake();
        
        base.OnNetworkSpawn();
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
    

    protected virtual void InitGroundContactFilter()
    {
        // Set up ground contact filter
        m_groundContactFilter.useTriggers = false;
        m_groundContactFilter.useLayerMask = true;
        m_groundContactFilter.layerMask = m_groundMask.value;
    }


    public bool IsOnGround()
    {
        // Ground Check
        // Acceptable ground is not a trigger, on ground layer mask (Default), and does not this entity's rigidbody.

        // Clear ground contact list
        m_groundContactList[0] = null;

        // Check if touching ground
        Physics2D.OverlapCircle(foot.transform.position, m_footRadius, m_groundContactFilter, m_groundContactList);

        return m_groundContactList[0] != null;
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

    public abstract void Attack();
    protected abstract void OnDeath();
    protected abstract void OnHurt();
    protected abstract void OnAwake();
    protected abstract void OnHeal();

}
