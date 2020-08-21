using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Bouncable : MonoBehaviour
{
    public System.Action OnBounce;
    public UnityEvent OnBounceUE;
    public float bounceVel;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null && player.transform.position.y > transform.position.y)
        {
            if (player.rb.velocity.y < bounceVel)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, bounceVel);
            }
            OnBounce?.Invoke();
            OnBounceUE?.Invoke();
        }
    }
}
