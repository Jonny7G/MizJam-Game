using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{
    [Header("parameters")]
    [SerializeField] private string playerWalkString;
    [SerializeField] private string playerGroundedString;

    [Space(20)]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController player;
    [SerializeField] private AimingBehavior aiming;
    private void Update()
    {
        animator.SetBool(playerGroundedString, player.Grounded);
        animator.SetBool(playerWalkString, player.MoveAxis.x != 0);
        sr.flipX = aiming.GetAimDir().x < 0;
    }
}
