using Godot;

public class WeaponInfo {
    public int index;
    public int cost;
    public string name;
    public int shotCount;
    public float projectileWeight;
    public bool inPossesion;
    public bool controlTrajectory;
    public Texture projectileTexture;
    public int power;

    public int maxPower;
    public int maxShotCount;
    public float maxProjectileWeight;


    public int GetWeaponUpgradeCost(float valueOfCurrentParam)
    {
        return (int) ((cost / 5) + (valueOfCurrentParam * 5));
    }
}