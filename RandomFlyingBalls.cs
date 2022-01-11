using Godot;
using System;

public class RandomFlyingBalls : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    
    [Export]
    public Rect2 areaToFire;

    [Export]
    public float fireDelay = 3.0f;

    [Export]
    public float maxVelocity = 1000;

    [Export]
    public bool enabled = false;

    private float currentTime = 0;

    private bool[] ballsFlying = new bool[3];
    private Ball[] balls = new Ball[3];

    private Random random = new Random();

    private int nextBallIndex = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i] = GetNode<Ball>("Ball" + (i + 1) + "/Ball");
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     if (enabled && currentTime >= fireDelay)
     {
         GD.Print("Fire random ball");
         _FireRandomBall();
         currentTime = 0;
     }

     currentTime += delta;
 }

 private void _FireRandomBall()
 {
     int ballIndex;
     if ((ballIndex = _GetRandomBallIndex()) == -1) return;

     _FireBall(ballIndex, _GetStartPoint(), _GetRandomVelocity());
 }

 private int _GetRandomBallIndex()
 {
    return nextBallIndex++ % 3;
 }

 private Vector2 _GetRandomVelocity()
 {
     return new Vector2((float) random.NextDouble() * maxVelocity, (float) random.NextDouble() * maxVelocity);
 }

 private Vector2 _GetStartPoint()
 {
     var border = random.Next(3);

     float x = random.Next((int) areaToFire.Size.x);
     float y = random.Next((int) areaToFire.Size.y);

    // upper border of area
     if (border == 0) {
         y = 0;
     }
     // right
     if (border == 1) {
         x = areaToFire.Size.x;
     }
     if (border == 2) {
         y = areaToFire.Size.y;
     }
     if (border ==3) {
         x = 0;
     }

    return new Vector2(x, y);
 }

 private void _FireBall(int index, Vector2 startPoint, Vector2 direction)
 {
     ballsFlying[index] = true;
     var ball = balls[index];
     ball.Position = startPoint;
     ball.LinearVelocity = direction;
     ball.GravityScale = 1;
 }
}
