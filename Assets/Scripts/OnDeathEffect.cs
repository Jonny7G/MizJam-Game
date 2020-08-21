using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathEffect : MonoBehaviour
{
    public List<GameObject> disableOnStart;
    public ParticleSystem pSystem;
    public int amountToEmit;
    public ObjectScaleBehavior scaler;
    public float xDeathScale, yDeathScale;
    public float targetRot;
    public Damageable damageHandler;
    public bool shouldDestroy = true;
    public System.Action DeathEnded;

    private bool emitted;
    private bool isDead;

    private void Start()
    {
        damageHandler.OnKill += StartEffects;
    }
    private void StartEffects()
    {
        isDead = true;
        if (scaler != null)
        {
            scaler.SetTargetScale(xDeathScale, yDeathScale);
            scaler.SetTargetRot(targetRot);
        }
        
    }
    private void Update()
    {
        if (isDead)
        {
            if ((scaler == null || scaler.retracting && scaler.targetRotation == Quaternion.Euler(0, 0, 0)) && !emitted)
            {
                if (scaler != null)
                {
                    scaler.gameObject.SetActive(false);
                }
                pSystem.Emit(amountToEmit);
                transform.parent = null;
                emitted = true;
                foreach (GameObject obj in disableOnStart)
                {
                    obj.SetActive(false);
                }
            }
            else if (emitted && pSystem.particleCount == 0)
            {
                Debug.Log("removing");
                DeathEnded?.Invoke();
                if (shouldDestroy)
                {
                    Destroy(damageHandler.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
