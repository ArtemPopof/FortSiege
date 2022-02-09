using Godot;
using System;

public class MainMenuHeader : Panel
{
    [Signal]
    public delegate void ChangeScreen(String targetScreen);
    [Signal]
    public delegate void ChangeWeapon(int index);

    public override void _Ready()
    {
        var weapons = Data.weapons;

        var weaponNodes = GetNode<Container>("HBoxContainer/AvailableWeapons").GetChildren();
        for (int i = 0; i < weaponNodes.Count; i++)
        {
            UpdateWeaponState(weaponNodes[i] as Node, weapons[i]);
        }
    }

    private void UpdateWeaponState(Node node, WeaponInfo info)
    {
        var buyMaskButton = node.GetChildren()[1] as Panel;
        buyMaskButton.Visible = !info.inPossesion;
    }

    public void GoToShop()
    {
        GD.Print("[MainMenuHeader] Go to weapon shop");

        EmitSignal("ChangeScreen", Constants.WEAPON_SHOP_SCREEN);
    }

    public void ChangeWeaponButtonPressed(int index)
    {
        GD.Print("[MainMenuHeader] Changing current weapon to " + Data.weapons[index].name);

        EmitSignal("ChangeWeapon", index);
    }
}
