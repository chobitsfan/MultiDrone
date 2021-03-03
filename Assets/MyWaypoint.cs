using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWaypoint : MonoBehaviour
{
    Camera cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void OnGUI()
    {
        Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);
        GUIContent content = new GUIContent("Waypoint");
        Vector2 sz = GUIStyle.none.CalcSize(content);
        GUI.Label(new Rect(pos.x, Screen.height - pos.y + 30, sz.x, sz.y), content, GUIStyle.none);
    }
}
