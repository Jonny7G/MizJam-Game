using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamager : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        collision.collider.GetComponent<Damageable>()?.Damage(1);
    }
}
