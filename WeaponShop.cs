using Godot;
using System;
using System.Collections.Generic;

public class WeaponShop : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    public delegate void ScreenDone();

    private Swiper swiper;
    private WeaponInfoPanel infoPanel;

    private int weaponIndex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var hasSomeStartIndexFromCaller = StorageManager.gameProperties.ContainsKey(PropertyKeys.SCENE_ARG_1);
        weaponIndex = hasSomeStartIndexFromCaller ? (int) StorageManager.gameProperties[PropertyKeys.SCENE_ARG_1] : 0;

        swiper = GetNode<Swiper>("VBoxContainer/HBoxContainer/SwiperPanel/VBoxContainer/Swiper");
        infoPanel = GetNode<WeaponInfoPanel>("VBoxContainer/HBoxContainer/InfoPanel");

        swiper.Connect("Swiped", this, "WeaponIndexChanged");

        infoPanel.SetInfo(Data.weapons[0]);
        
        swiper.SwipeTo(weaponIndex);
    }

    public void BuyButtonPressed()
    {
        var weapon = Data.weapons[weaponIndex];
        GD.Print("Try to buy weapon " + weapon.name);

        var currentCoinCount = StorageManager.GetInt(PropertyKeys.COIN_COUNT);
        if (currentCoinCount < weapon.cost)
        {
            GD.Print("Not enough money to buy weapon");
            UIManager.instance.ShowDialog(UIManager.NOT_ENOUGH_MONEY_TO_BUY_DIALOG, -1, -1, weapon.cost - currentCoinCount);
            return;
        }

        // can buy this weapon
        Data.AddWeaponToPossesion(weaponIndex);
        StorageManager.StoreValue(PropertyKeys.COIN_COUNT, currentCoinCount - weapon.cost);
        StorageManager.Save();

        WeaponIndexChanged(weaponIndex);
    }

    public void StartGameButtonPressed()
    {
        StorageManager.StoreValue(PropertyKeys.CURRENT_WEAPON, weaponIndex);

        Visible = false;
        EmitSignal("ScreenDone");   

        swiper.Visible = false;
    }

    public void WeaponIndexChanged(int index)
    {
        GD.Print("Weapon index changed to " + index);
        if (index == -1 || index > Data.weapons.Count) return;
        weaponIndex = index;

        infoPanel.SetInfo(Data.weapons[weaponIndex]);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
