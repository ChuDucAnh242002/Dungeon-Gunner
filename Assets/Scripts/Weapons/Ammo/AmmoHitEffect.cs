using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake(){
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect){
        SetShootEffectColorGradient(ammoHitEffect.colorGradient);

        SetShootEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed,
            ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

        SetShootEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

        SetShootEffectParticleSprite(ammoHitEffect.sprite);

        SetShootEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
    }

    private void SetShootEffectColorGradient(Gradient gradient){
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;

    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticleNumber){
        ParticleSystem.MainModule main = ammoHitEffectParticleSystem.main;
        main.duration = duration;
        main.startSize = startParticleSize;
        main.startSpeed = startParticleSpeed;
        main.startLifetime = startLifetime;
        main.gravityModifier = effectGravity;
        main.maxParticles = maxParticleNumber; 
    }

    private void SetShootEffectParticleEmission(int emissionRate, int burstParticleNumber){
        ParticleSystem.EmissionModule emission = ammoHitEffectParticleSystem.emission;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emission.SetBurst(0, burst);
        emission.rateOverTime = emissionRate;
    }

    private void SetShootEffectParticleSprite(Sprite sprite){
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = ammoHitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimation.AddSprite(sprite);
    }

    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity){
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = ammoHitEffectParticleSystem.velocityOverLifetime;
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve(minVelocity.x, maxVelocity.x);
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve(minVelocity.y, maxVelocity.y);
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve(minVelocity.z, maxVelocity.z);
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        velocityOverLifetime.x = minMaxCurveX;
        velocityOverLifetime.y = minMaxCurveY;
        velocityOverLifetime.z = minMaxCurveZ;
    }
}
