using Godot;
using System.Collections.Generic;

public class ObjectManager
{
    private static ObjectManager instance;

    private Dictionary<string, object> singletonObjects;

    public static void Init()
    {
        GD.Print("[ObjectManager] Init...");

        instance = new ObjectManager();

        instance.singletonObjects = new Dictionary<string, object>();
    }

    public static void RegisterSingleton(string key, object objectValue)
    {
        GD.Print("[ObjectManager] Register singleton " + key);
        instance.singletonObjects.Add(key, objectValue);
    }

    public static T GetSingleton<T>(string key)
    {
        return (T) instance.singletonObjects[key];
    }
}
