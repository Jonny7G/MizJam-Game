using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public Transform targetPosition;
    public BoxCollider2D targetingColl;
    public bool shouldDisableColl = true;
    public LayerMask player;
    public bool alwaysPull;
    public bool IsGrappled { get; private set; }

    public void SetGrappledState(bool state)
    {
        IsGrappled = state;
        if (IsGrappled && shouldDisableColl)
        {
            targetingColl.enabled = false;
        }
    }
    private void Update()
    {
        Damageable dmgble = GetComponent<Damageable>();
        if(dmgble == null || dmgble.Health > 0)
        {
            if (!IsGrappled && !targetingColl.enabled)
            {
                targetingColl.enabled = !Physics2D.BoxCast(transform.position, targetingColl.size, 0f, Vector2.down, 0.01f, player);
            }
        }
        else
        {
            targetingColl.enabled = false;
        }
    }

}
