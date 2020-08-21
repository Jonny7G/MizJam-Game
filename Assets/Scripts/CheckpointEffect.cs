using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointEffect : MonoBehaviour
{
    public ParticleSystem effect;
    public Light effectLight;
    public CheckpointHandler checkPoint;

    private void Start()
    {
        checkPoint.OnActivated += StartEmission;
    }
    private void StartEmission()//null checks cuz if this breaks everything breaks
    {
        if (effectLight != null)
        {
            effectLight.enabled = true;
        }
        if (effect != null)
        {
            ParticleSystem.EmissionModule mod = effect.emission;
            mod.enabled = true;
        }
    }
}
