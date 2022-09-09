using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpInterval;
    [SerializeField] private float jumpAngle;

    private Rigidbody2D body;
    private bool isAlive;
    private bool m_isOnGround;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        isAlive = true;
        
        StartCoroutine(Jumping());
    }

    
    IEnumerator Jumping()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(jumpInterval);

            // Ground Check
            Collider2D groundCollider = Physics2D.OverLapBox(m_foot.transform.position, m_footRadius, m_groundMask.value);
            if (groundCollider)
            {
                m_isOnGround = true;
            }
            else
            {
                m_isOnGround = false;
            }

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

}
