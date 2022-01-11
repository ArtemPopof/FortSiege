using Godot;
using System;
using System.Collections.Generic;

public class EnemyCounterPanel : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private class EnemyIcon {
        public bool crossed;
        public TextureRect texture;
        public TextureRect crossTexture;
        public Enemy enemy;
    }

    private TextureRect referenceEnemyImage;
    private TextureRect referenceCrossImage;

    private List<EnemyIcon> enemyIcons;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        referenceCrossImage = GetNode<TextureRect>("Cross");
        referenceEnemyImage = GetNode<TextureRect>("EnemyScoreImage");

        referenceEnemyImage.Visible = false;
        referenceCrossImage.Visible = false;

        enemyIcons = new List<EnemyIcon>();
    }

    public void Init(List<Enemy> enemies) {
        GD.Print("Init counter panel for " + enemies.Count + " enemies");

        Reset();

        for (int i = 0; i < enemies.Count; i++) {
            enemyIcons.Add(new EnemyIcon {
                enemy = enemies[i],
                crossed = false,
                texture = CreateEnemyImage(i)
            });

            enemies[i].Connect("EnemyDied", this, "EnemyDied");
        }
    }

    private TextureRect CreateEnemyImage(int index) {
        var image = referenceEnemyImage.Duplicate() as TextureRect;

        image.RectPosition = new Vector2(0 + (index * referenceEnemyImage.RectSize.x + 20), referenceEnemyImage.RectPosition.y);

        image.Visible = true;
        
        AddChild(image);

        return image;
    }

    public void EnemyDied(Enemy enemy) {
        GD.Print("Creating cross");

        enemyIcons.ForEach(enemyIcon => CrossEnemyIfDead(enemyIcon, enemy));
    }

    private void CrossEnemyIfDead(EnemyIcon enemyIcon, Enemy enemy) {
        if (enemyIcon.enemy != enemy) {
            return;
        }

        CreateCrossTexture(enemyIcon);
    }

    private void CreateCrossTexture(EnemyIcon enemyIcon) {
        var image = referenceCrossImage.Duplicate() as TextureRect;
        image.RectPosition = new Vector2(enemyIcon.texture.RectPosition.x + 3, enemyIcon.texture.RectPosition.y);
        image.Visible = true;
        
        AddChild(image);

        enemyIcon.crossTexture = image;
    }

    public void Reset() {
        if (enemyIcons == null) return;
        
        foreach (var enemyIcon in enemyIcons)
        {
            if (enemyIcon.texture != null) {
                enemyIcon.texture.Visible = false;
                enemyIcon.texture.QueueFree();
            }
            if (enemyIcon.crossTexture != null) {
                enemyIcon.crossTexture.Visible = false;
                enemyIcon.crossTexture.QueueFree();
            }
        }

        enemyIcons.Clear();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
