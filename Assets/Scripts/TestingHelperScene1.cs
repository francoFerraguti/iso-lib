using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingHelperScene1 : MonoBehaviour
{
    void Awake()
    {
        IsoCamera.Init();
        IsoCamera.zoomMultiplier = 4;
    }

    void Update()
    {
        StartCoroutine(IsoCamera.Zoom(-Input.GetAxis("Mouse ScrollWheel")));

        if (Input.GetKeyDown("a"))
        {
            StartCoroutine(IsoCamera.RotateClockwise());
        }
        if (Input.GetKeyDown("d"))
        {
            StartCoroutine(IsoCamera.RotateCounterClockwise());
        }
    }
}
