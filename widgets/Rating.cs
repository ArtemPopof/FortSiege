using Godot;
using System;
using System.Collections.Generic;

public class Rating : Node2D
{
    [Export]
    public float spacing = 20f;

    private TextureRect filled;
    private TextureRect hollow;

    private List<TextureRect> stars;

    public override void _Ready()
    {
        GetNode<Node2D>("Reference").Visible = false;

        filled = GetNode<TextureRect>("Reference/Filled");
        hollow = GetNode<TextureRect>("Reference/Hollow");

        stars = new List<TextureRect>(3);
    }

    public void Init(int maxRating, int currentRating)
    {
        GD.Print("[Rating Widget] Init rating " + currentRating);

        Reset();

        var widgetWidth = maxRating * filled.RectSize.x * filled.RectScale.x + ((maxRating - 1) * spacing);
        var currentX = -1f * widgetWidth / 2f;

        for (int i = 0; i < maxRating; i++)
        {
            stars.Add(SpawnStar(currentX, i < currentRating));
            currentX += spacing + filled.RectSize.x * filled.RectScale.x;
        }
    }

    private TextureRect SpawnStar(float x, bool spawnFilled)
    {
        TextureRect star = (TextureRect) (spawnFilled ? filled.Duplicate() : hollow.Duplicate());

        star.RectPosition = new Vector2(x, 0);

        star.Visible = true;

        AddChild(star);

        return star;
    }

    private void Reset()
    {
        stars.ForEach((rect) => {
            rect.QueueFree();
        });

        stars.Clear();
    }

}
