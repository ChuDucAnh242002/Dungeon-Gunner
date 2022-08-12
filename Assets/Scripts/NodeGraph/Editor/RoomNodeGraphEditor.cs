using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    [MenuItem("Room Node Graph Editor", menuItem ="Window/Dungeo Editor/Room Node Graph Editor")]

    private static void OpenWindow(){
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable() {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
    }

    private void OnGUI() {
        
        Vector2 pos1 = new Vector2(100f, 100f);
        Vector2 pos2 = new Vector2(300f, 300f);
        string labelField1 = "Node 1";
        string labelField2 = "Node 2";
        createArea(pos1, labelField1);
        createArea(pos2, labelField2);
    }

    private void createArea(Vector2 pos, string labelField){
        Vector2 size = new Vector2(nodeWidth, nodeHeight);
        Rect rect = new Rect(pos, size);
        GUILayout.BeginArea(rect, roomNodeStyle);
        EditorGUILayout.LabelField(labelField);
        GUILayout.EndArea();
    }

}


