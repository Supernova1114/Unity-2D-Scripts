using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using UnityEngine;

public abstract class Weapon : PickupItem
{
    [Header("Weapon")]
    [SerializeField] private float attackInterval;

    private bool cooldown = false;

    protected override void OnConsume()
    {
        Attack();
    }


    /// <summary>
    /// Handle weapon attack logic.
    /// </summary>
    public void Attack()
    {
        if (!cooldown)
        {
            cooldown = true;
            OnAttack();

            if (attackInterval > 0)
            {
                this.StartTimer(attackInterval, () => cooldown = false);
            }
        }
    }

    protected abstract void OnAttack();
}
