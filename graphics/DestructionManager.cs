using Godot;
using System;
using System.Collections.Generic;

public class DestructionManager
{
    private int detalisationLevel;

    public DestructionManager(int detalisationLevel)
    {
        this.detalisationLevel = detalisationLevel;
    }
    

    public List<RigidBody2D> TearApart(TextureRect textureRect) 
    {
        GD.Print("[Destruction Manager] Trying to destroy object to pieces " + textureRect.GetParent().Name);
        GD.Print("[Destruction Manager] Number of pieces: " + (2 ^ detalisationLevel));

        var parts = new List<RigidBody2D>(2^detalisationLevel);
        var polygons = new List<Polygon2D>(2^detalisationLevel);

        var polygon = new Polygon2D();
        polygon.Texture = textureRect.Texture;
        
        TearApart(polygon, 1, polygons);

        return parts;
    }

    private void TearApart(Polygon2D rect, int level, List<Polygon2D> polygons)
    {
        var textureSize = rect.Texture.GetSize();

        var crackX1 = GD.Randf() * textureSize.x;
        var crackY1 = GD.Randf() * textureSize.y;
        var crackX2 = GD.Randf() * textureSize.x;
        var crackY2 = GD.Randf() * textureSize.y;

        int crackSide1 = ((int) Math.Round(GD.Randf() * 3));
        int crackSide2 = 1 + ((int) Math.Round(GD.Randf() * 2)) + crackSide1;

        if (crackSide1 == 0)
        {
            crackY1 = 0;
        }
        if (crackSide1 == 1)
        {
            crackX1 = textureSize.x;
        }
        if (crackSide1 == 2)
        {
            crackY1 = textureSize.y;
        }
        if (crackSide1 == 3)
        {
            crackX1 = 0;
        }

        if (crackSide2 == 0)
        {
            crackY2 = 0;
        }
        if (crackSide2 == 1)
        {
            crackX2 = textureSize.x;
        }
        if (crackSide2 == 2)
        {
            crackY2 = textureSize.y;
        }
        if (crackSide2 == 3)
        {
            crackX2 = 0;
        }

        // this two parts is atomic, no need more fragments
        if (level == detalisationLevel)
        {
            var firstPolygon = new Polygon2D();
            firstPolygon.Polygon = new Vector2[4];
        }

        // TODO recursive call
        
    }
}
