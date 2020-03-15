using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public float expOnDeath;
    public The_Player Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<The_Player>();
    }

    public override void Die()
    {
        Player.Add_Experience(expOnDeath);
        base.Die();
    }
}
