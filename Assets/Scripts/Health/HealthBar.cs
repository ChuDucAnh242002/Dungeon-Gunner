using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Header GameObject Reference
    [Space(10)]
    [Header("GameObject References")]
    #endregion
    [SerializeField] private GameObject healthBar;

    public void EnableHealthBar(){
        gameObject.SetActive(true);
    }

    public void DisableHealthBar(){
        gameObject.SetActive(false);
    }

    public void SetHealthBarValue(float healthPerCent){
        healthBar.transform.localScale = new Vector3(healthPerCent, 1f, 1f);
    }
}
