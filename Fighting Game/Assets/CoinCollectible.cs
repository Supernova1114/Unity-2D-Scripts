using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : CollectibleItem
{
    [SerializeField] private GameObject spriteObj;
    [SerializeField] private float spinSpeed;
    [SerializeField] private int value;

    private float spinSpeedOffset;


    void Awake()
    {
        spinSpeedOffset = spinSpeed * spinSpeed;
    }

    protected override void OnCollect()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        spriteObj.transform.Rotate(new Vector3(0, spinSpeedOffset * Time.deltaTime, 0));
    }
}
