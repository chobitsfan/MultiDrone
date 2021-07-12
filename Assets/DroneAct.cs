using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.EventSystems;

public class DroneAct : MonoBehaviour, IPointerClickHandler
{
    public int DroneID;
    public GameObject MyDroneModel;
    public GameObject Waypoint;
    [System.NonSerialized]
    public int apm_mode = -1;
    [System.NonSerialized]
    public bool waiting_in_chk_point = false;

    GUIStyle selectedStyle;
    Camera cam;
    byte[] buf;
    MAVLink.MavlinkParse mavlinkParse;
    Socket sock;
    bool selected = false;
    IPEndPoint myproxy;
    bool armed = false;
    static Material lineMaterial;
    GameObject wp = null;
    Vector3 lastPos = Vector3.zero;
    Vector3 vel = Vector3.zero;
    bool tracking = false;
    bool pos_tgt_local_rcved = false;
    bool mis_cur_rcved = false;
    //bool mis_item_reached_rcved = false;
    byte system_status = 0;
    int cur_mis_seq = 0;
    List<int> mission_chk_points = new List<int>();
    int mission_count = -1;
    int wait_mission_seq = -1;
    int nxt_wp_seq = -1;
    float vel_update_int = 0f;
    List<MAVLink.mavlink_mission_item_int_t> upload_mission = new List<MAVLink.mavlink_mission_item_int_t>();
    float att_pos_update_int = 10f;
    string status_text = "";
    float status_text_timeout = 0f;
    const float STATUS_TEXT_CD = 4f;

    // Start is called before the first frame update
    void Start()
    {
        selectedStyle = new GUIStyle { normal = new GUIStyleState { background = MakeTex(Color.green) } };
        selected = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        buf = new byte[512];
        mavlinkParse = new MAVLink.MavlinkParse();
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Blocking = false
        };
        sock.Bind(new IPEndPoint(IPAddress.Loopback, 17500 + DroneID));
        myproxy = new IPEndPoint(IPAddress.Loopback, 17500);
        wp = GameObject.Instantiate(Waypoint, Vector3.zero, Quaternion.identity);
        wp.SetActive(false);
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
        att_pos_update_int += Time.deltaTime;
        if (status_text_timeout > 0f)
        {
            status_text_timeout -= Time.deltaTime;
            if (status_text_timeout <= 0f)
            {
                status_text = "";
            }
        }
        while (sock.Available > 0)
        {
            int recvBytes = 0;
            try
            {
                recvBytes = sock.Receive(buf); //Receive will read the FIRST queued datagram
            }
            catch (SocketException e)
            {
                Debug.LogWarning("socket err " + e.ErrorCode);
            }
            if (recvBytes > 0)
            {
                byte[] msg_buf = new byte[recvBytes];
                System.Array.Copy(buf, msg_buf, recvBytes);
                MAVLink.MAVLinkMessage msg = mavlinkParse.ReadPacket(msg_buf);
                if (msg != null)
                {
                    if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                    {
                        var status_txt = (MAVLink.mavlink_statustext_t)msg.data;
                        //Debug.Log(System.Text.Encoding.ASCII.GetString(status_txt.text));
                        status_text = "\n" + System.Text.Encoding.ASCII.GetString(status_txt.text);
                        status_text_timeout = STATUS_TEXT_CD;
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        var heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                        apm_mode = (int)heartbeat.custom_mode;
                        if (apm_mode != (int)MAVLink.COPTER_MODE.GUIDED)
                        {
                            wp.SetActive(false);
                        }
                        if ((heartbeat.base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) == 0)
                        {
                            if (armed)
                            {
                                MyDroneModel.GetComponent<DroneAnime>().PropellerRun = false;
                            }
                            armed = false;
                        }
                        else
                        {
                            if (!armed)
                            {
                                MyDroneModel.GetComponent<DroneAnime>().PropellerRun = true;
                            }
                            armed = true;
                        }
                        system_status = heartbeat.system_status;
                        if (!pos_tgt_local_rcved && (apm_mode == (int)MAVLink.COPTER_MODE.GUIDED))
                        {
                            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
                            {
                                command = (ushort)MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                                param1 = (float)MAVLink.MAVLINK_MSG_ID.POSITION_TARGET_LOCAL_NED,
                                param2 = 1000000
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
                            sock.SendTo(data, myproxy);
                        }
                        if (!mis_cur_rcved && (apm_mode == (int)MAVLink.COPTER_MODE.AUTO))
                        {
                            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
                            {
                                command = (ushort)MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                                param1 = (float)MAVLink.MAVLINK_MSG_ID.MISSION_CURRENT,
                                param2 = 1000000
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
                            sock.SendTo(data, myproxy);
                        }
                        if (mission_count < 0)
                        {
                            MAVLink.mavlink_mission_request_list_t cmd = new MAVLink.mavlink_mission_request_list_t
                            {
                                target_system = 0,
                                target_component = 0,
                                mission_type = 0
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST, cmd);
                            sock.SendTo(data, myproxy);
                            //Debug.Log("send MISSION_REQUEST_LIST");
                        }
                        if ((wait_mission_seq >= 0) && (wait_mission_seq < mission_count))
                        {
                            MAVLink.mavlink_mission_request_int_t cmd = new MAVLink.mavlink_mission_request_int_t
                            {
                                target_system = 0,
                                target_component = 0,
                                seq = (ushort)wait_mission_seq,
                                mission_type = 0
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_INT, cmd);
                            sock.SendTo(data, myproxy);
                            //Debug.Log("send MISSION_REQUEST_INT " + wait_mission_seq);
                        }
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.POSITION_TARGET_LOCAL_NED)
                    {
                        pos_tgt_local_rcved = true;
                        var pos_tgt = (MAVLink.mavlink_position_target_local_ned_t)msg.data;
                        if (((pos_tgt.type_mask & 0x1000) == 0x1000) || system_status != (byte)MAVLink.MAV_STATE.ACTIVE)
                        {
                            wp.SetActive(false);
                        }
                        else
                        {
                            wp.transform.position = new Vector3(-pos_tgt.x, -pos_tgt.z, pos_tgt.y);
                            wp.SetActive(true);
                        }
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_CURRENT)
                    {
                        mis_cur_rcved = true;
                        cur_mis_seq = ((MAVLink.mavlink_mission_current_t)msg.data).seq;
                        //Debug.Log("rcv MISSION_CURRENT " + cur_mis_seq);
                        if (cur_mis_seq < nxt_wp_seq)
                        {
                            MAVLink.mavlink_mission_set_current_t cmd = new MAVLink.mavlink_mission_set_current_t
                            {
                                target_system = 0,
                                target_component = 0,
                                seq = (ushort)nxt_wp_seq
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_SET_CURRENT, cmd);
                            sock.SendTo(data, myproxy);
                        }
                        else if (mission_chk_points.Contains(cur_mis_seq))
                        {
                            waiting_in_chk_point = true;
                        }
                        else
                        {
                            waiting_in_chk_point = false;
                        }
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_COUNT)
                    {
                        mission_count = ((MAVLink.mavlink_mission_count_t)msg.data).count;
                        if (mission_count > 0)
                        {
                            MAVLink.mavlink_mission_request_int_t cmd = new MAVLink.mavlink_mission_request_int_t
                            {
                                target_system = 0,
                                target_component = 0,
                                seq = 0,
                                mission_type = 0
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_INT, cmd);
                            sock.SendTo(data, myproxy);
                            wait_mission_seq = 0;
                            //Debug.Log("send MISSION_REQUEST_INT 0");
                        }
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT)
                    {
                        var item_int = (MAVLink.mavlink_mission_item_int_t)msg.data;
                        if (item_int.seq == wait_mission_seq)
                        {
                            if (item_int.command == 93) //MAV_CMD_NAV_DELAY
                            {
                                mission_chk_points.Add(item_int.seq);
                            }
                            wait_mission_seq += 1;
                            if (wait_mission_seq < mission_count)
                            {
                                MAVLink.mavlink_mission_request_int_t cmd = new MAVLink.mavlink_mission_request_int_t
                                {
                                    target_system = 0,
                                    target_component = 0,
                                    seq = (ushort)wait_mission_seq,
                                    mission_type = 0
                                };
                                byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_INT, cmd);
                                sock.SendTo(data, myproxy);
                                //Debug.Log("send MISSION_REQUEST_INT " + wait_mission_seq);
                            }
                            else
                            {
                                MAVLink.mavlink_mission_ack_t cmd = new MAVLink.mavlink_mission_ack_t
                                {
                                    target_system = 0,
                                    target_component = 0,
                                    type = 0,
                                    mission_type = 0
                                };
                                byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_ACK, cmd);
                                sock.SendTo(data, myproxy);
                                status_text = "\nmission dl ok";
                                status_text_timeout = STATUS_TEXT_CD;
                                //Debug.Log("send MISSION_ACK");
                            }
                        }
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST)
                    {
                        var seq = ((MAVLink.mavlink_mission_request_t)msg.data).seq;
                        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT, upload_mission[seq]);
                        sock.SendTo(data, myproxy);
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_INT)
                    {
                        var seq = ((MAVLink.mavlink_mission_request_int_t)msg.data).seq;
                        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT, upload_mission[seq]);
                        sock.SendTo(data, myproxy);
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_ACK)
                    {
                        status_text = "\nmission ack " + ((MAVLink.mavlink_mission_ack_t)msg.data).type;
                        status_text_timeout = STATUS_TEXT_CD;
                    }
                    else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.ATT_POS_MOCAP)
                    {
                        att_pos_update_int = 0f;
                        var att_pos = (MAVLink.mavlink_att_pos_mocap_t)msg.data;
                        gameObject.transform.localPosition = new Vector3(-att_pos.x, att_pos.y, att_pos.z);
                        gameObject.transform.localRotation = new Quaternion(-att_pos.q[1], att_pos.q[2], att_pos.q[3], -att_pos.q[0]);
                    }
                    /*else if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_REACHED)
                    {
                        Debug.Log("rcv MISSION_ITEM_REACHED "+((MAVLink.mavlink_mission_item_reached_t)msg.data).seq);
                        int seq = ((MAVLink.mavlink_mission_item_reached_t)msg.data).seq;
                        if (mis_chk_points.Contains(seq))
                        {
                            MAVLink.mavlink_mission_set_current_t cmd = new MAVLink.mavlink_mission_set_current_t
                            {
                                target_system = 0,
                                target_component = 0,
                                seq = (ushort)(seq + 2)
                            };
                            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_SET_CURRENT, cmd);
                            sock.SendTo(data, myproxy);
                        }
                    }*/
                }
            }
        }
        if (att_pos_update_int > 5f)
        {
            tracking = false;
            att_pos_update_int = 5f;
        }
        else
        {
            tracking = true;
        }
        if (tracking)
        {
            vel_update_int += Time.deltaTime;
            if (vel_update_int > 0.5f)
            {
                vel = (transform.localPosition - lastPos) / vel_update_int;
                lastPos = transform.localPosition;
                vel_update_int = 0f;
            }
        }
        if (!tracking)
        {
            if (MyDroneModel.activeSelf)
            {
                MyDroneModel.SetActive(false);
                GetComponent<BoxCollider>().enabled = false;
            }
        }
        else if (!MyDroneModel.activeSelf)
        {
            MyDroneModel.SetActive(true);
            GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void SendMissionCount(ushort count)
    {
        if (selected)
        {
            MAVLink.mavlink_mission_count_t cmd = new MAVLink.mavlink_mission_count_t
            {
                target_system = 0,
                target_component = 0,
                count = count,
                mission_type = 0
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_COUNT, cmd);
            sock.SendTo(data, myproxy);
        }
    }

    public void NextWP()
    {
        if (selected)
        {
            MAVLink.mavlink_mission_set_current_t cmd = new MAVLink.mavlink_mission_set_current_t
            {
                target_system = 0,
                target_component = 0,
                seq = (ushort)(cur_mis_seq + 1)
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.MISSION_SET_CURRENT, cmd);
            sock.SendTo(data, myproxy);
            nxt_wp_seq = cur_mis_seq + 1;
            Debug.Log("NextWP " + nxt_wp_seq);
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

    public void Disarm(bool forced = true)
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

    public void DeSelected()
    {
        selected = false;
    }

    public void TakeOff()
    {
        if (selected)
        {
            MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
            {
                command = (ushort)MAVLink.MAV_CMD.TAKEOFF,
                param7 = 1
            };
            byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
            sock.SendTo(data, myproxy);
        }
    }

    public void Guided()
    {
        if (selected)
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

    public void Land()
    {
        if (selected)
        {
            MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
            {
                base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                target_system = 0,
                custom_mode = (uint)MAVLink.COPTER_MODE.LAND
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

    public bool Goto(float x, float y, float z)
    {
        if (selected)
        {
            if (apm_mode != (int)MAVLink.COPTER_MODE.GUIDED)
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
            return true;
        }
        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick " + DroneID);
        GameObject[] drones = GameObject.FindGameObjectsWithTag("Drones");
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().DeSelected();
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
        if (!tracking) return;

        Vector3 pos = cam.WorldToScreenPoint(gameObject.transform.position);

        var unity_pos = transform.localPosition;
        var neu_pos = new Vector3(-unity_pos.x, unity_pos.z, unity_pos.y);
        var pos_info = neu_pos.ToString("F2");

        var neu_vel = new Vector3(-vel.x, vel.z, vel.y);
        var vel_info = neu_vel.ToString("F2");

        GUIContent content = new GUIContent("MAV" + DroneID + "\n" + (MAVLink.COPTER_MODE)apm_mode + "\n" + pos_info + "\n" + vel_info + status_text);
        if (selected)
        {
            Vector2 sz = selectedStyle.CalcSize(content);
            GUI.Label(new Rect(pos.x, Screen.height - pos.y + 30, sz.x, sz.y), content, selectedStyle);
        }
        else
        {
            Vector2 sz = GUIStyle.none.CalcSize(content);
            GUI.Label(new Rect(pos.x, Screen.height - pos.y + 30, sz.x, sz.y), content, GUIStyle.none);
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
        if (!tracking) return;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        //GL.MultMatrix(transform.localToWorldMatrix);
        GL.LoadPixelMatrix();

        //draw drone heading line
        GL.Begin(GL.LINES);
        GL.Color(Color.blue);
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        GL.Vertex3(pos.x, pos.y, 0);
        pos = Camera.main.WorldToScreenPoint(transform.position - transform.right * 0.5f);
        GL.Vertex3(pos.x, pos.y, 0);
        GL.End();

        if (wp.activeSelf)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            pos = Camera.main.WorldToScreenPoint(transform.position);
            GL.Vertex3(pos.x, pos.y, 0);
            pos = Camera.main.WorldToScreenPoint(wp.transform.position);
            GL.Vertex3(pos.x, pos.y, 0);
            GL.End();
        }

        GL.PopMatrix();
    }

    public void Selected()
    {
        if (!tracking) return;

        selected = true;
    }

    public bool UploadMission(string[] csv_lines)
    {
        if (selected)
        {
            upload_mission.Clear();
            mission_chk_points.Clear();
            foreach (var csv_line in csv_lines)
            {
                var wp = csv_line.Split(',');
                if (wp.Length == 12)
                {
                    try
                    {
                        MAVLink.mavlink_mission_item_int_t mission_item = new MAVLink.mavlink_mission_item_int_t
                        {
                            seq = (ushort)int.Parse(wp[0]),
                            command = (ushort)int.Parse(wp[3]),
                            param1 = float.Parse(wp[4]),
                            param2 = float.Parse(wp[5]),
                            param3 = float.Parse(wp[6]),
                            param4 = float.Parse(wp[7]),
                            x = int.Parse(wp[8]),
                            y = int.Parse(wp[9]),
                            z = int.Parse(wp[10]),
                            autocontinue = byte.Parse(wp[11])
                        };
                        upload_mission.Add(mission_item);
                        if (mission_item.command == 93) //MAV_CMD_NAV_DELAY
                        {
                            mission_chk_points.Add(mission_item.seq);
                        }
                    }
                    catch (System.Exception)
                    {
                        break;
                    }                   
                }
            }
            if (upload_mission.Count > 0) SendMissionCount((ushort)upload_mission.Count);
            return true;
        }
        return false;
    }
}
