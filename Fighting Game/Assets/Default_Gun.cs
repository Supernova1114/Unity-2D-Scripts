using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Default_Gun : RangedWeapon
{
    protected override void OnCollect()
    {

    }

    protected override void OnOnAttack()
    {
        Bullet bulletScript = Instantiate(bullet, transform.position, transform.rotation);
        bulletScript.GetComponent<NetworkObject>().Spawn();
        
        bulletScript.SetOwner(GetOwner());
        bulletScript.AddVelocity(new Vector2(GetOwner().GetVelocity().x, 0));
        
    }

    

}
