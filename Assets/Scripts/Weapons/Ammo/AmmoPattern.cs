using UnityEngine;

public class AmmoPattern : MonoBehaviour, IFireable
{
    [SerializeField] private Ammo[] ammoArray;

    private float ammoRange;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;

    public GameObject GetGameObject(){
        return gameObject;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, 
        float ammoSpeed, Vector3 weaponAimDirectionVector, float fireTime, bool overrideAmmoMovement){
        this.ammoDetails = ammoDetails;
        this.ammoSpeed = ammoSpeed;
        
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector, fireTime);

        ammoRange = ammoDetails.ammoRange;

        gameObject.SetActive(true);

        foreach (Ammo ammo in ammoArray){
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, fireTime, true);
        }

        if (ammoDetails.ammoChargeTime > 0f){
            ammoChargeTimer = ammoDetails.ammoChargeTime;
        } else {
            ammoChargeTimer = 0f;
        }
    }

    private void Update(){
        if (ammoChargeTimer > 0f){
            ammoChargeTimer -= Time.deltaTime;
            return;
        }

        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));

        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f){
            DisableAmmo();
        }
    }

    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, float fireTime){
        
        float randomSpread = ammoDetails.ammoSpreadMin;
        if(fireTime >= 0.1f && fireTime < 0.3f){
            randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax/2);
        }
        if (fireTime >= 0.3f){
            randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);
        }

        int spreadToggle = Random.Range(0, 2)*2 -1;
        if(weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance){
            fireDirectionAngle = aimAngle;
        } else {
            fireDirectionAngle = weaponAimAngle;
        }

        fireDirectionAngle += spreadToggle * randomSpread;

        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    private void DisableAmmo(){
        gameObject.SetActive(false);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
    }

#endif
    #endregion
}
