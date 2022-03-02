using System.Collections.Generic;
using Godot;

public class WeaponExtention 
{
    public long Index {get; set;}
    public string Name {get; set;}
    public int Price {get; set;}
}

public class Data
{
    public const int WEAPON_CATAPULT_ID = 0;
    public const int WEAPON_CANNON_ID = 1;

    public static List<WeaponInfo> weapons;
    public static List<WeaponExtention> extentions;

    public static Dictionary<int, List<WeaponExtention>> weaponExtentions;

    public static void Init()
    {
        GD.Print("Init data manager");

        if (!StorageManager.IsInitialized())
        {
            GD.Print("Storage manager should be initilized first");
            return;
        }

        var possesion = StorageManager.GetString(PropertyKeys.WEAPON_POSSESION_LIST);
        if (possesion == null)
        {
            InitData();
        }

        weapons = new List<WeaponInfo>(2);

        var possesionArray = Util.ToPossesionArray(possesion, weapons.Capacity);

        var catapult = new WeaponInfo();
        catapult.index = 0;
        catapult.cost = 20;
        catapult.name = "Catapult";
        catapult.controlTrajectory = false;
        catapult.inPossesion = possesionArray[WEAPON_CATAPULT_ID];
        catapult.shotCount = 2 + StorageManager.GetInt(PropertyKeys.WEAPON_SHOT_COUNT_MODIFICATION + "0");
        catapult.projectileTexture = ResourceLoader.Load("res://textures/CatapultProjectile.svg") as Texture;
        catapult.projectileWeight = 7 + StorageManager.GetInt(PropertyKeys.WEAPON_WEIGHT_MODIFICATION + "0");
        catapult.power = 1 + + StorageManager.GetInt(PropertyKeys.WEAPON_POWER_MODIFICATION + "0");

        catapult.maxPower = 3;
        catapult.maxProjectileWeight = 7;
        catapult.maxShotCount = 3;

        var cannon = new WeaponInfo();
        cannon.index = 1;
        cannon.cost = 70;
        cannon.name = "Cannon";
        cannon.shotCount = 2 + StorageManager.GetInt(PropertyKeys.WEAPON_SHOT_COUNT_MODIFICATION + "1");
        cannon.controlTrajectory = true;
        cannon.inPossesion = possesionArray[WEAPON_CANNON_ID];
        cannon.projectileTexture = ResourceLoader.Load("res://textures/CannonProjectile.png") as Texture;
        cannon.projectileWeight = 2 + StorageManager.GetInt(PropertyKeys.WEAPON_WEIGHT_MODIFICATION + "1");
        cannon.power = 3 + StorageManager.GetInt(PropertyKeys.WEAPON_POWER_MODIFICATION + "1");

        cannon.maxPower = 5;
        cannon.maxProjectileWeight = 4;
        cannon.maxShotCount = 4;

        weapons.Add(catapult);
        weapons.Add(cannon);

        extentions = new List<WeaponExtention>();
        
        var shotTrajectoryExtention = new WeaponExtention();
        shotTrajectoryExtention.Index = 0;
        shotTrajectoryExtention.Name = "Shot trajectory";
        shotTrajectoryExtention.Price = 15;

        extentions.Add(shotTrajectoryExtention);

        weaponExtentions = new Dictionary<int, List<WeaponExtention>>();
        weaponExtentions.Add(0, extentions);
        weaponExtentions.Add(1, extentions);

        GD.Print("Data loaded");
    }

    private static void InitData()
    {
        // init with default values
        StorageManager.StoreValue(PropertyKeys.WEAPON_POSSESION_LIST, "0"); // cannon
        StorageManager.StoreValue(PropertyKeys.WEAPON_POWER_MODIFICATION + "0", 0);
        StorageManager.StoreValue(PropertyKeys.WEAPON_SHOT_COUNT_MODIFICATION + "0", 0);
        StorageManager.StoreValue(PropertyKeys.WEAPON_WEIGHT_MODIFICATION + "0", 0);

        StorageManager.StoreValue(PropertyKeys.WEAPON_POWER_MODIFICATION + "1", 0);
        StorageManager.StoreValue(PropertyKeys.WEAPON_SHOT_COUNT_MODIFICATION + "1", 0);
        StorageManager.StoreValue(PropertyKeys.WEAPON_WEIGHT_MODIFICATION + "1", 0);

        StorageManager.StoreValue(PropertyKeys.AVAILABLE_EXTENTIONS + "0", "");
        StorageManager.StoreValue(PropertyKeys.AVAILABLE_EXTENTIONS + "1", "");
        
        StorageManager.Save();
    }

    public static void AddWeaponToPossesion(int weaponIndex)
    {
        weapons[weaponIndex].inPossesion = true;

        var possesion = StorageManager.GetString(PropertyKeys.WEAPON_POSSESION_LIST);
        StorageManager.StoreValue(PropertyKeys.WEAPON_POSSESION_LIST, possesion + ";" + weaponIndex);
    }
}