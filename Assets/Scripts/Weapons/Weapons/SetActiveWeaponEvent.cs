using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SetActiveWeaponEvent : MonoBehaviour {
    public event Action<SetActiveWeaponEvent, SetActiveWeaponEventArgs> OnSetActiveWeapon;

    public void CallSetActiveWeaponEvent(Weapon weapon){
        OnSetActiveWeapon?.Invoke(this, new SetActiveWeaponEventArgs() {weapon = weapon});
    }
}

public class SetActiveWeaponEventArgs : EventArgs{
    public Weapon weapon;
}
