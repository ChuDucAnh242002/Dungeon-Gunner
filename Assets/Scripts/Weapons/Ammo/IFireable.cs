using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    void InitialiseAmmo(AmmoDetailsSO ammoDetails, 
        float aimAngle, float weaponAimAngle, float ammoSpeed, 
        Vector3 weaponAimDirectionVector, float fireTime = 0f,
        bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}

