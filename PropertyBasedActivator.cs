using Godot;
using System;

public class PropertyBasedActivator : Node2D
{
    [Export]
    public string property;
    [Export]
    public bool updateOnChange = true;

    public Node2D ActiveChild { get; private set; }

    private int propertyIndex = -1;
    
    ~PropertyBasedActivator()
    {
        if (propertyIndex == -1) return;

        StorageManager.UnsubscribeToPropertyChange(property, propertyIndex);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (updateOnChange)
        {
            propertyIndex = StorageManager.SubscibeToPropertyChange(property, (key, value) => {
                if (!IsInstanceValid(this)) {
                    return;
                }

                Activate();
                Update();
            });
        } 

        Activate();
    }

    public void Activate()
    {
        foreach (Node2D item in GetChildren())
        {
            item.Visible = false;
        }

        var index = StorageManager.GetInt(property);

        var child = GetChildren()[index] as Node2D;
        child.Visible = true;

        ActiveChild = child;
    }
}
