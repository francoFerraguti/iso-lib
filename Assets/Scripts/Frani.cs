using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Frani
{
    public static float GetInBetween(float minValue, float maxValue, int steps, int iteration)
    {
        float stepValue = (maxValue - minValue) / (float)(steps - 1);
        return maxValue - stepValue * iteration;
    }

}
