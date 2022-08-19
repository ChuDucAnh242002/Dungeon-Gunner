using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCursor : MonoBehaviour
{
    private void Awake(){
        Cursor.visible = false;
    }

    private void Update(){
        transform.position = Input.mousePosition;
    }
}
