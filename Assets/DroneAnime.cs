using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAnime : MonoBehaviour
{
    public GameObject Propeller1;
    public GameObject Propeller2;
    public GameObject Propeller3;
    public GameObject Propeller4;
    [System.NonSerialized]
    public bool PropellerRun = false;

    // Update is called once per frame
    void Update()
    {
        if (PropellerRun)
        {
            Propeller1.transform.Rotate(0, 0, Time.deltaTime * 800, Space.Self);
            Propeller2.transform.Rotate(0, 0, Time.deltaTime * 800, Space.Self);
            Propeller3.transform.Rotate(0, 0, Time.deltaTime * 800, Space.Self);
            Propeller4.transform.Rotate(0, 0, Time.deltaTime * 800, Space.Self);
        }
    }
}
