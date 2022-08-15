using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Object/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject {
    [HideInInspector] public string guid;
    #region Header ROOM PREFAB
    [Space(10)]
    [Header("ROOM PREFAB")]
    #endregion

    #region Tooltip
    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environement game objects)")]
    #endregion
    public GameObject prefab;
    [HideInInspector] public GameObject previousPrefab;
    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("ROOM CONFIGURATION")]
    #endregion

    #region Tooltip
    [Tooltip("Exception for type 'Corridor' CorridorNS and CorridorEW.")]
    #endregion Tooltip
    public RoomNodeTypeSO roomNodeType;
    #region Tooltip
    [Tooltip("Rectangle around room tilemap. Using coordinate brush pointer to get the tilemap grid position for that bottom left corner")]
    #endregion
    public Vector2Int lowerBounds;
    #region Tooltip
    [Tooltip("Rectangle around room tilemap. Using coordinate brush pointer to get the tilemap grid position for that top right corner")]
    #endregion
    public Vector2Int upperBounds;
    #region Tooltip
    [Tooltip("There should be a maximum of doorways for a room. 3 tile opening size, with middle tile pos being the doorway coordinate")]
    #endregion
    [SerializeField] public List<Doorway> doorwayList;
    #region Tooltip
    [Tooltip("Possible spawn position for the room in tile map (enemies and chests)")]
    #endregion
    public Vector2Int[] spawnPositionArray;

    public List<Doorway> GetDoorwayList(){return doorwayList;}

    #region 
#if UNITY_EDITOR
    private void OnValidate(){
        if(guid == "" || previousPrefab != prefab){
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
#endif
    #endregion

}
