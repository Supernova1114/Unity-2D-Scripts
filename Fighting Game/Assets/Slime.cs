using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MichaelWolfGames;

public class Slime : Entity
{
    [Header("Slime Entity")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpInterval;
    [SerializeField] private int jumpAngle;
    [SerializeField] private GameObject spriteObj;

    /// <summary>
    /// OnAwake for the Slime
    /// </summary>
    protected override void OnAwake()
    {
        StartCoroutine(Jumping());
    }


    /// <summary>
    /// Coroutine for entity behaviour.
    /// Jumps at a random interval.
    /// </summary>
    /// <returns></returns>
    IEnumerator Jumping()
    {
        while (IsAlive())
        {
            yield return new WaitForSeconds(jumpInterval + Random.Range(-0.5f, 0.5f));

            if (IsOnGround())
            {
                float angleDeg = Mathf.Deg2Rad * jumpAngle;
                float jumpDirection = Mathf.Sign((PlayerEntity.GetInstance().transform.position - transform.position).x);
                Vector2 jumpVector = jumpForce * new Vector2(jumpDirection * Mathf.Cos(angleDeg), Mathf.Sin(angleDeg));

                Debug.DrawRay(transform.position, jumpVector, Color.red, 1f);

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
        throw new System.NotImplementedException();
    }
}
