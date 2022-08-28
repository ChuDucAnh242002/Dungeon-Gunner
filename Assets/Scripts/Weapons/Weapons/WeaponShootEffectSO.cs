using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject {
    #region Header WEAOPN SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion
    public Gradient colorGradient;
    public float duration = 0.05f;
    public float startParticleSize = 0.25f;
    public float startParticleSpeed = 3f;
    public float startLifetime = 0.5f;
    public int maxParticleNumber = 100;
    public int emissionRate = 100;
    public int burstParticleNumber = 20;
    public float effectGravity = -0.0f;
    public Sprite sprite;
    public Vector3 velocityOverLifetimeMin;
    public Vector3 velocityOverLifetimeMax;
    public GameObject weaponShootEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);

    }
#endif
    #endregion
}
