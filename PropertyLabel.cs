using Godot;
using System;

public class PropertyLabel : Label
{
    private int propertyIndex;

    ~PropertyLabel()
    {
        StorageManager.UnsubscribeToPropertyChange(PropertyKeys.COIN_COUNT, propertyIndex);
    }

    public override void _Ready()
    {
        propertyIndex = StorageManager.SubscibeToPropertyChange(PropertyKeys.COIN_COUNT, (key, value) => Text = value.ToString());

        Text = StorageManager.GetInt(PropertyKeys.COIN_COUNT).ToString();
    }
}
