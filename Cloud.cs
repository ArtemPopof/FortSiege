using Godot;
using System;

public class Cloud : TextureRect
{
    private const float MAX_SCALE = 0.25f;
    private const float MAX_SPEED = 22.0f;

    private const float TEXTURE_SCALE = 1 / 100f;

    private float scale;
    private float opacity;
    private float distance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void Init() 
    {
        Randomize();
    }

    public void Randomize()
    {
        distance = GD.Randf();

        scale = (GD.Randf() * MAX_SCALE) * (distance * 2);
        if (scale > MAX_SCALE) scale = MAX_SCALE;
        opacity = distance;

        Expand = true;
        RectSize = new Vector2(RectSize.x * 0.1f, RectScale.y * 0.1f);
        RectScale = new Vector2(scale, scale);

        Modulate = new Color(1, 1, 1, opacity);
    }

    public void Rerandomize()
    {
        distance = GD.Randf();

        scale = (GD.Randf() * MAX_SCALE) * (distance * 2);
        if (scale > MAX_SCALE) scale = MAX_SCALE;
        opacity = distance;

        Expand = true;
        //RectSize = new Vector2(RectSize.x * 0.1f, RectScale.y * 0.1f);
        RectScale = new Vector2(scale, scale);

        Modulate = new Color(1, 1, 1, opacity);
    }

    public void Move(float delta)
    {
        RectGlobalPosition = new Vector2(RectGlobalPosition.x + delta * MAX_SPEED * distance, RectGlobalPosition.y);
    }

    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
        
    // }
}