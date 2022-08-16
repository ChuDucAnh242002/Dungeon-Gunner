using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/Player/Current Player", order = 0)]
public class CurrentPlayerSO : ScriptableObject {
    public PlayerDetailsSO playerDetails;
    public string playerName;
}
