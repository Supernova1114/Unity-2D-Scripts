using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Entity
{
    [Header("Slime Entity")]
    [SerializeField] private float jumpVelocityY;
    [SerializeField] private float airVelocityX;

    [SerializeField] private float jumpInterval;
    [SerializeField] private int jumpAngle;

    [SerializeField] private GameObject spriteObj;
    [SerializeField] private int attackDamage;
    [SerializeField] private float knockbackForce;

    private PlayerEntity playerInstance;
    private bool isOnGround = false;
    private float desiredDirection;

    /// <summary>
    /// OnAwake for the Slime
    /// </summary>
    protected override void OnAwake()
    {
        StartCoroutine(Jumping());
    }

    private void Start()
    {
        playerInstance = PlayerEntity.GetInstance();
    }

    /// <summary>
    /// Coroutine for entity behaviour.
    /// Jumps at a random interval.
    /// </summary>
    /// <returns></returns>
    IEnumerator Jumping()
    {
        yield return null;
        while (IsAlive())
        {
            yield return new WaitForSeconds(jumpInterval + Random.Range(-0.5f, 0.5f));


            if (isOnGround)
            {
                if (playerInstance)
                {
                    desiredDirection = Mathf.Sign((playerInstance.transform.position - transform.position).x);
                }
                else
                {
                    desiredDirection = Random.Range(1, -1) == 0? -1 : 1;
                    print(desiredDirection);
                }

                Vector2 jumpVector = jumpVelocityY * transform.up;
                m_rigidbody.velocity = jumpVector;      
            }
        }
    }


    /// <summary>
    /// Update function.
    /// </summary>
    private void Update()
    {
        OnAnimate();
    }


    /// <summary>
    /// Attack on collide
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Try to get Entity class from top-most parent.
        if (collision.transform.root.TryGetComponent<Entity>(out var entity))
        {
            // If owner is a different team than entity hit, hurt entity hit.
            if (!IsOnSameTeam(entity))
            {
                // knockback left or right
                float knockbackDir = Mathf.Sign((entity.transform.position - transform.position).x);
                entity.HurtKnockback(attackDamage, Vector2.right * knockbackDir * knockbackForce);
            }
        }
    }

    /// <summary>
    /// Dynamic animations for entity.
    /// </summary>
    private void OnAnimate()
    {
        // Squash and stretch of sprite.
        if (m_rigidbody.velocity.y > 0)
        {
            float scaleFactor = m_rigidbody.velocity.y / 10f;

            if (scaleFactor < 0.8f)
            {
                spriteObj.transform.localScale = new Vector3(1f - scaleFactor, 1f + scaleFactor, 1f);
                spriteObj.transform.localPosition = new Vector3(0f, scaleFactor / 2f, 0f);
            }

        }
        else
        {
            spriteObj.transform.localScale = Vector3.one;
        }
    }


    /// <summary>
    /// FixedUpdate
    /// </summary>
    private void FixedUpdate()
    {
        isOnGround = IsOnGround();

        float velocityX;

        if (isOnGround)
        {
            velocityX = 0;
        }
        else
        {
            velocityX = desiredDirection * airVelocityX;
        }

        m_rigidbody.velocity = new Vector2(velocityX, m_rigidbody.velocity.y);
    }


    protected override void OnDeath()
    {
        Destroy(gameObject);
    }

    protected override void OnHurt()
    {
    }

    protected override void OnHeal()
    {
    }

    public override void Attack()
    {

    }
}
