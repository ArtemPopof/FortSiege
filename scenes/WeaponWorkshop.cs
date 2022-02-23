using Godot;
using System;

public class WeaponWorkshop : Node2D
{

    private Swiper swiper;
    
    public override void _Ready()
    {
        swiper = GetNode<Swiper>("WeaponSwiper");
        swiper.Connect("Swiped", this, "IndexChanged");

        var currentWeapon = StorageManager.GetInt(PropertyKeys.CURRENT_WEAPON);
        var currentWeaponInfo = Data.weapons[currentWeapon];

        swiper.SwipeTo(currentWeapon);

        UpdateCurrentWeaponInfo(currentWeaponInfo);
    }

    private void IndexChanged(int index)
    {
        UpdateCurrentWeaponInfo(Data.weapons[index]);
    }

    private void UpdateCurrentWeaponInfo(WeaponInfo weaponInfo)
    {
        var titleLabel = GetNode<Label>("ModificationPanel/VBoxContainer/WeaponName");
        var shotCounter = GetNode<ShotCounter>("ModificationPanel/VBoxContainer/ShotCount/ShotCounter");
        var weight = GetNode<HorizontalLevel>("ModificationPanel/VBoxContainer/BallWeight/HorizontalLevel");
        var power = GetNode<HorizontalLevel>("ModificationPanel/VBoxContainer/PowerLevel/HorizontalLevel2");
        var rating = GetNode<Label>("ModificationPanel/VBoxContainer/Rating/Label");
        var afterBuyPanel = GetNode<HBoxContainer>("ModificationPanel/BuyPanel/AfterBuy");
        var beforeBuyPanel = GetNode<HBoxContainer>("ModificationPanel/BuyPanel/BeforeBuy");

        titleLabel.Text = weaponInfo.name;
        shotCounter.Init(weaponInfo);
        weight.UpdateLevel(Math.Max(1, (int) (weaponInfo.projectileWeight / 2)));
        power.UpdateLevel((int) weaponInfo.power);

        var ratingValue = (weaponInfo.power * 4) + weaponInfo.shotCount + (weaponInfo.projectileWeight);
        rating.Text = "Rating: " + ((ratingValue / 6.0).ToString());

        if (weaponInfo.inPossesion)
        {
            afterBuyPanel.Visible = true;
            beforeBuyPanel.Visible = false;

            GD.Print("[WeaponWorkshop] Current weapon: " + StorageManager.GetInt(PropertyKeys.CURRENT_WEAPON));
            var text = GetNode<Label>("ModificationPanel/BuyPanel/AfterBuy/Label2");
            text.Text = StorageManager.GetInt(PropertyKeys.CURRENT_WEAPON) == weaponInfo.index ? "Current" : "Select";
        }
        else
        {
            afterBuyPanel.Visible = false;
            beforeBuyPanel.Visible = true;
        }
    }

    private void SelectCurrentWeapon()
    {
        StorageManager.StoreValue(PropertyKeys.CURRENT_WEAPON, swiper.CurrentIndex);
        StorageManager.Save();

        var text = GetNode<Label>("ModificationPanel/BuyPanel/AfterBuy/Label2");
        text.Text = "Current";
    }
}
