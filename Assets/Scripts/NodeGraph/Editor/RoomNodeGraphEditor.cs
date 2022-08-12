using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    // [MenuItem("Room Node Graph Editor", menuItem ="Window/Dungeo Editor/Room Node Graph Editor")]

    private static void OpenWindow(){
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }


    private void OnEnable() {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }


    // Double click on node graph to open the editor
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceId, int line){
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceId) as RoomNodeGraphSO;

        if (roomNodeGraph != null){
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    

    private void OnGUI() {
        if(currentRoomNodeGraph != null){

            DrawDraggedLine();

            ProcessEvents(Event.current);

            DrawRoomConnections();

            DrawRoomNodes();
        }

        if (GUI.changed){
            Repaint();
        }
    }

    private void DrawDraggedLine(){
        if (currentRoomNodeGraph.linePosition != Vector2.zero){
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent){

        if(currentRoomNode == null || currentRoomNode.isLeftClickDragging == false){
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null){
            ProcessRoomNodeGraphEvents(currentEvent);
        } else {
            currentRoomNode.ProcessEvents(currentEvent);
        }
        
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent){
        List<RoomNodeSO> roomNodeList = currentRoomNodeGraph.roomNodeList;

        for(int i = roomNodeList.Count - 1; i >= 0; i--){
            if(roomNodeList[i].rect.Contains(currentEvent.mousePosition)){
                return roomNodeList[i];
            }
        }

        return null;
    }


//-------------------------------Graph event ------------------------------------------------//
    private void ProcessRoomNodeGraphEvents(Event currentEvent){
        switch(currentEvent.type){

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
        // Right button down
        if(currentEvent.button == 1){
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition){
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject){
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType){
        Vector2 mousePosition = (Vector2) mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        Rect rect = new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight));
        roomNode.Initialise(rect, currentRoomNodeGraph, roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        currentRoomNodeGraph.OnValidate();
    }

    private void ProcessMouseUpEvent(Event currentEvent){
        if(currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null){
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            if(roomNode != null){
                if(currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id)){
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent){
        if(currentEvent.button == 1){
            ProccessRightMouseDragEvent(currentEvent);
        }
    }

    private void ProccessRightMouseDragEvent(Event currentEvent){
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null){
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    public void DragConnectingLine(Vector2 delta){
        currentRoomNodeGraph.linePosition += delta;
    }

    public void ClearLineDrag(){
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void DrawRoomConnections(){
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            if(roomNode.childRoomNodeIDList.Count > 0){
                foreach(string childRoomNodeID in roomNode.childRoomNodeIDList){
                    if(currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID)){

                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode){
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        Vector2 midPosition = (endPosition + startPosition) / 2f;
        Vector2 direction = endPosition - startPosition;

        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        Handles.DrawBezier(arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, Color.white, null, connectingLineWidth);

        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;

    }

    private void DrawRoomNodes(){
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }
//-------------------------------Graph event-------------------------------------------------//
}


