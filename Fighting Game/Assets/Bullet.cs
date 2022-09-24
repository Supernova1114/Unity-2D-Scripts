using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float timeAlive;

    private Entity ownerEntity;
    private Rigidbody2D m_rigidbody;


    /// <summary>
    /// Awake for Bullet
    /// </summary>
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_rigidbody.velocity = transform.right * speed;

        OnAwake();

        if (timeAlive > 0)
        {
            Destroy(gameObject, timeAlive);
        }

    }


    /// <summary>
    /// Handle bullet collision.
    /// </summary>
    /// <param name="collision">The collider of the object.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isCollisionOwner = false; // If the bullet collided with its owner.

        // Try to get Entity class from top-most parent.
        if (collision.transform.root.TryGetComponent<Entity>(out var entity))
        {
            // If collision is not self
            if (ownerEntity != entity)
            {
                // If owner is a different team than entity hit, hurt entity hit.
                if (ownerEntity.GetTeam() != entity.GetTeam())
                {
                    entity.HurtKnockback(damage, (entity.transform.position - transform.position).normalized * knockbackForce);
                }
            }
            else
            {
                isCollisionOwner = true;
            }
        }

        if (!isCollisionOwner)
        {
            OnCollision(collision);

            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Add velocity to bullet.
    /// </summary>
    /// <param name="velocity">The velocity to add.</param>
    public void AddVelocity(Vector2 velocity)
    {
        m_rigidbody.velocity += velocity;
    }


    public void SetOwner(Entity owner)
    {
        ownerEntity = owner;
    }

    public Entity GetOwner()
    {
        return ownerEntity;
    }

    protected abstract void OnAwake();
    protected abstract void OnCollision(Collider2D collision);
    

}
