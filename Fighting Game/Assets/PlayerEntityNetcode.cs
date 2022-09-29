using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerEntityNetcode : NetworkBehaviour
{
    private PlayerEntity playerEntity;

    
    public override void OnNetworkSpawn()
    {
        playerEntity = GetComponent<PlayerEntity>();
        base.OnNetworkSpawn();
    }

    [ServerRpc]
    public void TryPickupDropItemServerRpc()
    {
        playerEntity.TryPickupDropItem();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
