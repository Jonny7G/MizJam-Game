using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    public System.Action<PlayerController> OnPlayerHitBoundary;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            OnPlayerHitBoundary?.Invoke(player);
        }
    }
}
