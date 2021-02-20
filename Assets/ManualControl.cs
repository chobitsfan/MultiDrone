using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManualControl : MonoBehaviour
{
    GameObject[] drones;
    short pitch, roll, throttle, yaw;
    float controlTs = 0;

    private void Start()
    {
        drones = GameObject.FindGameObjectsWithTag("Drones");
    }
    public void OnArm()
    {
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().Arm();
        }
    }

    public void OnDisarm()
    {
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().Disarm(true);
        }
    }

    public void OnThrottleYaw(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        //Debug.Log("OnThrottleYaw"+ v);
        throttle = (short)((v.y + 1f) * 500f);
        yaw = (short)(v.x * 1000f);
        //Control();
    }

    public void OnPitchRoll(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        //Debug.Log("OnPitchRoll"+v);
        pitch = (short)(v.y * 1000f);
        roll = (short)(v.x * 1000f);
        //Control();
    }

    public void OnStabilize()
    {
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().Stabilize();
        }
    }

    public void OnPoshold()
    {
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().Poshold();
        }
    }

    private void FixedUpdate()
    {
        controlTs += Time.fixedDeltaTime;
        if (controlTs > 0.05f)
        {
            controlTs = 0;
            foreach (GameObject drone in drones)
            {
                drone.GetComponent<DroneAct>().ManualControl(pitch, roll, throttle, yaw);
            }
        }
    }

    void Control()
    {
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().ManualControl(pitch, roll, throttle, yaw);
        }
    }
}
