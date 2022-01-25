using Godot;
using System;

public class PropertyBasedActivator : Node2D
{
    [Export]
    public string property;
    [Export]
    public bool updateOnChange = true;

    public CanvasItem activeChild;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (updateOnChange)
        {
            StorageManager.SubscibeToPropertyChange(property, (key, value) => {
                Activate();
                Update();
            });
        }

        Activate();
    }

    public void Activate()
    {
        foreach (CanvasItem item in GetChildren())
        {
            item.Visible = false;
        }

        var index = StorageManager.GetInt(property);

        var child = GetChildren()[index] as CanvasItem;
        child.Visible = true;

        activeChild = child;
    }
}
