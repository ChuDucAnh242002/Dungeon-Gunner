using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    // [MenuItem("Room Node Graph Editor", menuItem ="Window/Dungeo Editor/Room Node Graph Editor")]

    private static void OpenWindow(){
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }


    private void OnEnable() {

        Selection.selectionChanged += InspectorSelectionChanged;

        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);


        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable(){
        Selection.selectionChanged -= InspectorSelectionChanged;
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

            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            DrawDraggedLine();

            ProcessEvents(Event.current);

            DrawRoomConnections();

            DrawRoomNodes();
        }

        if (GUI.changed){
            Repaint();
        }
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor){
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        // Draw vertical
        for (int i = 0; i< verticalLineCount; i++){
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset,
                             new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        // Draw horizontal
        for (int j = 0; j < horizontalLineCount; j++){
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset,
                             new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    private void DrawDraggedLine(){
        if (currentRoomNodeGraph.linePosition != Vector2.zero){
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent){

        graphDrag = Vector2.zero;

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

            case EventType.KeyDown:
                ProcessKeyDownEvent(currentEvent);
                // Debug.Log("Key Down");
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
        // Left button down
        else if (currentEvent.button == 0){
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ClearAllSelectedRoomNodes(){
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            if(roomNode.isSelected){
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    
    private void ShowContextMenu(Vector2 mousePosition){
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Selected All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddItem(new GUIContent("Deselected All Room Nodes"), false, DeSelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected RoomNode Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected RoomNode"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject){
        if (currentRoomNodeGraph.roomNodeList.Count == 0){
            CreateRoomNode(new Vector2(20f, 300f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

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

    private void SelectAllRoomNodes(){
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    private void DeSelectAllRoomNodes(){
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            roomNode.isSelected = false;
        }
        GUI.changed = true;
    }

    private void DeleteSelectedRoomNodeLinks(){
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            if(roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0){
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--){
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    if (childRoomNode != null && childRoomNode.isSelected){
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void DeleteSelectedRoomNodes(){
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            if(roomNode.isSelected && !roomNode.roomNodeType.isEntrance){
                roomNodeDeletionQueue.Enqueue(roomNode);

                // Delete parent
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList){
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null){
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }


                // Delete child
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList){
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null){
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Delete queue
        while (roomNodeDeletionQueue.Count > 0){
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            DestroyImmediate(roomNodeToDelete, true);

            AssetDatabase.SaveAssets();
        }


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
        else if(currentEvent.button == 0){
            ProccessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProccessRightMouseDragEvent(Event currentEvent){
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null){
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProccessLeftMouseDragEvent(Vector2 dragDelta){
        graphDrag = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++){
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    public void DragConnectingLine(Vector2 delta){
        currentRoomNodeGraph.linePosition += delta;
    }


    private void ProcessKeyDownEvent (Event currentEvent){
        if (currentEvent.control){

            if(currentEvent.keyCode == KeyCode.C){
                CreateRoomNode(currentEvent.mousePosition);
            }

            if (currentEvent.keyCode == KeyCode.A){
                SelectAllRoomNodes();
            }

            if (currentEvent.keyCode == KeyCode.X){
                DeleteSelectedRoomNodeLinks();
            }

            if (currentEvent.keyCode == KeyCode.D){
                DeleteSelectedRoomNodes();
            }

            if (currentEvent.keyCode == KeyCode.Z){
                DeSelectAllRoomNodes();
            }

        }
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
            if (roomNode.isSelected){
                roomNode.Draw(roomNodeSelectedStyle);
            } else {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }
//-------------------------------Graph event-------------------------------------------------//

    private void InspectorSelectionChanged(){
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
        if(roomNodeGraph != null){
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}


