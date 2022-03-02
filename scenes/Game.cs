using Godot;
using System;
using System.Threading;
using System.Collections.Generic;

public enum GameState
{
    ONGOING_GAME,
    LEVEL_CLEARED,
    GAME_OVER,
    NO_MORE_SHOTS
}

public enum FireState
{
    READY,
    SET_TRAJECTORY
}

public class Game : Node2D
{
    [Signal]
    public delegate void LevelChanged(Level level);
    [Signal]
    public delegate void ProgressChanged(float progress);

    private int currentLevel;
    private Level currentLevelObject;
    private bool gameStarted = false;

    private Node2D currentLevelNode;

    private bool levelCleared;
    private bool levelsCleared;

    private Weapon weapon;
    private EnemyCounterPanel counterPanel;
    private TimerWidget timer;
    private ShotCounter shotCounter;
    private Label levelDoneLabel;
    private Node2D fireButton;
    private Background background;
    private CoinSpawner coinSpawner;
    private CoinCounter coinCounter;
    private LevelSlider fireLevelSlider;
    private TrajectoryPainter trajectoryPainter;
    private Node2D menu;
    private StaticToCamera uiLayer;
    private AudioStreamPlayer2D backgroundMusic;
    private Joystick joystick;

    //private Node2D menuNode;
    private Header header;
    private MobileCamera camera;
    //private MainMapScene mainMenu;

    private float timeSinceLastShot;
    private bool fired;
    private GameState state;
    private FireState fireState;

    public override void _Ready()
    {
        new System.Threading.Thread(new ThreadStart(_Init)).Start();
    }

    public void _Init()
    {
        GD.Print("[Game] Start loading");

        uiLayer = GetNode<StaticToCamera>("UILayout");
        menu = GetNode<Node2D>("UILayout/Menu");
        counterPanel = GetNode<EnemyCounterPanel>("UILayout/EnemyCounterPanel");
        fireButton = GetNode<Node2D>("UILayout/FireButton");
        coinCounter = GetNode<CoinCounter>("UILayout/CoinCounter");
        fireLevelSlider = GetNode<LevelSlider>("UILayout/LevelSlider");
        backgroundMusic = GetNode<AudioStreamPlayer2D>("BackgroundMusic");
        trajectoryPainter = GetNode<TrajectoryPainter>("TrajectoryDisplayer");
        coinSpawner = GetNode<CoinSpawner>("CoinSpawner");
        joystick = GetNode<Joystick>("UILayout/Joystick");

        camera = main.camera;

        uiLayer.Connect(camera);

        coinSpawner.counter = coinCounter;

        EmitSignal("ProgressChanged", 10);

        StartGame(StorageManager.GetInt(PropertyKeys.CURRENT_LEVEL));
    }

    private void StartGame(int level) {
        currentLevel = level;
        gameStarted = true;
        fired = false;
        levelsCleared = true;
        state = GameState.ONGOING_GAME;

        menu.Visible = false;
        //mainMenu.Visible = false;
        //menuNode.Visible = false;

        fireLevelSlider.Visible = true;

        var weaponActivator = GetNode<PropertyBasedActivator>("Weapons");
        weaponActivator.Activate();

        EmitSignal("ProgressChanged", 15);

        weapon = weaponActivator.ActiveChild as Weapon;
        weapon.Reset();
        weapon.Visible = true;
        weapon.SetEnabled(true);
        weapon.Connect("Fired", this, "BallFired");
        weapon.Connect("ForceChanged", fireLevelSlider, "SetLevel");
        weapon.Connect("ProjectilePositionChanged", trajectoryPainter, "SetTrajectoryStart");
        weapon.Connect("FireVelocityChanged", trajectoryPainter, "SetStartVelocity");

        fireLevelSlider.Connect("LevelChanged", weapon, "SetForce");
        if (weapon.info.controlTrajectory)
        {
            joystick.Connect("VectorChanged", weapon, "SetTrajectory");
        }
        
        ResetFireState();

        timer = GetNode<TimerWidget>("UILayout/TimerWidget");
        shotCounter = GetNode<ShotCounter>("UILayout/ShotCounter");
        levelDoneLabel = GetNode<Label>("UILayout/LevelDone");
        background = GetNode<Background>("Backgound");

        EmitSignal("ProgressChanged", 20);
        
        timer.Reset(60);
        background.Init();
        coinCounter.SetCount(0);

        trajectoryPainter.SetTrajectoryStart(weapon.GetProjectileStartPosition());
        trajectoryPainter.Visible = true;
        trajectoryPainter.SetGroundY(GetNode<StaticBody2D>("StaticBody2D").GlobalPosition.y);

        GD.Print("Current coin count: " + coinCounter.count);

        EmitSignal("ProgressChanged", 25);
        
        LoadCurrentLevel();

        camera.Enabled = true;

        backgroundMusic.Playing = true;

        EmitSignal("ProgressChanged", 100);
    }

    private void ResetFireState()
    {
        GD.Print("[Main] Reset fire button state");
        if (weapon.info.controlTrajectory)
        {
            fireState = FireState.READY;
            GetNode<Label>("UILayout/FireButton/Label").Text = "Set";
        }
        else 
        {
            fireState = FireState.SET_TRAJECTORY;
            GetNode<Label>("UILayout/FireButton/Label").Text = "Fire";
        }

        fireLevelSlider.SetLevel(0.0f);
        //fireButton.Visible = true;
    }

    private void BallFired()
    {
        GD.Print("BallFired");
        if (!shotCounter.ShotPerformed()) {
            // no more shots
            return;
        }

        ResetFireState();
    }

    private void EnemyDied(Enemy enemy)
    {
        coinSpawner.SpawnCoins(enemy.coinReward, enemy.GlobalPosition);
    }

    public void NoMoreShots() {
        GD.Print("Main: No more shots");

        timeSinceLastShot = 0;
        state = GameState.NO_MORE_SHOTS;

        fireButton.Visible = false;
        //GameOver("Oh no, no more shots left!");
    }

    private void LoadCurrentLevel() {
        GD.Print("Loading level: Level" + currentLevel.ToString());

        // clean up objects
        coinSpawner.ClearCoins();
        weapon.ClearProjectiles();
        //

        var packedLevel = ResourceLoader.Load<PackedScene>("res://Level" + (currentLevel + 1).ToString() + ".tscn");
        var sceneObject = packedLevel.Instance<Node2D>();

        EmitSignal("ProgressChanged", 65);

        var instancePosition = GetNode<Position2D>("BuildingPosition");

        sceneObject.Position = instancePosition.Position;

        AddChildBelowNode(trajectoryPainter, sceneObject);

        if (currentLevelObject != null) {
            currentLevelObject.QueueFree();
        }

        currentLevelObject = sceneObject as Level;
        currentLevelObject.Connect("AllEnemiesAreDead", this, "AllEnemiesAreDead");
        currentLevelObject.Connect("AllPrefabsAreStill", this, "AllPrefabsAreStill");
        currentLevelObject.Connect("LevelEnemyDied", this, "EnemyDied");
        currentLevelObject.Connect("CoinCollected", this, "CoinCollected");
        currentLevelObject.Connect("ChestDestroyed", coinSpawner, "SpawnCoins");

        EmitSignal("ProgressChanged", 80);

        timer.Reset(60);
        counterPanel.Init(currentLevelObject.getEnemies());
        shotCounter.Init(weapon.info);
        
        currentLevelNode = sceneObject;

        EmitSignal("LevelChanged", currentLevelObject);
    }

    private void CoinCollected(int count)
    {
        coinCounter.SetCount(coinCounter.count + count);
    }

    private void AllPrefabsAreStill()
    {
        GD.Print("All prefabs stay still");
    }

    public void FireButtonPressed() {
        // if (fireState == FireState.READY)
        // {
        //     GD.Print("Set trajectory button pressed");
        //     fireState = FireState.SET_TRAJECTORY;
        //     weapon.currentState = FireState.SET_TRAJECTORY;
        //     GetNode<Label>("FireButton/Label").Text = "Fire";
        //     return;
        // }

        GD.Print("Fire button pressed");
        fired = true;
        weapon.Fire();
        //ResetFireState();
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		if (!(@event is InputEventScreenTouch touchEvent))
		{
            return;
		}
	}

    public void Restart() {
        GD.Print("Restart Level");
        
        coinCounter.SetCount(0);
        currentLevelObject.Visible = false;
        currentLevelObject.QueueFree();
        weapon.Reset();
        levelCleared = false;
        levelsCleared = false;

        state = GameState.ONGOING_GAME;
        LoadCurrentLevel();

        ResetFireState();
        
        levelDoneLabel.Visible = false;
        GetNode<Node2D>("UILayout/RestartButton").Visible = false;
    }

    private void LevelOver(String message, bool gameover = false) {
        levelDoneLabel.Text = message;
        levelDoneLabel.Visible = true;
        gameStarted = false;
        fireButton.Visible = false;

        GetNode<Label>("UILayout/RestartButton/HBoxContainer/MarginContainer/Label").Text = gameover ? "Restart" : "Complete";
        GetNode<Node2D>("UILayout/RestartButton").Visible = true;

        if (!gameover)
        {
            levelCleared = true;
        }

        timer.Stop();
        state = GameState.GAME_OVER;

        weapon.SetEnabled(false);
    }

    private void ChangeLevel() {
        levelCleared = false;
        currentLevel++;
        if (currentLevel > 4)
        {
            currentLevel = 1;
        }
        state = GameState.ONGOING_GAME;

        currentLevelNode.QueueFree();

        levelDoneLabel.Visible = false;
        ResetFireState();

        counterPanel.Reset();
        coinCounter.SetCount(0);
        weapon.Reset();      
        timer.Reset(60);
        background.Reset();
        fireLevelSlider.SetLevel(0);

        LoadCurrentLevel();
    }

    public void AllEnemiesAreDead() {
        levelCleared = true;
        var newCountCount = StorageManager.GetInt(PropertyKeys.COIN_COUNT) + coinCounter.count;
        var possesion = StorageManager.GetString(PropertyKeys.AVAILABLE_LEVELS);
        StorageManager.StoreValue(PropertyKeys.AVAILABLE_LEVELS, possesion + ";" + (currentLevel + 1));
        StorageManager.StoreValue(PropertyKeys.COIN_COUNT, newCountCount);
        StorageManager.Save();
        LevelOver("Level cleared!");
    }

    public void onTimeout() {
        GD.Print("Time's up");

        LevelOver("Time's up", true);
    }

 public override void _Process(float delta)
 {
     timeSinceLastShot += delta;

     if (state != GameState.NO_MORE_SHOTS) {
         return;
     }

     GD.Print("Moving prefabs: " + currentLevelObject.activePrefabs);

     if (timeSinceLastShot >= 3.5f)
     {
         var hasNoMovingPrefabs = currentLevelObject.activePrefabs == 0 && !isAnyMovingBalls(weapon.Balls);
         if (hasNoMovingPrefabs)
         {
             GD.Print("No more shots and every object is still. Game over");
             LevelOver("No more shots!", true);
         }
     }
 }

 private bool isAnyMovingBalls(List<Ball> balls)
 {
     foreach (var ball in balls)
     {
         if (ball.LinearVelocity.LengthSquared() > 5f)
         {
             return true;
         }
     }

     return false;
 }

 public void OnLevelOverButtonPressed()
 {
     GD.Print("Level cleared button presed");
     
     if (levelCleared)
     {
         GD.Print("Next Level");
         ChangeLevel(); 
     } else
     {
         GD.Print("Restart");
         Restart();
     }

     GetNode<Node2D>("UILayout/RestartButton").Visible = false;
 }
}