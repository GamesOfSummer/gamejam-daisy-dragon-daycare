using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetMouseCursor : MonoBehaviour {

    void OnMouseEnter () {
        Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
    }
}