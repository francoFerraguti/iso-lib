using UnityEngine;
using System.IO;
using System.Collections;
public static class IsoMap
{
    public static int[,] map;
    public static int width = 0;
    public static int height = 0;

    public static void Load(string mapPath)
    {
        Read(mapPath);
        Create();
    }

    private static void CreateTile(int x, int y)
    {
        Vector3 topLeft = new Vector3(-6.4f * (float)map.GetLength(0) / 2.0f, 0, 6.4f * (float)map.GetLength(1) / 2.0f);
        Vector3 displacement = new Vector3(6.4f * x, 0, -6.4f * y);
        switch (map[x, y])
        {
            case 0:
                break;
            case 1:
                MonoBehaviour.Instantiate(Resources.Load("Prefabs/TestGrass2"), topLeft + displacement, Quaternion.identity);
                break;
            case 2:
                MonoBehaviour.Instantiate(Resources.Load("Prefabs/TestGrass"), topLeft + displacement, Quaternion.identity);
                break;
            case 3:
                MonoBehaviour.Instantiate(Resources.Load("Prefabs/TestRock"), topLeft + displacement, Quaternion.identity);
                break;
            case 4:
                MonoBehaviour.Instantiate(Resources.Load("Prefabs/TestWater"), topLeft + displacement, Quaternion.identity);
                break;
        }
    }

    private static void Create()
    {
        for (int i = 0; i < map.GetLength(1); i++)
        {
            for (int j = 0; j < map.GetLength(0); j++)
            {
                CreateTile(j, i);
            }
        }
    }

    private static void Read(string mapPath)
    {
        StreamReader reader = new StreamReader(mapPath);
        string line = "";

        while ((line = reader.ReadLine()) != null)
        {
            width = line.Length;
            height++;
        }

        reader = new StreamReader(mapPath);
        map = new int[width, height];
        int rowCount = 0;
        int columnCount = 0;

        while ((line = reader.ReadLine()) != null)
        {
            for (rowCount = 0; rowCount < line.Length; rowCount++)
            {
                map[rowCount, columnCount] = line[rowCount] - 48;
            }
            columnCount++;
        }
    }
}
