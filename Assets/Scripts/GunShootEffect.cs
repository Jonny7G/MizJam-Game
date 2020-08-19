using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShootEffect : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private ParticleSystem effect;
    [SerializeField] private int amount;

    private void Start()
    {
        controller.OnGrappleShoot += ShootEffect;
    }
    private void ShootEffect()
    {
        effect.Emit(amount);
    }
}
