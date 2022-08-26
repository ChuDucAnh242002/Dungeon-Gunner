using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapons Details")]
public class WeaponDetailsSO : ScriptableObject {
    #region Header WEAPON BASE DETAILS
    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    #endregion 

    public string weaponName;
    public Sprite weaponSprite;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    #endregion 
    public Vector3 weaponShootPosition;
    public AmmoDetailsSO weaponCurrentAmmo;
    public SoundEffectSO weaponFiringSoundEffect;
    public SoundEffectSO weaponReloadingSoundEffect;

    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]
    #endregion 
    public bool hasInfiniteAmmo = false;
    public bool hasInfiniteClipCapacity = false; // For enemy
    public int weaponClipAmmoCapacity = 6;  // Depend on weapon
    public int weaponAmmoCapacity = 100; // total ammo
    public float weaponFireRate = 0.2f;
    public float weaponPrechargeTime = 0f; // hold button down before fire
    public float weaponReloadTime = 0f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if(!hasInfiniteAmmo){
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }
        if (!hasInfiniteClipCapacity){
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }

#endif
    #endregion


}
