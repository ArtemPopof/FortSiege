using Godot;
using System;

public class PropertyLabel : Label, PropertyChangeListener
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StorageManager.SubscibeToPropertyChange(PropertyKeys.COIN_COUNT, this);

        Text = StorageManager.GetInt(PropertyKeys.COIN_COUNT).ToString();
    }

    public void PropertyChanged(string key, object value)
    {
        Text = value.ToString();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
