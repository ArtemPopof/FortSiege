using Godot;
using System;

public class Rating : Node2D
{
    [Export]
    public float spacing = 20f;

    private TextureRect filled;
    private TextureRect hollow;

    public override void _Ready()
    {
        GetNode<Node2D>("Reference").Visible = false;

        filled = GetNode<TextureRect>("Reference/Filled");
        hollow = GetNode<TextureRect>("Reference/Hollow");
    }

    public void Init(int maxRating, int currentRating)
    {
        GD.Print("[Rating Widget] Init rating " + currentRating);

        var widgetWidth = maxRating * filled.RectSize.x * filled.RectScale.x + ((maxRating - 1) * spacing);
        var currentX = -1f * widgetWidth / 2f;

        for (int i = 0; i < maxRating; i++)
        {
            SpawnStar(currentX, i < currentRating);
            currentX += spacing + filled.RectSize.x * filled.RectScale.x;
        }
    }

    private void SpawnStar(float x, bool spawnFilled)
    {
        TextureRect star = (TextureRect) (spawnFilled ? filled.Duplicate() : hollow.Duplicate());

        star.RectPosition = new Vector2(x, 0);

        star.Visible = true;

        AddChild(star);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
