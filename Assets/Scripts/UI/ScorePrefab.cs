using UnityEngine;
using TMPro;

public class ScorePrefab : MonoBehaviour
{
    public TextMeshProUGUI rankTMP;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI scoreTMP;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(rankTMP), rankTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(nameTMP), nameTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(levelTMP), levelTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scoreTMP), scoreTMP);
    }
#endif
    #endregion
}
