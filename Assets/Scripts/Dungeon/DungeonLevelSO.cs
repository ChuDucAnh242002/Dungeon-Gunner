using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The name for th level")]
    #endregion

    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    #endregion
    #region Tooltip
    [Tooltip("Populate the list with the room templates that will be part of the level")]
    #endregion Tooltip
    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion

    #region Tooltip
    [Tooltip("Populate the list with the room node graphs which should be randomly selected from for the level.")]
    #endregion Tooltip
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList)){
            return;
        }
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList)){
            return;
        }

        // Check if there are entrance, EWcorridor and NScorridor
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList){
            if(roomTemplateSO == null){
                return ;
            }
            if(roomTemplateSO.roomNodeType.isCorridorEW){
                isEWCorridor = true;
            }
            if(roomTemplateSO.roomNodeType.isCorridorNS){
                isNSCorridor = true;
            }
            if(roomTemplateSO.roomNodeType.isEntrance){
                isEntrance = true;
            }

        }

        if (isEWCorridor == false){
            Debug.Log("In " + this.name.ToString() + " : No EW Corridor Room Type Specified");
        }
        if (isNSCorridor == false){
            Debug.Log("In " + this.name.ToString() + " : No NA Corridor Room Type Specified");
        }
        if (isEntrance == false){
            Debug.Log("In " + this.name.ToString() + " : No Entrance Room Type Specified");
        }

        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList){
            if(roomNodeGraph == null){
                return;
            }
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList){
                if (roomNodeSO == null){
                    continue;
                }

                RoomNodeTypeSO roomNodeType = roomNodeSO.roomNodeType;
                if(roomNodeType.isEntrance || 
                   roomNodeType.isCorridorEW ||
                   roomNodeType.isCorridorNS ||
                   roomNodeType.isCorridor ||
                   roomNodeType.isNone){
                    continue;
                }

                bool isRoomNodeTypeFound = false;

                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList){
                    if(roomTemplateSO == null){
                        continue;
                    }
                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType){
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound){
                    Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeType.name.ToString() + "found for node graph " 
                        + roomNodeGraph.name.ToString());
                }
            }
        }

    }

#endif
    #endregion


}