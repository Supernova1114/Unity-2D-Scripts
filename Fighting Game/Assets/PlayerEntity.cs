using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerEntity : Entity
{

    private static PlayerEntity instance;

    [Header("PlayerEntity")]
    [SerializeField] private GameObject playerSpriteObj;
    //[SerializeField] private int attackDamage;
    //[SerializeField] private float knockbackForce;
    //[SerializeField] private Vector2 attackBoxOffset;
    //[SerializeField] private Vector2 attackBoxSize;

    

    public static PlayerEntity GetInstance()
    {
        return instance;
    }

    private void OnAnimate()
    {
        Quaternion targetRotation = Quaternion.Euler(playerSpriteObj.transform.rotation.x, Mathf.Rad2Deg * Mathf.Acos(m_facingDirection), playerSpriteObj.transform.rotation.z);
        playerSpriteObj.transform.rotation = Quaternion.RotateTowards(playerSpriteObj.transform.rotation, targetRotation, 5f);
    }

    protected override void OnDeath()
    {
    }

    protected override void OnHeal()
    {
    }

    protected override void OnHurt()
    {
    }

    

}
