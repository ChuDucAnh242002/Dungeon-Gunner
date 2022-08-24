using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponFiredEvent : MonoBehaviour
{
    public Action<WeaponFiredEvent, WeaponFiredEventArgs> OnWeaponFired;
    public void CallWeaponFiredEvent(Weapon weapon){
        OnWeaponFired?.Invoke(this, new WeaponFiredEventArgs() {weapon = weapon});
    }

}

public class WeaponFiredEventArgs : EventArgs
{
    public Weapon weapon;
}