using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyGroundFloor : MonoBehaviour, IPointerClickHandler
{
    public InputField inputFieldX;
    public InputField inputFieldY;
    Plane gnd_plane = new Plane(Vector3.up, 0);
    public void OnPointerClick(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(eventData.position.x, eventData.position.y));
        if (gnd_plane.Raycast(ray, out float distance))
        {
            var pos = ray.GetPoint(distance);
            //Debug.Log(pos);
            inputFieldX.text = (-pos.x).ToString("F2");
            inputFieldY.text = pos.z.ToString("F2");
        }
    }
}
