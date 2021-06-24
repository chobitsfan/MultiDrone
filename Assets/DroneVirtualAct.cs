using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneVirtualAct : MonoBehaviour
{
    const int MAX_POIONTS = 100;
    LineRenderer pastPathRender;
    Vector3 lastPos = Vector3.zero;
    Queue<Vector3> pastPath = new Queue<Vector3>(MAX_POIONTS);
    // Start is called before the first frame update
    void Start()
    {
        pastPathRender = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - lastPos).sqrMagnitude > 0.04f)
        {
            if (pastPath.Count >= MAX_POIONTS)
            {
                pastPath.Dequeue();
            }
            pastPath.Enqueue(transform.position);
            pastPathRender.positionCount = pastPath.Count;
            pastPathRender.SetPositions(pastPath.ToArray());
            lastPos = transform.position;
        }
    }
}
