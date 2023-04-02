using MichaelWolfGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Entity
{
    [SerializeField] private List<Transform> positionList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginBehavior());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Begin boss fight behavior
    IEnumerator BeginBehavior()
    {
        yield return null;
        

    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnAwake()
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnHeal()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnHurt()
    {
        throw new System.NotImplementedException();
    }


}
