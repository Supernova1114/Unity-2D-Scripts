using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MichaelWolfGames;

public class Slime : Entity
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpInterval;
    [SerializeField] private float jumpAngle;

    [Header("Ground Check")]
    [SerializeField] private GameObject foot;
    [SerializeField] private float footRadius;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private GameObject spriteObj;

    [SerializeField] private float knockbackVelocity;

    private Rigidbody2D body;
    private bool m_isOnGround;


    // Start is called before the first frame update
    protected override void OnStart()
    {
        body = GetComponent<Rigidbody2D>();

        StartCoroutine(Jumping());
    }


    IEnumerator Jumping()
    {
        while (IsAlive())
        {
            yield return new WaitForSeconds(jumpInterval + Random.Range(-0.5f, 0.5f));

            Hurt(1);

            if (m_isOnGround)
            {
                float angleDeg = Mathf.Deg2Rad * jumpAngle;
                float jumpDirection = Mathf.Sign((PlayerMovement.GetInstance().transform.position - transform.position).x);
                Vector2 jumpVector = jumpForce * new Vector2(jumpDirection * Mathf.Cos(angleDeg), Mathf.Sin(angleDeg));

                Debug.DrawRay(transform.position, jumpVector, Color.red, 1f);

                body.velocity = jumpVector;
            }
        }
    }


    private void Update()
    {
        Animate();
    }

    
    private void Animate()
    {
        if (body.velocity.y > 0)
        {
            float scaleFactor = body.velocity.y / 10f;
            spriteObj.transform.localScale = new Vector3(1f - scaleFactor, 1f + scaleFactor, 1f);
            spriteObj.transform.localPosition = new Vector3(0f, scaleFactor / 2f, 0f);
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
}
