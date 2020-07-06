using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class BtnAct : MonoBehaviour
{
    MAVLink.MavlinkParse mavlinkParse = new MAVLink.MavlinkParse();
    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 17500); 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GuidedClick()
    {
        //Debug.Log("clicked");
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = 0,
            custom_mode = 4
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, ep);

        StartCoroutine(SetGPSOrigin());
    }

    IEnumerator SetGPSOrigin()
    {
        yield return new WaitForSeconds(0.1f);
        MAVLink.mavlink_set_gps_global_origin_t cmd = new MAVLink.mavlink_set_gps_global_origin_t
        {
            latitude = (int)(24.7733321 * 10000000),
            longitude = (int)(121.0449535 * 10000000),
            altitude = 100
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_GPS_GLOBAL_ORIGIN, cmd);
        sock.SendTo(data, ep);
    }

    public void ArmClick()
    {
        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            target_system = 0,
            command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM,
            param1 = 1
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        sock.SendTo(data, ep);
    }

    public void TakeoffClick()
    {
        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            target_system = 0,
            command = (ushort)MAVLink.MAV_CMD.TAKEOFF,
            param7 = 0.7f
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        sock.SendTo(data, ep);
    }

    public void LandClick()
    {
        MAVLink.mavlink_set_mode_t cmd = new MAVLink.mavlink_set_mode_t
        {
            base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
            target_system = 0,
            custom_mode = 9
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.SET_MODE, cmd);
        sock.SendTo(data, ep);
    }

    public void DisarmClick()
    {
        MAVLink.mavlink_command_long_t cmd = new MAVLink.mavlink_command_long_t
        {
            command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM,
            target_system = 0,
            param1 = 0,
            param2 = 21196
        };
        byte[] data = mavlinkParse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        sock.SendTo(data, ep);
    }
}
