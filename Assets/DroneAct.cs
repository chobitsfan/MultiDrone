using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using UnityEngine.EventSystems;

public class DroneAct : MonoBehaviour, IPointerClickHandler
{
    public int DroneID;
    public GameObject Propeller;
    public GameObject Waypoint;
    GUIStyle selectedStyle;
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
    GameObject wp = null;
    // Start is called before the first frame update
    void Start()
    {
        selectedStyle = new GUIStyle { normal = new GUIStyleState { background = MakeTex(Color.green) } };
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

    Texture2D MakeTex(Color32 col)
    {
        Color32[] pix = { col, col, col, col };
        Texture2D tex = new Texture2D(2, 2);
        tex.SetPixels32(pix, 0);
        tex.Apply();
        return tex;
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
            Propeller.transform.Rotate(0, Time.deltaTime * 800, 0, Space.Self);
        }
    }

    public void Arm()
    {
        if (selected)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
            {
                command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM,
                param1 = 1
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
            sock.SendTo(data, myproxy);
        }
    }

    public void Disarm(bool forced)
    {
        if (selected)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
            {
                command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM,
                param1 = 0,
                param2 = forced ? 21196 : 0
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
            sock.SendTo(data, myproxy);
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

    public void Poshold()
    {
        if (selected)
        {
            MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
            {
                base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                target_system = 0,
                custom_mode = (uint)MAVLink.COPTER_MODE.POSHOLD
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
            sock.SendTo(data, myproxy);
        }
    }

    public void Stabilize()
    {
        if (selected)
        {
            MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
            {
                base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                target_system = (byte)DroneID,
                custom_mode = (uint)MAVLink.COPTER_MODE.STABILIZE
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
            sock.SendTo(data, myproxy);
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

    public void ManualControl(short pitch, short roll, short throttle, short yaw)
    {
        if (selected)
        {
            //Debug.Log("ManualControl " + throttle);
            MAVLink.mavlink_manual_control_t cmd = new MAVLink.mavlink_manual_control_t
            {
                target = (byte)DroneID,
                x = pitch,
                y = roll,
                z = throttle,
                r = yaw
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL, cmd);
            sock.SendTo(data, myproxy);
        }
    }

    public void Goto(float x, float y, float z)
    {
        if (selected)
        {
            if (apm_mode != (uint)MAVLink.COPTER_MODE.GUIDED)
            {
                MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
                {
                    base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                    target_system = 0,
                    custom_mode = (uint)MAVLink.COPTER_MODE.GUIDED
                };
                byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
                sock.SendTo(data, myproxy);
            }
            {
                MAVLink.mavlink_set_position_target_local_ned_t cmd = new MAVLink.mavlink_set_position_target_local_ned_t
                {
                    target_system = 0,
                    target_component = 0,
                    coordinate_frame = (byte)MAVLink.MAV_FRAME.LOCAL_NED,
                    type_mask = 0x0DF8,
                    x = x,
                    y = y,
                    z = z
                };
                byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_POSITION_TARGET_LOCAL_NED, cmd);
                sock.SendTo(data, myproxy);
            }
            if (wp == null)
            {
                wp = GameObject.Instantiate(Waypoint, new Vector3(-x, -z, y), Quaternion.identity);
            }
            else
            {
                wp.transform.position = new Vector3(-x, -z, y);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick " + DroneID);
        GameObject[] drones = GameObject.FindGameObjectsWithTag("Drones");
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().DeSelect();
        }
        selected = true;
    }

    /*private void OnMouseDown()
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
    }*/

    private void OnGUI()
    {
        Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);
        GUIContent content = new GUIContent("MAV" + DroneID + "\n" + (MAVLink.COPTER_MODE)apm_mode);
        if (selected)
        {
            Vector2 sz = selectedStyle.CalcSize(content);
            GUI.Label(new Rect(pos.x, Screen.height - pos.y + 40, sz.x, sz.y), content, selectedStyle);
        }
        else
        {
            Vector2 sz = GUIStyle.none.CalcSize(content);
            GUI.Label(new Rect(pos.x, Screen.height - pos.y + 40, sz.x, sz.y), content, GUIStyle.none);
        }
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
        //GL.MultMatrix(transform.localToWorldMatrix);
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);
        GL.Color(Color.blue);
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        GL.Vertex3(pos.x, pos.y, 0);
        pos = Camera.main.WorldToScreenPoint(transform.position - transform.right * 0.5f);
        GL.Vertex3(pos.x, pos.y, 0);
        //GL.Vertex3(0, 0, 0);
        //GL.Vertex3(-0.5f, 0, 0);

        GL.End();
        GL.PopMatrix();
    }
}
