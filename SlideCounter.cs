using Godot;
using System;

public class SlideCounter : ColorRect
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private const int CIRCLE_SIZE = 15;
    private const float SPACING = CIRCLE_SIZE + 7;

    [Export]
    public int itemCount = 1;

    private int currentIndex;

    private float startX;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentIndex = 0;

        var sizeOfWidgets = itemCount * CIRCLE_SIZE + (itemCount - 1) * SPACING;
        startX = (RectSize.x / 2) - sizeOfWidgets / 2;
    }

    public override void _Draw()
    {
        for (int i = 0; i < itemCount; i++)
        {
            DrawCircle(new Vector2(startX + (i * (CIRCLE_SIZE + SPACING)), RectSize.y + CIRCLE_SIZE), 12, i == currentIndex ? Constants.BUTTON_COLOR : Constants.BUTTON_DISABLED_COLOR);
        }
    }

    public void SetIndex(int index)
    {
        GD.Print("Set index: " + index);
        currentIndex = index;
        Update();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
