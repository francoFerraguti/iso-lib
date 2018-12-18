using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingHelperScene2 : MonoBehaviour
{
    void Awake()
    {
        IsoCamera.Init();
        IsoMap.Load("Assets/Maps/TestMap1.map");
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
