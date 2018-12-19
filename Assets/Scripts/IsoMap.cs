using UnityEngine;
using System.IO;
using System.Collections;
public static class IsoMap
{
    public static string[,] map;
    public static GameObject[,] tiles;
    public static int width = 0;
    public static int height = 0;
    public static float tileWidth = 6.4f;
    public static float tileHeight = 6.4f;

    public static void Load(string mapPath)
    {
        Read(mapPath);
        Create();
        tiles[2, 2].SetActive(false);
    }

    private static GameObject CreateTile(int x, int y)
    {
        if (map[x, y] == "0")
        {
            return null;
        }

        Vector3 topLeft = new Vector3(-tileWidth * (float)map.GetLength(0) / 2.0f, 0, tileHeight * (float)map.GetLength(1) / 2.0f);
        Vector3 displacement = new Vector3(tileWidth * x, 0, -tileHeight * y);

        return (GameObject)MonoBehaviour.Instantiate(Resources.Load(IsoResources.Get(map[x, y])), topLeft + displacement, Quaternion.identity);
    }

    private static void Create()
    {
        for (int i = 0; i < map.GetLength(1); i++)
        {
            for (int j = 0; j < map.GetLength(0); j++)
            {
                tiles[j, i] = CreateTile(j, i);
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
        map = new string[width, height];
        tiles = new GameObject[width, height];
        int rowCount = 0;
        int columnCount = 0;

        while ((line = reader.ReadLine()) != null)
        {
            for (rowCount = 0; rowCount < line.Length; rowCount++)
            {
                map[rowCount, columnCount] = line[rowCount].ToString();
            }
            columnCount++;
        }

        Frani.DebugMatrix(map);
    }
}
