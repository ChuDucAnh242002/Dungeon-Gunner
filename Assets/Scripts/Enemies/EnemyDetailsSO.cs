using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject {
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion
    public string enemyName;
    public GameObject enemyPrefab;
    public float chaseDistance = 50f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
    }
#endif
    #endregion
}
