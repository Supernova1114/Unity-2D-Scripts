using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default_Gun : RangedWeapon
{
    protected override void OnAwake()
    {

    }

    protected override void OnCollect()
    {
        
    }

    protected override void OnOnAttack()
    {
        Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>().AddVelocity(new Vector2(GetOwner().GetVelocity().x, 0));
    }

    

}
