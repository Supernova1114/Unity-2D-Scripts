using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MichaelWolfGames;

public class Slime : Entity
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpInterval;
    [SerializeField] private int jumpAngle;

    [Header("Ground Check")]
    [SerializeField] private GameObject foot;
    [SerializeField] private float footRadius;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private GameObject spriteObj;

    private bool m_isOnGround;


    protected override void OnAwake()
    {
        StartCoroutine(Jumping());
    }


    IEnumerator Jumping()
    {
        while (IsAlive())
        {
            yield return new WaitForSeconds(jumpInterval + Random.Range(-0.5f, 0.5f));

            if (m_isOnGround)
            {
                float angleDeg = Mathf.Deg2Rad * jumpAngle;
                float jumpDirection = Mathf.Sign((PlayerEntity.GetInstance().transform.position - transform.position).x);
                Vector2 jumpVector = jumpForce * new Vector2(jumpDirection * Mathf.Cos(angleDeg), Mathf.Sin(angleDeg));

                Debug.DrawRay(transform.position, jumpVector, Color.red, 1f);

                m_rigidbody.velocity = jumpVector;
            }
        }
    }


    private void Update()
    {
        OnAnimate();
    }

    private void OnAnimate()
    {
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


    private void FixedUpdate()
    {
        // Ground Check
        Collider2D groundCollider = Physics2D.OverlapCircle(foot.transform.position, footRadius, groundMask.value);
        if (groundCollider)
        {
            m_isOnGround = true;
        }
        else
        {
            m_isOnGround = false;
        }
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
