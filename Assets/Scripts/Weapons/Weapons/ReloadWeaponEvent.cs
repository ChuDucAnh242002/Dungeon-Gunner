using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    // If total ammo is increased then specify the topUpAmmoPercent
    public void CallReloadWeaponEvent(Weapon weapon, int topUpAmmoPercent){
        OnReloadWeapon?.Invoke(this, new ReloadWeaponEventArgs(){
            weapon = weapon,
            topUpAmmoPercent = topUpAmmoPercent
        });
    }
}

public class ReloadWeaponEventArgs : EventArgs 
{
    public Weapon weapon;
    public int topUpAmmoPercent; // percent of ammo capacity
}