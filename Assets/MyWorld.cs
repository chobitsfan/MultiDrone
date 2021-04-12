using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyWorld : MonoBehaviour
{
    public InputField inputFieldX;
    public InputField inputFieldY;
    public InputField inputFieldZ;
    static Material lineMaterial;
    GameObject[] drones;

    private void Start()
    {
        drones = GameObject.FindGameObjectsWithTag("Drones");
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
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
        foreach (GameObject drone in drones)
        {
            drone.GetComponent<DroneAct>().Goto(x, y, z);
        }
    }
}
