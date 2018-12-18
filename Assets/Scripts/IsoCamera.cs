using UnityEngine;

public static class IsoCamera
{
    public static float zoomMultiplier = 2.0f;

    public static void Init()
    {
        Camera.main.transform.position = new Vector3(0, -0.5f, 0);
        Camera.main.transform.eulerAngles = new Vector3(30, -45, 0);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5;
        Camera.main.nearClipPlane = -10;
        Camera.main.farClipPlane = 100;
    }

    public static void Zoom(float amount)
    {
        Camera.main.orthographicSize += amount * zoomMultiplier;
    }
}
