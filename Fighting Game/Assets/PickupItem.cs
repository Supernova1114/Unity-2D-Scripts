using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class PickupItem : NetworkBehaviour
{
    private Collider2D itemCollider;
    private Entity ownerEntity;

    
    public override void OnNetworkSpawn()
    {
        itemCollider = GetComponent<Collider2D>();

        OnAwake();
        base.OnNetworkSpawn();
    }
    

    /// <summary>
    /// Logic for the collection of the item.
    /// </summary>
    /// <param name="owner">The new owner of the item.</param>
    public void Collect(Entity owner)
    {
        if (ownerEntity == null)
        {
            ownerEntity = owner;
            SetPhysics(false);

            OnCollect();
        }
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
        itemCollider.attachedRigidbody.simulated = b;
        itemCollider.enabled = b;
    }

    protected abstract void OnCollect();
    protected abstract void OnConsume();
    protected abstract void OnAwake();
}
