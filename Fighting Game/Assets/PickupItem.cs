using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    private Collider2D [] itemColliders;

    private Entity ownerEntity;


    private void Start()
    {
        itemColliders = GetComponentsInChildren<Collider2D> ();
    }

    /// <summary>
    /// Logic for the collection of the item.
    /// </summary>
    /// <param name="owner">The new owner of the item.</param>
    public void Collect(Entity owner)
    {
        ownerEntity = owner;
        SetPhysics(false);

        OnCollect();
    }


    /// <summary>
    /// Logic for dropping the item.
    /// </summary>
    public void Drop()
    {
        ownerEntity = null;
        SetPhysics(true);
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


    private void SetPhysics(bool b)
    {
        itemColliders[0].attachedRigidbody.simulated = b;
        foreach (Collider2D collider in itemColliders)
        {
            collider.enabled = b;
        }
    }

    protected abstract void OnCollect();
    protected abstract void OnConsume();
}
