using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(StopReloadWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private StopReloadWeaponEvent stopReloadWeaponEvent;
    private Coroutine reloadWeaponCoroutine;

    private void Awake(){
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        stopReloadWeaponEvent = GetComponent<StopReloadWeaponEvent>();
    }
    

    private void OnEnable() {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        stopReloadWeaponEvent.OnStopReloadWeapon += StopReloadWeaponEvent_OnStopReloadWeapon;
    }
    private void OnDisable() {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        stopReloadWeaponEvent.OnStopReloadWeapon -= StopReloadWeaponEvent_OnStopReloadWeapon;
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs){
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs){
        if (reloadWeaponCoroutine != null){
            StopCoroutine(reloadWeaponCoroutine);
        }

        Weapon weapon = reloadWeaponEventArgs.weapon;
        int topUpAmmoPercent = reloadWeaponEventArgs.topUpAmmoPercent;
        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(weapon, topUpAmmoPercent));
    }

    private IEnumerator ReloadWeaponRoutine(Weapon weapon, int topUpAmmoPercent){
        int weaponAmmoCapacity = weapon.weaponDetails.weaponAmmoCapacity;
        int weaponRemainingAmmo = weapon.weaponRemainingAmmo;
        int weaponClipAmmoCapacity = weapon.weaponDetails.weaponClipAmmoCapacity;
        float weaponReloadTime = weapon.weaponDetails.weaponReloadTime;
        bool hasInfiniteAmmo = weapon.weaponDetails.hasInfiniteAmmo;
        SoundEffectSO weaponReloadingSoundEffect = weapon.weaponDetails.weaponReloadingSoundEffect;

        if(weaponReloadingSoundEffect != null){
            SoundEffectManager.Instance.PlaySoundEffect(weaponReloadingSoundEffect);
        }


        weapon.isWeaponReloading = true;

        while(weapon.weaponReloadTimer < weaponReloadTime){
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        // Increase ammo
        if (topUpAmmoPercent != 0){
            int ammoIncrease = Mathf.RoundToInt((weaponAmmoCapacity * topUpAmmoPercent) / 100f);
            int totalAmmo = weaponRemainingAmmo + ammoIncrease;

            if (totalAmmo > weaponAmmoCapacity){
                weapon.weaponRemainingAmmo = weaponAmmoCapacity;
            } else {
                weapon.weaponRemainingAmmo = totalAmmo;
            }
        }

        if (hasInfiniteAmmo){
            weapon.weaponClipRemainingAmmo = weaponClipAmmoCapacity;
        }
        else if (weaponRemainingAmmo >= weaponClipAmmoCapacity){
            weapon.weaponClipRemainingAmmo = weaponClipAmmoCapacity;
        }
        else {
            weapon.weaponClipRemainingAmmo = weaponRemainingAmmo;
        }

        weapon.weaponReloadTimer = 0f;
        weapon.isWeaponReloading = false;

        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs){
        Weapon weapon = setActiveWeaponEventArgs.weapon;
        if(weapon.isWeaponReloading){
            if(reloadWeaponCoroutine != null){
                StopCoroutine(reloadWeaponCoroutine);
            }

            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(weapon, 0));
        }
    }

    private void StopReloadWeaponEvent_OnStopReloadWeapon(StopReloadWeaponEvent stopReloadWeaponEvent, StopReloadWeaponEventArgs stopReloadWeaponEventArgs){
        Weapon weapon = stopReloadWeaponEventArgs.weapon;
        if (weapon.isWeaponReloading){
            if (reloadWeaponCoroutine != null){
                weapon.isWeaponReloading = false;
                weapon.weaponReloadTimer = 0f;
                StopCoroutine(reloadWeaponCoroutine);
            }
        }
    }
}
