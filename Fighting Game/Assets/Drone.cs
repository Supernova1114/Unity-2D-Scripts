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

    PlayerEntity playerInstance;

    protected override void OnAwake()
    {
        StartCoroutine(AttackCoroutine());
    }

    private void Start()
    {
        playerInstance = PlayerEntity.GetInstance();
    }

    /// <summary>
    /// Coroutine for attack logic.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(2);

        while (IsAlive())
        {
            yield return new WaitForSeconds(attackInterval);

            Attack();
        }
    }


    /// <summary>
    /// Update function for Drone
    /// </summary>
    private void Update()
    {
        float targetAngle = Mathf.LerpAngle(spriteObj.transform.rotation.eulerAngles.z, Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg, rotationSmooth);
        spriteObj.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }


    /// <summary>
    /// FixedUpdate for Drone
    /// </summary>
    private void FixedUpdate()
    {
        if (playerInstance)
        {
            targetDir = playerInstance.transform.position - transform.position;
            Vector2 targetPos = Vector2.SmoothDamp(m_rigidbody.velocity, (targetDir - (targetDir.normalized * targetOffset) + targetPosOffset) * moveSpeed, ref currentVelocity, movementSmooth);
            m_rigidbody.velocity = targetPos;
        }
        else
        {
            m_rigidbody.velocity = Vector2.zero;
        }
    }


    /// <summary>
    /// Attack logic for drone. Fires a bullet.
    /// </summary>
    public override void Attack()
    {
        Instantiate(bullet, transform.position, spriteObj.transform.rotation).GetComponent<Bullet>().SetOwner(this);
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
