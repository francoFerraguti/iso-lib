using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
public static class IsoResources
{
    public static Dictionary<string, string> resources;
    public static void Load(string configPath)
    {
        Read(configPath);
    }

    public static string Get(string key)
    {
        string value = "";
        resources.TryGetValue(key, out value);
        return value;
    }

    private static void Read(string configPath)
    {
        resources = new Dictionary<string, string>();
        StreamReader reader = new StreamReader(configPath);
        string line = "";
        string key = "";

        while ((line = reader.ReadLine()) != null)
        {
            if (line.Length == 1)
            {
                key = line;
            }
            else
            {
                resources.Add(key, line);
            }
        }
    }
}
