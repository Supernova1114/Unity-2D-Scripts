using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    [SerializeField] private int maxAmmo;
    [SerializeField] private bool infiniteAmmo;
    [SerializeField] protected Bullet bullet;

    private int currentAmmo;


    protected override void OnAwake()
    {
        currentAmmo = maxAmmo;
    }
    

    /// <summary>
    /// Add ammo to the weapon. Does not add to over max ammo.
    /// </summary>
    /// <param name="addAmount">The ammount of ammo to add.</param>
    public void AddAmmo(int addAmount)
    {
        int newAmount = currentAmmo + addAmount;

        if (newAmount >= maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
        else
        {
            currentAmmo += addAmount;
        }
    }


    /// <summary>
    /// Handle attack for the weapon.
    /// Subtracts ammo on attack.
    /// </summary>
    protected override void OnAttack()
    {
        if (infiniteAmmo)
        {
            OnOnAttack();
        }
        else if (currentAmmo > 0)
        {
            currentAmmo--;
            OnOnAttack();
        }
    }

    protected abstract void OnOnAttack();
}
