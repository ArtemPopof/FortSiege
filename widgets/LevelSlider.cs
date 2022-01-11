using Godot;
using System;

public class LevelSlider : ColorRect
{
    [Export]
    private float startLevel = 0;

    [Signal]
    public delegate void LevelChanged(float level);

    private float currentLevel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentLevel = startLevel;
    }
    public override void _Draw()
    {
        var x = 0;
        var y = RectSize.y * (1 - currentLevel);
        var width = RectSize.x;
        var height = RectSize.y - y;

        // make color dynamic
        var color = Colors.OrangeRed;

        if (currentLevel != 0)
        {
            DrawRect(new Rect2(x, y, new Vector2(width, height)), color);
        }
    }

    public void OnInput(InputEvent @event)
    {
        if (!Visible)
        {
            return;
        }

		if (!(@event is InputEventScreenDrag drag))
		{
            return;
		}

        currentLevel = (RectSize.y - drag.Position.y) / RectSize.y; 
        if (currentLevel < 0) currentLevel = 0;
        if (currentLevel > 1) currentLevel = 1;

        EmitSignal("LevelChanged", currentLevel);
        Update();
    }

    public void SetLevel(float value)
    {
        currentLevel = value;
        Update();
    }
}
