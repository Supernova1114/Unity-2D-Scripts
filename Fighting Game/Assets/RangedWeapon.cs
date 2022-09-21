using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    [SerializeField] private int maxAmmo;
    [SerializeField] protected GameObject bullet;

    private int currentAmmo;

    private void Awake()
    {
        currentAmmo = maxAmmo;
        OnAwake();
    }

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

    protected override void OnAttack()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            OnOnAttack();
        }
    }

    protected abstract void OnAwake();
    protected abstract void OnOnAttack();
}
