using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyWorld : MonoBehaviour
{
    public InputField inputFieldX;
    public InputField inputFieldY;
    public InputField inputFieldZ;
    public Text LogText;
    public GameObject FloorQuad;
    static Material lineMaterial;
    List<DroneAct> drones = new List<DroneAct>();
    float chk_cd = 0.5f;

    private void Start()
    {
        var droneGameObjects = GameObject.FindGameObjectsWithTag("Drones");
        foreach (var droneGameObject in droneGameObjects)
        {
            drones.Add(droneGameObject.GetComponent<DroneAct>());
        }
    }

    public void FloorXChanged(string text)
    {
        try
        {
            Vector3 scale = FloorQuad.transform.localScale;
            scale.x = int.Parse(text);
            FloorQuad.transform.localScale = scale; 
        }
        catch (Exception)
        {

        }
    }

    public void FloorYChanged(string text)
    {
        try
        {
            Vector3 scale = FloorQuad.transform.localScale;
            scale.y = int.Parse(text);
            FloorQuad.transform.localScale = scale;
        }
        catch (Exception)
        {

        }
    }

    private void Update()
    {
        chk_cd -= Time.deltaTime;
        if (chk_cd < 0)
        {
            chk_cd = 0.5f;
            /*bool all_waiting = true;
            foreach (var drone in drones)
            {
                if ((drone.apm_mode >= 0) && (!drone.waiting_in_chk_point))
                {
                    all_waiting = false;
                    break;
                }
            }*/
            bool all_waiting = false;
            foreach (var drone in drones)
            {
                if (drone.apm_mode >= 0)
                {
                    if (drone.waiting_in_chk_point)
                    {
                        all_waiting = true;
                    }
                    else
                    {
                        all_waiting = false;
                        break;
                    }
                }
            }
            if (all_waiting) NextWP();
        }
        
        /*if (Mouse.current.leftButton.isPressed)
        {
            float distance;
            var mouse_pos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouse_pos.x, mouse_pos.y));
            if (gnd_plane.Raycast(ray, out distance))
            {
                Debug.Log(ray.GetPoint(distance));
            }
        }*/
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
        Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));
        GL.Vertex3(pos.x, pos.y, 0);
        pos = Camera.main.WorldToScreenPoint(new Vector3(-1, 0, 0));
        GL.Vertex3(pos.x, pos.y, 0);
        //GL.Vertex3(0, 0, 0);
        //GL.Vertex3(-1, 0, 0);

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        pos = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));
        GL.Vertex3(pos.x, pos.y, 0);
        pos = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 1));
        GL.Vertex3(pos.x, pos.y, 0);
        //GL.Vertex3(0, 0, 0);
        //GL.Vertex3(0, 1, 0);

        GL.End();
        GL.PopMatrix();
    }

    public void Goto()
    {
        float x, y, z;
        try
        {
            x = float.Parse(inputFieldX.text);
            y = float.Parse(inputFieldY.text);
            z = float.Parse(inputFieldZ.text);
        }
        catch (Exception)
        {
            return;
        }
        foreach (var drone in drones)
        {
            if (drone.Goto(x, y, -z)) break;
        }
    }

    public void Guided()
    {
        foreach (var drone in drones)
        {
            drone.Guided();
        }
    }

    public void Arm()
    {
        foreach (var drone in drones)
        {
            drone.Arm();
        }
    }

    public void TakeOff()
    {
        foreach (var drone in drones)
        {
            drone.TakeOff();
        }
    }

    public void Land()
    {
        foreach (var drone in drones)
        {
            drone.Land();
        }
    }

    public void Disarm()
    {
        foreach (var drone in drones)
        {
            drone.Disarm();
        }
    }

    public void Auto()
    {
        foreach (var drone in drones)
        {
            drone.Auto();
        }
    }

    public void SelectAll()
    {
        foreach (var drone in drones)
        {
            drone.Selected();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void NextWP()
    {
        foreach (var drone in drones)
        {
            drone.NextWP();
        }
    }

    public void OpenMissionFile()
    {
        var open_result =  SFB.StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        if (open_result.Length > 0)
        {
            var log_filename = open_result[0];
            var csv_lines = System.IO.File.ReadAllLines(log_filename);
            foreach (var drone in drones)
            {
                if (drone.UploadMission(csv_lines)) break;
            }
        }
    }

    public void StatusText(string text)
    {
        LogText.text = text;
    }
}
