using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public int TargetID;
    public UnityEngine.UI.Text StatusText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("escape pressed");
            StatusText.text = "All MAV selected";
            TargetID = 0;
        }
    }
}
