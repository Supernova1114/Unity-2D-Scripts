using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    private Entity ownerEntity;


    /// <summary>
    /// Logic for the collection of the item.
    /// </summary>
    /// <param name="owner">The new owner of the item.</param>
    public void Collect(Entity owner)
    {
        ownerEntity = owner;
        gameObject.GetComponent<Collider2D>().enabled = false;

        OnCollect();
    }


    /// <summary>
    /// Logic for dropping the item.
    /// </summary>
    public void Drop()
    {
        ownerEntity = null;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }


    /// <summary>
    /// Logic for the Consume / Attack input for the item. 
    /// </summary>
    public void Consume()
    {
        OnConsume();
    }


    /// <summary>
    /// Get the item's current owner.
    /// </summary>
    /// <returns>The owner Entity</returns>
    public Entity GetOwner()
    {
        return ownerEntity;
    }

    protected abstract void OnCollect();
    protected abstract void OnConsume();
}
