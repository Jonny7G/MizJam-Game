using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncable : MonoBehaviour
{
    public System.Action OnBounce;
    [SerializeField] private float bounceVel;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            if (player.rb.velocity.y < bounceVel)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, bounceVel);
            }
        }
    }
}
