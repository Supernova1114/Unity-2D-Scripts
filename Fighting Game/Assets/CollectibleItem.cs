using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectibleItem : MonoBehaviour
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
    /// Get the item's current owner.
    /// </summary>
    /// <returns>The owner Entity</returns>
    public Entity GetOwner()
    {
        return ownerEntity;
    }

    protected abstract void OnCollect();
}
