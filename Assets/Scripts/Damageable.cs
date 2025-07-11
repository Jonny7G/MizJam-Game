﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int Health;
    public System.Action OnKill { get; set; }
    public System.Action OnDamage;
    public void Damage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Kill();
            }
            else
            {
                OnDamage?.Invoke();
            }
        }
    }
    public void Kill()
    {
        Debug.Log("killed");
        OnKill?.Invoke();
        //Destroy(gameObject);
    }
}
