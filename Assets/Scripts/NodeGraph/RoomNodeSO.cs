using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject {
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType){
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle){
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();

        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance){
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        } else {
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // If the room type selection has changed child connections potentially invalid
            if(roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor ||
               !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
               !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom
            ){

                if (childRoomNodeIDList.Count > 0){
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--){
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        if (childRoomNode != null){
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }

        
        if (EditorGUI.EndChangeCheck()){
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();

    }

    public string[] GetRoomNodeTypesToDisplay(){
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++){
            if(roomNodeTypeList.list[i].displayInNodeGraphEditor){
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void ProcessEvents(Event currentEvent){
        switch (currentEvent.type){
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent){
        if(currentEvent.button == 0){
            ProcessLeftClickDownEvent();
        }
        else if(currentEvent.button == 1){
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDownEvent(){
        // Select in editor also in inspector
        Selection.activeObject = this;

        if(isSelected){
            isSelected = false;
        } else {
            isSelected = true;
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent){
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    private void ProcessMouseUpEvent(Event currentEvent){
        if(currentEvent.button == 0){
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent(){
        if(isLeftClickDragging){
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent){
        if(currentEvent.button == 0){
            ProcessLeftClickDragEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDragEvent(Event currentEvent){
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta){
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeIDToRoomNode(string childID){

        if(IsChildRoomValid(childID)){
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    public bool IsChildRoomValid(string childID){
        bool isConnectedBossNodeAlready = false;

        // 1 boss room per level
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList){
            bool isBossRoom = roomNode.roomNodeType.isBossRoom;
            bool hasBossRoomParent = roomNode.parentRoomNodeIDList.Count > 0;
            if(isBossRoom && hasBossRoomParent){
                isConnectedBossNodeAlready = true;
            }
        }

        RoomNodeTypeSO childRoomNodeType = roomNodeGraph.GetRoomNode(childID).roomNodeType;

        // If child node has type of boss room and boss room already have
        if (childRoomNodeType.isBossRoom && isConnectedBossNodeAlready) return false;
        

        // Node is None
        if (childRoomNodeType.isNone) return false;

        // Child list contains id already
        if (childRoomNodeIDList.Contains(childID)) return false;
        

        // Node and child are the same
        if (id == childID) return false;
        

        // Parent list contains id
        if(parentRoomNodeIDList.Contains(childID)) return false;

        // Child already have parent, 1 parent only
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0) return false;


        // Child and this node are not corridor
        if (!childRoomNodeType.isCorridor && !roomNodeType.isCorridor) return false;

        // Max corridors
        if (childRoomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors) return false;
        

        // Child node is an entrance
        if (childRoomNodeType.isEntrance) return false;
        

        // If child is not corridor and there is a child node already
        if (!childRoomNodeType.isCorridor && childRoomNodeIDList.Count > 0) return false;
        
        return true;
    }

    public bool AddParentRoomNodeIDToRoomNode(string parentID){
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childID){
        if(childRoomNodeIDList.Contains(childID)){
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID){
        if(parentRoomNodeIDList.Contains(parentID)){
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion
}
