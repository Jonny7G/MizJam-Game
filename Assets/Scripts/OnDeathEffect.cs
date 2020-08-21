using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathEffect : MonoBehaviour
{
    public ParticleSystem pSystem;
    public ObjectScaleBehavior scaler;
    public float xDeathScale, yDeathScale;
    public float targetRot;
    public int amountToEmit;
    public Damageable damageHandler;

    private bool emitted;
    private bool isDead;
    private void Start()
    {
        damageHandler.OnKill += StartEffects;
    }
    private void StartEffects()
    {
        isDead = true;
        scaler.SetTargetScale(xDeathScale, yDeathScale);
        scaler.SetTargetRot(targetRot);
    }
    private void Update()
    {
        if (isDead)
        {
            if (scaler.retracting && scaler.targetRotation == Quaternion.Euler(0,0,0) && !emitted)
            {
                scaler.gameObject.SetActive(false);
                pSystem.Emit(amountToEmit);
                transform.parent = null;
                emitted = true;
            }
            else if (emitted && pSystem.particleCount == 0)
            {
                Debug.Log("removing");
                Destroy(damageHandler.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
