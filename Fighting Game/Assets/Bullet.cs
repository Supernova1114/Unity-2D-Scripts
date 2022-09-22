using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float timeAlive;

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
        if (collision.TryGetComponent<Entity>(out var entity))
        {
            entity.HurtKnockback(damage, (entity.transform.position - transform.position).normalized * knockbackForce);
        }

        OnCollision(collision);

        Destroy(gameObject);
    }


    /// <summary>
    /// Add velocity to bullet.
    /// </summary>
    /// <param name="velocity">The velocity to add.</param>
    public void AddVelocity(Vector2 velocity)
    {
        m_rigidbody.velocity += velocity;
    }

    protected abstract void OnAwake();
    protected abstract void OnCollision(Collider2D collision);
    

}
