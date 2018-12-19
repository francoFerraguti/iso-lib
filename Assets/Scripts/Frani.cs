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

    public static void DebugMatrix(string[,] matrix)
    {
        string row = "";

        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                row += matrix[j, i];
            }
            Debug.Log(row);
            row = "";
        }
    }


    public static void DebugMatrix(int[,] matrix)
    {
        string row = "";

        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                row += matrix[j, i];
            }
            Debug.Log(row);
            row = "";
        }
    }
}
