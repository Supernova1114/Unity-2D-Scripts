using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerEntity : Entity
{

    private static PlayerEntity instance;

    [Header("PlayerEntity")]
    [SerializeField] private GameObject playerSpriteObj;
    

    public static PlayerEntity GetInstance()
    {
        return instance;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CollectibleItem"))
        {
            collision.GetComponent<CollectibleItem>().Collect(this);
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
