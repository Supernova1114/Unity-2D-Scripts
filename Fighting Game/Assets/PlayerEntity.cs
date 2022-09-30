using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerEntity : Entity
{
    [Header("PlayerEntity")]
    [SerializeField] private GameObject playerSpriteObj;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject itemObj = collision.transform.root.gameObject;
        if (itemObj.CompareTag("CollectibleItem"))
        {
            itemObj.GetComponent<CollectibleItem>().Collect(this);
        }
    }

    protected override void OnDeath()
    {
        print("Bleh");
    }

    protected override void OnHeal()
    {
    }

    protected override void OnHurt()
    {
    }

    

}
