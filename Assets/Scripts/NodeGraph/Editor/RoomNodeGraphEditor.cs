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
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        if (GUI.changed){
            Repaint();
        }
    }

    private void ProcessEvents(Event currentEvent){

        if(currentRoomNode == null || currentRoomNode.isLeftClickDragging == false){
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null){
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

    }

    private void DrawRoomNodes(){
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }
//-------------------------------Graph event-------------------------------------------------//
}


