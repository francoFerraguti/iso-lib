using UnityEngine;
using System.Collections;
public static class IsoCamera
{
    private static bool zooming = false;
    public static float zoomMultiplier = 1.4f;
    public static int zoomSteps = 12;
    public static int rotationSpeed = 4; //can be 1, 2 or 4
    private static bool rotating = false;

    public static void Init()
    {
        Camera.main.transform.position = new Vector3(0, 0.5f, 0);
        Camera.main.transform.eulerAngles = new Vector3(30, -45, 0);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 27;
        Camera.main.nearClipPlane = -100;
        Camera.main.farClipPlane = 100;
    }

    public static IEnumerator Zoom(float amount)
    {
        if (zooming || amount == 0)
        {
            yield break;
        }
        zooming = true;

        for (int i = 0; i < zoomSteps; i++)
        {
            float skewValue = (amount > 0) ? Frani.GetInBetween(0, 2, zoomSteps, i) : Frani.GetInBetween(-2, 0, zoomSteps, i);
            Camera.main.orthographicSize += amount * zoomMultiplier + skewValue;
            yield return null;
        }

        zooming = false;
        yield break;
    }

    public static IEnumerator RotateCounterClockwise()
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        for (int angles = 0; angles < 44 / rotationSpeed; angles++)
        {
            Camera.main.transform.eulerAngles = new Vector3(30, Camera.main.transform.eulerAngles.y - rotationSpeed - Frani.GetInBetween(-3, 3, 44 / rotationSpeed, angles), 0);
            yield return null;
        }

        Camera.main.transform.eulerAngles = new Vector3(30, Camera.main.transform.eulerAngles.y - 1, 0);
        rotating = false;
        yield break;
    }

    public static IEnumerator RotateClockwise()
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        for (int angles = 0; angles < 44 / rotationSpeed; angles++)
        {
            Camera.main.transform.eulerAngles = new Vector3(30, Camera.main.transform.eulerAngles.y + rotationSpeed + Frani.GetInBetween(-3, 3, 44 / rotationSpeed, angles), 0);
            yield return null;
        }

        Camera.main.transform.eulerAngles = new Vector3(30, Camera.main.transform.eulerAngles.y + 1, 0);
        rotating = false;
        yield break;
    }
}
