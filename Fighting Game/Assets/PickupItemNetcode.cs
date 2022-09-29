using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickupItemNetcode : NetworkBehaviour
{
    private PickupItem pickupItem;


    public override void OnNetworkSpawn()
    {
        pickupItem = GetComponent<PickupItem>();

        base.OnNetworkSpawn();
    }

    public void Collect()
    {
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
