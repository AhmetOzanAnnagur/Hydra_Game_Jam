using UnityEngine;

public class CrosshairFollow : MonoBehaviour
{
    public Camera cam;
    public bool hideSystemCursor = true;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        if (hideSystemCursor) Cursor.visible = false;
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 world = cam.ScreenToWorldPoint(mouse);
        world.z = 0f;
        transform.position = world;
    }
}
