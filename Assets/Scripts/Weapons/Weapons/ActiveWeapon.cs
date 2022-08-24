using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    [SerializeField] private Transform weaponShootPositionTransform;
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake(){
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable() {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable() {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs){
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon){
        currentWeapon = weapon;
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        if(weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null){
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();
        }

        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo(){
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon(){
        return currentWeapon;
    }

    public Vector3 GetShootPosition(){
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition(){
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon(){
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }

#endif
    #endregion
}
