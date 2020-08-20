using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHandler : MonoBehaviour
{
    public Collider2D newBoundary;
    public System.Action OnActivated;
    public bool WasActivated { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!WasActivated)
        {
            OnActivated?.Invoke();
            WasActivated = true;
            if (newBoundary != null)
            {
                newBoundary.enabled = true;
            }
        }
    }
}
