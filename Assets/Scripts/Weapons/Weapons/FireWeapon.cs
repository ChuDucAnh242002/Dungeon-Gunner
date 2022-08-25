using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;

    private void Awake(){
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }

    private void OnEnable(){
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }
    private void OnDisable(){
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update(){
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs){
        WeaponFire(fireWeaponEventArgs);
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs){
        WeaponPreCharge(fireWeaponEventArgs);

        if(fireWeaponEventArgs.fire){
            if(IsWeaponReadyToFire()){
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs){
        if (fireWeaponEventArgs.firePreviousFrame){
            firePreChargeTimer -= Time.deltaTime;
        } else {
            ResetPrechargeTimer();
        }
    }

    private bool IsWeaponReadyToFire(){

        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        bool hasInfiniteAmmo = currentWeapon.weaponDetails.hasInfiniteAmmo;
        bool hasInfiniteClipCapacity = currentWeapon.weaponDetails.hasInfiniteClipCapacity;

        if (currentWeapon.weaponRemainingAmmo <= 0 && !hasInfiniteAmmo) return false;
        if (currentWeapon.isWeaponReloading) return false;
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f) return false;
        if (!hasInfiniteClipCapacity && currentWeapon.weaponClipRemainingAmmo <= 0) return false;
        
        return true;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector){
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();
        
        if(currentAmmo != null){
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector){

        int ammoCounter = 0;

        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        float ammoSpawnInterval;

        if (ammoPerShot > 1){
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        } else {
            ammoSpawnInterval = 0f;
        }

        while (ammoCounter < ammoPerShot){

            ammoCounter++;

            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            IFireable ammo = (IFireable) PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            yield return new WaitForSeconds(ammoSpawnInterval);
        }


        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        bool hasInfiniteClipCapacity = currentWeapon.weaponDetails.hasInfiniteClipCapacity;
        if(!hasInfiniteClipCapacity){
            currentWeapon.weaponClipRemainingAmmo--;
            currentWeapon.weaponRemainingAmmo--;
        }

        weaponFiredEvent.CallWeaponFiredEvent(currentWeapon);
    }

    private void ResetCoolDownTimer(){
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void ResetPrechargeTimer(){
        float weaponPrechargeTime = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
        firePreChargeTimer = weaponPrechargeTime;
    }

}


