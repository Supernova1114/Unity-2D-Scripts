using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerEntity : Entity
{

    private static PlayerEntity instance;


    public static PlayerEntity GetInstance()
    {
        return instance;
    }

    protected override void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnHeal()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnHurt()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnStart()
    {
        throw new System.NotImplementedException();
    }

    /*[SerializeField] private GameObject playerSpriteObj;

    private void OnAnimate()
    {
        Quaternion targetRotation = Quaternion.Euler(playerSpriteObj.transform.rotation.x, Mathf.Rad2Deg * Mathf.Acos(m_facingDirection), playerSpriteObj.transform.rotation.z);
        playerSpriteObj.transform.rotation = Quaternion.RotateTowards(playerSpriteObj.transform.rotation, targetRotation, 5f);
    }*/

    /*protected override void OnHeal()
    {

    }

    protected override void OnHurt()
    {

    }

    protected override void OnDeath()
    {
        Destroy(gameObject);
    }*/
}
