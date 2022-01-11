using Godot;
using System.Collections.Generic;

public abstract class Weapon : Node2D
{
    public WeaponInfo info;
    public FireState currentState;

    protected List<Node2D> usedProjectiles = new List<Node2D>(5);

    [Signal]
    public delegate void Fired(float xVelocity, float yVelocity);
    [Signal]
    public delegate void TrajectoryChanged(float value);
    [Signal]
    public delegate void ForceChanged(float value);

    public abstract void Reset();
    public abstract void SetEnabled(bool enabled);
    public abstract void Fire();
    public abstract void SetTrajectory(float value);
    public abstract void SetForce(float value);

    public void ClearProjectiles()
    {
        GD.Print("[Weapon] Cleaning projectiles");

        foreach (Node2D projectile in usedProjectiles)
        {
            projectile.QueueFree();
        }

        usedProjectiles.Clear();
    }

}