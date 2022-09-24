using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : CollectibleItem
{
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private float spinSpeed;

    private float spinSpeedOffset;

    protected override void OnCollect()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        spinSpeedOffset = spinSpeed * spinSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        spriteObj.transform.Rotate(new Vector3(0, spinSpeedOffset * Time.deltaTime, 0));
    }
}
