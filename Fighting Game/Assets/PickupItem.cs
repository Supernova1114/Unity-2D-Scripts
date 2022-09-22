using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    private Entity ownerEntity;

    public void Collect(Entity owner)
    {
        ownerEntity = owner;
        gameObject.GetComponent<Collider2D>().enabled = false;

        OnCollect();
    }

    public void Drop()
    {
        ownerEntity = null;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void Consume()
    {
        OnConsume();
    }

    public Entity GetOwner()
    {
        return ownerEntity;
    }

    protected abstract void OnCollect();
    protected abstract void OnConsume();
}
