using Godot;
using System;

public enum SwipeState {
    IDLE,
    SWIPING,
    ANIMATING
}

public class Swiper : HBoxContainer
{
    [Signal]
    public delegate void Swiped(int currentIndex);
    [Signal]
    public delegate void SwipeCanceled();

    [Export]
    public float scrollEndingSpeed = 2.0f;
    [Export]
    public int itemCount = 1;

    private float minXToSwipe = 200f;

    public int CurrentIndex {private set; get;}

    private SwipeState state;

    private int animatingDirection;
    private float animatingTargetRectX;
    private float swipeStartPoint;
    private float swiperStartPoint;

    private float startGlobalX;

    private float slideWidth;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        state = SwipeState.IDLE;
        CurrentIndex = 0;
        slideWidth = (this.RectSize.x * this.RectScale.x) / (itemCount);
        minXToSwipe = (this.RectSize.x * this.RectScale.x) / (itemCount) / 6;
        scrollEndingSpeed = (this.RectSize.x * this.RectScale.x) / (itemCount) * 2;
        startGlobalX = RectGlobalPosition.x;

        GD.Print("start rect pos: " + RectPosition.x);
        GD.Print("Swipe speed: " + scrollEndingSpeed);
        GD.Print("Slide width: " + slideWidth);

        EmitSignal("Swiped", CurrentIndex);
    }

    public void SwipeTo(int index)
    {
        if (CurrentIndex == index) return;

        EmitSignal("Swiped", index);
        var currentRectX = RectPosition.x;
        var diff = (CurrentIndex - index);

        animatingTargetRectX = currentRectX + diff * slideWidth;
        state = SwipeState.ANIMATING;

        CurrentIndex = index;
    }

    public override void _Input(InputEvent @event)
	{                        
        if (!Visible) {
            return;
        }

        if (@event is InputEventScreenTouch touch)
        {
            if (touch.IsPressed() && touch.Position.x > startGlobalX && touch.Position.x < startGlobalX + slideWidth)
            {
                GD.Print("Start swiping");
                if (state == SwipeState.ANIMATING)
                {
                    GD.Print("Still animating previos swipe");
                    return;
                }
                state = SwipeState.SWIPING;
                swipeStartPoint = touch.Position.x;
                swiperStartPoint = RectPosition.x;
            } else if (state == SwipeState.SWIPING)
            {
                GD.Print("Released");
                state = SwipeState.ANIMATING;
                SwipeOrAbort();
            }

            return;
        }

		if (!(@event is InputEventScreenDrag drag) || state != SwipeState.SWIPING)
		{
            return;
		}

        //GD.Print("Dragging swiper: " + RectPosition.x);
        var diff = drag.Position.x - swipeStartPoint;
        RectPosition = new Vector2(swiperStartPoint + diff, RectPosition.y);

        //swiping to the left of 0 index
        if (RectPosition.x > slideWidth / 10.0f)
        {
            RectPosition = new Vector2(slideWidth / 10.0f, RectPosition.y);
        }

        var minX =  0 - (this.RectSize.x * this.RectScale.x) + slideWidth * 0.9f;
        if (RectPosition.x < minX)
        {
            RectPosition = new Vector2(minX, RectPosition.y);
        }
	}

    private void SwipeOrAbort()
    {
        var edgeLeft = RectPosition.x > slideWidth / 10.0f;

        var diff = RectPosition.x - swiperStartPoint;
        animatingDirection = diff > 0 ? 1 : -1;

        var cantSlide = (CurrentIndex - animatingDirection) < 0 || (CurrentIndex - animatingDirection) > itemCount - 1;

        if (edgeLeft || Mathf.Abs(diff) < minXToSwipe || cantSlide)
        {
            GD.Print("Not enough diff, abort swipe");
            animatingDirection *= -1;
            animatingTargetRectX = swiperStartPoint;
            return;
        }

        GD.Print("Swipe!");
        CurrentIndex -= animatingDirection;
        EmitSignal("Swiped", CurrentIndex);
        animatingTargetRectX = swiperStartPoint + animatingDirection * slideWidth;
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     if (state != SwipeState.ANIMATING)
     {
         return;
     }

     if (animatingDirection == 1)
     {
         var newX = Math.Min(animatingTargetRectX, RectPosition.x + scrollEndingSpeed * delta);
         RectPosition = new Vector2(newX, RectPosition.y);
     }
     else 
     {
        var newX = Math.Max(animatingTargetRectX, RectPosition.x - scrollEndingSpeed * delta);
        RectPosition = new Vector2(newX, RectPosition.y);   
     }

     if (RectPosition.x == animatingTargetRectX)
     {
         state = SwipeState.IDLE;
     }
 }
}
