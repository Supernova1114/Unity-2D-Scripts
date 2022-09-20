using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float timeAlive;

    private Rigidbody2D m_rigidbody;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_rigidbody.velocity = transform.right * speed;

        Destroy(gameObject, timeAlive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            entity.HurtKnockback(damage, (collision.transform.position - transform.position).normalized * knockbackForce);
        }

        Destroy(gameObject);
    }

}
