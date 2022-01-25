using Godot;
using System;

public class PropertyLabel : Label
{
    public override void _Ready()
    {
        StorageManager.SubscibeToPropertyChange(PropertyKeys.COIN_COUNT, (key, value) => Text = value.ToString());

        Text = StorageManager.GetInt(PropertyKeys.COIN_COUNT).ToString();
    }
}
