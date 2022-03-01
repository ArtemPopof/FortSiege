using Godot;
using System;
using System.Collections.Generic;

public class Clouds : Node2D
{
    [Export]
    public List<Texture> cloudTextures;
    [Export]
    public int cloudCount = 10;
    [Export]
    public int maxY = 200;
    [Export]
    public float MinAlpha = 0;

    private List<Cloud> clouds;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void Reset()
    {
        GD.Print("Reset clouds");
    }

    public void Init() 
    {
        GD.Print("Initializing Clouds...");

        clouds = GenerateClouds(cloudCount);
    }

    private List<Cloud> GenerateClouds(int count) 
    {
        GD.Randomize();

        var generatedClouds = new List<Cloud>(count);

        for (int i = 0; i < count; i++)
        {
            var cloud = new Cloud();
            cloud.BasicAlpha = MinAlpha;
            cloud.Texture = cloudTextures[(int) (GD.RandRange(0, cloudTextures.Count - 1))];
            cloud.RectGlobalPosition = new Vector2(GetViewportRect().Size.x * GD.Randf(), maxY * GD.Randf());
            cloud.Visible = true;
            cloud.MouseFilter = Control.MouseFilterEnum.Ignore;
            cloud.Init();

            generatedClouds.Add(cloud);

            AddChild(cloud);
        }

        return generatedClouds;
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (clouds == null) return;

        foreach (var cloud in clouds)
        {
            cloud.Move(delta);
            if (cloud.RectGlobalPosition.x > GetViewportRect().Size.x) {
                Respawn(cloud);
            }
        }
    }

    private void Respawn(Cloud cloud)
    {
        //GD.Print("Respawn cloud");
        var visible = GD.Randf() < 0.3;
        cloud.Visible = visible;
        if (!visible) return;
        //cloud.Texture = cloudTextures[(int) (GD.RandRange(0, cloudTextures.Count - 1))];
        cloud.Rerandomize();
        cloud.RectGlobalPosition = new Vector2(0 - cloud.RectSize.x * cloud.RectScale.x, maxY * GD.Randf());
    }
}
