using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake() {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle){
        SetShootEffectColorGradient(shootEffect.colorGradient);

        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticleSize, shootEffect.startParticleSpeed,
            shootEffect.startLifetime, shootEffect.effectGravity, shootEffect.maxParticleNumber);
        
        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);

        SetEmitterRotation(aimAngle);

        SetShootEffectParticleSprite(shootEffect.sprite);

        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifetimeMin, shootEffect.velocityOverLifetimeMax);
    }

    private void SetShootEffectColorGradient(Gradient gradient){
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;

    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticleNumber){
        ParticleSystem.MainModule main = shootEffectParticleSystem.main;
        main.duration = duration;
        main.startSize = startParticleSize;
        main.startSpeed = startParticleSpeed;
        main.startLifetime = startLifetime;
        main.gravityModifier = effectGravity;
        main.maxParticles = maxParticleNumber; 
    }

    private void SetShootEffectParticleEmission(int emissionRate, int burstParticleNumber){
        ParticleSystem.EmissionModule emission = shootEffectParticleSystem.emission;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emission.SetBurst(0, burst);
        emission.rateOverTime = emissionRate;
    }

    private void SetEmitterRotation(float aimAngle){
        ParticleSystem.RotationBySpeedModule rotationBySpeed = shootEffectParticleSystem.rotationBySpeed;
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    private void SetShootEffectParticleSprite(Sprite sprite){
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = shootEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimation.AddSprite(sprite);
    }

    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity){
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = shootEffectParticleSystem.velocityOverLifetime;
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
