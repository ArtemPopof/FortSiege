using System.Collections.Generic;
using Godot;

public class Data
{
    public const int WEAPON_CATAPULT_ID = 0;
    public const int WEAPON_CANNON_ID = 1;

    public static List<WeaponInfo> weapons;

    public static void Init()
    {
        GD.Print("Init data manager");

        var possesion = StorageManager.GetString(PropertyKeys.WEAPON_POSSESION_LIST);
        if (possesion == null)
        {
            // init with default values
            possesion = "0";
            StorageManager.StoreValue(PropertyKeys.WEAPON_POSSESION_LIST, possesion);
        }

        weapons = new List<WeaponInfo>(2);

        var possesionArray = ToPossesionArray(possesion, weapons.Capacity);

        var catapult = new WeaponInfo();
        catapult.cost = 0;
        catapult.name = "Catapult";
        catapult.controlTrajectory = false;
        catapult.inPossesion = possesionArray[WEAPON_CATAPULT_ID];
        catapult.shotCount = 4;
        catapult.projectileTexture = ResourceLoader.Load("res://textures/CatapultProjectile.svg") as Texture;
        catapult.projectileWeight = 7;

        var cannon = new WeaponInfo();
        cannon.cost = 50;
        cannon.name = "Cannon";
        cannon.shotCount = 3;
        cannon.controlTrajectory = true;
        cannon.inPossesion = possesionArray[WEAPON_CANNON_ID];
        cannon.projectileTexture = ResourceLoader.Load("res://textures/CannonProjectile.png") as Texture;
        cannon.projectileWeight = 5;

        weapons.Add(catapult);
        weapons.Add(cannon);

        GD.Print("weapons loaded");
    }

    private static bool[] ToPossesionArray(string dataString, int weaponCount)
    {
        var strings = dataString.Split(';');

        var array = new bool[weaponCount];
        
        for (var i = 0; i < strings.Length; i++)
        {
            array[int.Parse(strings[i])] = true;
        }

        return array;
    }

    public static void AddToPossesion(int weaponIndex)
    {
        weapons[weaponIndex].inPossesion = true;

        var possesion = StorageManager.GetString(PropertyKeys.WEAPON_POSSESION_LIST);
        StorageManager.StoreValue(PropertyKeys.WEAPON_POSSESION_LIST, possesion + ";" + weaponIndex);
    }
}