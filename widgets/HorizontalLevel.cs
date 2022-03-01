using Godot;
using System;
using System.Collections.Generic;

public class HorizontalLevel : Node2D
{
    [Export]
    private int currentLevel;

    public override void _Ready()
    {
        UpdateLevel(currentLevel);
    }

    public void UpdateLevel(int level)
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            var child = GetChild(i) as Panel;
            child.Modulate = new Color(0, 0, 0, i <= level ? 1 : 0.4f);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
