using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    #region Header References
    [Space(10)]
    [Header("Reference")]
    #endregion 
    public SpriteRenderer spriteRenderer;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
    }
#endif
    #endregion

}
