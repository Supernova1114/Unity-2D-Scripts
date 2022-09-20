using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Entity
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private float movementSmooth;
    [SerializeField] private float rotationSmooth;
    [SerializeField] private float targetOffset;
    [SerializeField] private Vector2 targetPosOffset;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float attackInterval;

    [SerializeField] private GameObject spriteObj;

    private Vector2 currentVelocity;
    Vector2 targetDir;

    protected override void OnAwake()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(2);

        while (IsAlive())
        {
            yield return new WaitForSeconds(attackInterval);

            Attack();
        }
        

    }

    private void Update()
    {
        float targetAngle = Mathf.LerpAngle(spriteObj.transform.rotation.eulerAngles.z, Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg, rotationSmooth);
        spriteObj.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void FixedUpdate()
    {
        targetDir = PlayerEntity.GetInstance().transform.position - transform.position;
        Vector2 targetPos = Vector2.SmoothDamp(m_rigidbody.velocity, targetDir - (targetDir.normalized * targetOffset) + targetPosOffset, ref currentVelocity, movementSmooth);
        m_rigidbody.velocity = targetPos;
    }

    public override void Attack()
    {
        Instantiate(bullet, transform.position, spriteObj.transform.rotation);
    }

    protected override void OnDeath()
    {
        Destroy(gameObject);
    }

    protected override void OnHeal()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnHurt()
    {
        
    }

}
