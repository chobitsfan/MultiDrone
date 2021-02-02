using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class DroneAct : MonoBehaviour
{
    public int DroneID;
    public GameObject Propeller;
    //public GameWorld gameWorld;
    //public UnityEngine.UI.Text StatusText;
    Camera cam;
    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;
    bool selected = false;
    IPEndPoint myproxy;
    uint apm_mode;
    bool armed = false;
    static Material lineMaterial;
    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        buf = new byte[1024];
        mavlinkParse = new MAVLink.MavlinkParse();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Loopback, 17500 + DroneID));
        myproxy = new IPEndPoint(IPAddress.Loopback, 17500);
    }

    // Update is called once per frame
    void Update()
    {
        int recvBytes = 0;
        try
        {
            recvBytes = sock.Receive(buf);
        }
        catch (SocketException) { }
        if (recvBytes > 0)
        {
            MAVLink.MAVLinkMessage msg = mavlinkParse.ReadPacket(buf);
            if (msg != null)
            {
                if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                {
                    var status_txt = (MAVLink.mavlink_statustext_t)msg.data;
                    Debug.Log(System.Text.Encoding.ASCII.GetString(status_txt.text));
                }
                else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                {
                    var heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                    apm_mode = heartbeat.custom_mode;
                    if ((heartbeat.base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) == 0) armed = false; else armed = true;
                }
            }
        }
        if (armed)
        {
            Propeller.transform.Rotate(0, 3, 0, Space.Self);
        }
    }

    public void DeSelect()
    {
        selected = false;
    }

    public void Guided()
    {
        if (selected)
        {
            Debug.Log("guided " + DroneID);
        }
    }

    public void Auto()
    {
        if (selected)
        {
            MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
            {
                base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                target_system = 0,
                custom_mode = (uint)MAVLink.COPTER_MODE.AUTO
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
            sock.SendTo(data, myproxy);
        }
    }

    private void OnMouseDown()
    {
        //Debug.Log("OnMouseDown " + DroneID);
        GameObject[] drones = GameObject.FindGameObjectsWithTag("Drones");
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().DeSelect();
        }
        selected = true;
        //gameWorld.TargetID = MavlinkID;
        //StatusText.text = "MAV" + MavlinkID + " selected";
        //Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);
        //Debug.Log(pos.x + "," + pos.y);
    }

    private void OnGUI()
    {
        Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);
        GUI.Label(new Rect(pos.x, Screen.height - pos.y + 10, 70, 50), "MAV" + DroneID + "\n" + (MAVLink.COPTER_MODE)apm_mode);
    }
    
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Color(Color.blue);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(-0.5f, 0, 0);

        GL.End();
        GL.PopMatrix();
    }
}
