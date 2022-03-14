using Godot;
using System;

public class LevelCompleteDialog : Node2D
{
    [Signal]
    public delegate void Continue();

    [Export]
    public float animationDuration = 5f;

    private bool playingAnimation;
    private float secToWait = 0f;
    private int countedCoins;
    private CoinCounter counter;
    private float countSpeed;

    private Label coinLabel;
    private Rating ratingWidget;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Visible = false;
        playingAnimation = false;

        coinLabel = GetNode<Label>("Panel/VBoxContainer/CoinCount/Label");
        ratingWidget = GetNode<Rating>("Panel/VBoxContainer/RatingWidget");
    }

    public void Show(CoinCounter coinCounter, int rating)
    {
        this.counter = coinCounter;
        coinLabel.Text = "0";
        ratingWidget.Init(3, rating);

        secToWait = 1f;
    }

    public override void _Process(float delta)
    {
        if (secToWait != 0)
        {
            secToWait -= delta;
            if (secToWait <= 0)
            {
                countSpeed = counter.count / animationDuration;
                countedCoins = 0;
                playingAnimation = true;
                Visible = true;
                secToWait = 0f;
                return;
            }
        }

        if (!playingAnimation) return;

        countedCoins += (int) Mathf.Max(countSpeed * delta, 1f);

        if (countedCoins > counter.count)
        {
            countedCoins = counter.count;
            ratingWidget.Visible = true;
            playingAnimation = false;
        }

        coinLabel.Text = countedCoins.ToString();
    }

    public void ContinueButtonClicked()
    {
        EmitSignal("Continue");

        Visible = false;
    }

}
