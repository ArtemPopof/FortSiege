using Godot;
using System;

public class PropertyBasedActivator : Node2D
{
    [Export]
    public string property;

    public Node2D activeChild;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void Activate()
    {
        var index = StorageManager.GetInt(property);

        //var child = GetChildren()[index] as Weapon;
        var child = GetChildren()[index] as Node2D;
        child.Visible = true;

        activeChild = child;

        //child.info = Data.weapons[child.GetIndex()];
    }
}
