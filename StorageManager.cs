using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System;

public class StorageManager {

    public static Dictionary data;
    public static File file;

    public static System.Collections.Generic.Dictionary<string, object> gameProperties;

    public static System.Collections.Generic.Dictionary<string, List<Action<string, object>>> propertyChangeListeners = 
    new System.Collections.Generic.Dictionary<string, List<Action<string, object>>>(10);

    // true if some properties was loaded, false if properties are empty
    public static bool Init()
    {        
        // var directory = new Directory();
        // directory.Remove("user://data.txt");

        GD.Print("Reading saved state from disk...");

        file = new File();
        file.Open("user://data.txt", File.ModeFlags.Read);
        var text = file.GetAsText();

        if (text.Trim() == "") {
            GD.Print("No properties to load");
            data = new Dictionary();
            file.Close();
            return false;
        }

        var result = JSON.Parse(text).Result;
        var dataLocal = (Dictionary) result;
        data = dataLocal;

        file.Close();

        gameProperties = new System.Collections.Generic.Dictionary<string, object>();

        GD.Print("State restored, size: " + text.Length);

        return true;
    }

    public static int SubscibeToPropertyChange(string key, Action<string, object> callback)
    {
        if (!propertyChangeListeners.ContainsKey(key))
        {
            propertyChangeListeners.Add(key, new List<Action<string, object>>(5));
        }

        var list = propertyChangeListeners[key];

        list.Add(callback);

        return list.Count - 1;
    }

    public static void UnsubscribeToPropertyChange(string key, int index)
    {
        var list = propertyChangeListeners[key];
        list.RemoveAt(index);
    }

    public static void StoreValue(string key, object value)
    {
        data[key] = value;

        NotifyListeners(key, value);
    }

    public static void IncreaseValue(string key, int value)
    {
        data[key] = ((int) data[key]) + value;

        NotifyListeners(key, value);
    }

    private static void NotifyListeners(string key, object value)
    {
        if (!propertyChangeListeners.ContainsKey(key)) return;

        var list = propertyChangeListeners[key];

        foreach (var listener in list)
        {
            listener.Invoke(key, value);
        }
    }

    public static void Save()
    {        
        GD.Print("Saving state to disk... ");

        var backup = new File();
        backup.Open("user://backup.txt", File.ModeFlags.Write);
        backup.StoreString(JSON.Print(data));
        backup.Close();

        var directory = new Directory();
        directory.Remove("user://data.txt");
        
        file = new File();
        file.Open("user://data.txt", File.ModeFlags.Write);
        file.StoreString(JSON.Print(data));
        file.Close();

        GD.Print("State saved");
    }

    public static void Clear()
    {
        GD.Print("[Storage Manager] clear all saved data");

        var directory = new Directory();
        directory.Remove("user://data.txt");
    }

    public static int GetInt(string key) 
    {
        var local = data;
        return !data.Contains(key) ? 0 : System.Convert.ToInt32(data[key]);
    }

    public static string GetString(string key)
    {
        return !data.Contains(key) ? null : data[key].ToString();
    }
}