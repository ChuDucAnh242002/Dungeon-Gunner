using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopReloadWeaponEvent : MonoBehaviour
{
    public event Action<StopReloadWeaponEvent, StopReloadWeaponEventArgs> OnStopReloadWeapon;
    public void CallStopReloadWeapon(Weapon weapon){
        OnStopReloadWeapon?.Invoke(this, new StopReloadWeaponEventArgs(){
            weapon = weapon
        });
    }
}

public class StopReloadWeaponEventArgs : EventArgs{
    public Weapon weapon;
}


