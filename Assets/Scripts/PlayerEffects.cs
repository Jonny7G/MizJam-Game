using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Effect trailEffect;
    [SerializeField] private Effect landEffect;
    [SerializeField] private Effect jumpEffect;
    [SerializeField] private Effect quickEffect;

    [SerializeField] private float landEffectOffset;
    [System.Serializable]
    public class Effect
    {
        public ParticleSystem PSystem;
        public int Amount;

        public void Emit()
        {
            PSystem.Emit(Amount);
        }
    }
    private bool wasGrounded = true;
    private void Start()
    {
        player.OnJump += StartLandEffect;
        landEffect.PSystem.transform.parent = null;
    }
    private void Update()
    {
        CheckTrailEffect();
        if (wasGrounded != player.Grounded)
        {
            if (!wasGrounded)
            {
                landEffect.PSystem.Clear();
                jumpEffect.PSystem.transform.position = transform.position + Vector3.down * landEffectOffset;
                jumpEffect.Emit();
            }
            wasGrounded = player.Grounded;
        }
    }
    private void StartLandEffect(Vector2 pos)
    {
        landEffect.PSystem.transform.position = transform.position + Vector3.down * landEffectOffset;
        landEffect.Emit();
    }
    private void CheckTrailEffect()
    {
        ParticleSystem.EmissionModule trailModul = trailEffect.PSystem.emission;
        if (player.MoveAxis.x != 0 && player.Grounded)
        {
            trailModul.enabled = true;
        }
        else
        {
            trailModul.enabled = false;
        }
    }
}
