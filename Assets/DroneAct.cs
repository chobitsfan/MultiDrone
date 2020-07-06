using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneAct : MonoBehaviour
{
    public int MavlinkID;
    public GameWorld gameWorld;
    public UnityEngine.UI.Text StatusText;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        //Debug.Log("OnMouseDown" + MavlinkID);
        gameWorld.TargetID = MavlinkID;
        StatusText.text = "MAV" + MavlinkID + " selected";
        //Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);
        //Debug.Log(pos.x + "," + pos.y);
    }

    private void OnGUI()
    {
        Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);
        GUI.Label(new Rect(pos.x - 15, Screen.height - pos.y, 50, 50), "MAV" + MavlinkID);
    }
}
