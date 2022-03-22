using Godot;
using System;

public class WeaponWorkshop : Node2D
{

    private Swiper swiper;

    private WeaponInfo currentInfo;
    
    public override void _Ready()
    {
        swiper = GetNode<Swiper>("WeaponSwiper");
        swiper.Connect("Swiped", this, "IndexChanged");

        var currentWeapon = StorageManager.GetInt(PropertyKeys.CURRENT_WEAPON);
        currentInfo = Data.weapons[currentWeapon];

        swiper.SwipeTo(currentWeapon);

        UpdateCurrentWeaponInfo(currentInfo);
    }

    private void IndexChanged(int index)
    {
        currentInfo = Data.weapons[index];
        UpdateCurrentWeaponInfo(currentInfo);
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
        rating.Text = "Rating: " + ((ratingValue / 6.0).ToString("0.0"));

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

            var text = GetNode<Label>("ModificationPanel/BuyPanel/BeforeBuy/Label2");
            text.Text = weaponInfo.cost.ToString();
        }

        UpdateUpgradeButtons(weaponInfo);
        UpdateAvailableExtentions(weaponInfo);
    }

    private void UpdateUpgradeButtons(WeaponInfo weaponInfo)
    {
        var upgradeWeightButton = GetNode<Panel>("ModificationPanel/UpgradePanel2");
        var upgradePowerButton = GetNode<Panel>("ModificationPanel/UpgradePanel3");
        var upgradeShotCountButton = GetNode<Panel>("ModificationPanel/UpgradePanel");

        upgradePowerButton.GetNode<Label>("HBoxContainer/Label2").Text = weaponInfo.GetWeaponUpgradeCost(weaponInfo.power).ToString();
        upgradeWeightButton.GetNode<Label>("HBoxContainer/Label2").Text = weaponInfo.GetWeaponUpgradeCost(weaponInfo.projectileWeight).ToString();
        upgradeShotCountButton.GetNode<Label>("HBoxContainer/Label2").Text = weaponInfo.GetWeaponUpgradeCost(weaponInfo.shotCount).ToString();

        upgradePowerButton.Visible = weaponInfo.maxPower != weaponInfo.power;
        upgradeShotCountButton.Visible = weaponInfo.maxShotCount != weaponInfo.shotCount;
        upgradeWeightButton.Visible = weaponInfo.maxProjectileWeight != weaponInfo.projectileWeight;
    }

    private void UpdateAvailableExtentions(WeaponInfo info)
    {
        var buyButton = GetNode<Panel>("Extension1/Extension/Panel/BuyPanel");
        var currentWeaponExtentions = StorageManager.GetString(PropertyKeys.AVAILABLE_EXTENTIONS + info.index);
        buyButton.Visible = currentWeaponExtentions == "";

        buyButton.GetNode<Label>("HBoxContainer/Label2").Text = Data.weaponExtentions[info.index][0].Price.ToString();
    }

    public void OnExtentionBuyClicked(int index)
    {
        var currentWeaponExtentions = Data.weaponExtentions[currentInfo.index];
        var extention = currentWeaponExtentions[index];

        if (!CheckCanBuy(extention.Price)) return;

        UIManager.ShowDialogForResult(UIManager.YES_NO_DIALOG, (result) => {
            if (!result)
            {
                return;
            }

            StorageManager.StoreValue(PropertyKeys.COIN_COUNT, StorageManager.GetInt(PropertyKeys.COIN_COUNT) - extention.Price);
            var currentExtentionsAvailable = StorageManager.GetString(PropertyKeys.AVAILABLE_EXTENTIONS + currentInfo.index);
            StorageManager.StoreValue(PropertyKeys.AVAILABLE_EXTENTIONS + currentInfo.index, currentExtentionsAvailable + index.ToString() + ";");
            StorageManager.Save();

            UpdateCurrentWeaponInfo(currentInfo);
        },  $"Do you really want to buy \n extention ({extention.Name}) \n for {extention.Price} coins?");
    }

    public void BuyButtonPressed()
    {
        var weapon = Data.weapons[swiper.CurrentIndex];
        GD.Print("Try to buy weapon " + weapon.name);

        if (!CheckCanBuy(weapon.cost)) return;

        UIManager.ShowDialogForResult(UIManager.YES_NO_DIALOG, (result) => {
            if (!result)
            {
                return;
            }

            Data.AddWeaponToPossesion(swiper.CurrentIndex);
            StorageManager.StoreValue(PropertyKeys.COIN_COUNT, StorageManager.GetInt(PropertyKeys.COIN_COUNT) - weapon.cost);
            StorageManager.StoreValue(PropertyKeys.CURRENT_WEAPON, swiper.CurrentIndex);
            StorageManager.Save();

            UpdateCurrentWeaponInfo(weapon);
        },  $"Do you really want to buy {weapon.name} \n for {weapon.cost} coins?");
    }

    private bool CheckCanBuy(int price)
    {
        var currentCoinCount = StorageManager.GetInt(PropertyKeys.COIN_COUNT);
        if (currentCoinCount < price)
        {
            GD.Print("Not enough money to buy ");
            UIManager.instance.ShowDialog(UIManager.NOT_ENOUGH_MONEY_TO_BUY_DIALOG, -1, -1, price - currentCoinCount);
            return false;
        }

        return true;
    }

    private void SelectCurrentWeapon()
    {
        StorageManager.StoreValue(PropertyKeys.CURRENT_WEAPON, swiper.CurrentIndex);
        StorageManager.Save();

        var text = GetNode<Label>("ModificationPanel/BuyPanel/AfterBuy/Label2");
        text.Text = "Current";
    }

    private void UpgradePower()
    {
        GD.Print("Trying to upgrade power");

        UpgradeAttribute(PropertyKeys.WEAPON_POWER_MODIFICATION, Data.weapons[swiper.CurrentIndex].power);
    }

    private void UpgradeWeight()
    {
        GD.Print("Trying to upgrade weight");
        UpgradeAttribute(PropertyKeys.WEAPON_WEIGHT_MODIFICATION, Data.weapons[swiper.CurrentIndex].projectileWeight);
    }

    private void UpgradeShotCount()
    {
        GD.Print("Trying to upgrade shot count");
        UpgradeAttribute(PropertyKeys.WEAPON_SHOT_COUNT_MODIFICATION, Data.weapons[swiper.CurrentIndex].shotCount);
    }

    private void UpgradeAttribute(string key, float value)
    {
        var upgradeCost = currentInfo.GetWeaponUpgradeCost(value);
        if (!CheckCanBuy(upgradeCost)) return;

        UIManager.ShowDialogForResult(UIManager.YES_NO_DIALOG, (result) => {
            if (!result)
            {
                return;
            }

            if (key == PropertyKeys.WEAPON_POWER_MODIFICATION)
            {
                currentInfo.power++;
            }
            if (key == PropertyKeys.WEAPON_SHOT_COUNT_MODIFICATION)
            {
                currentInfo.shotCount++;
            }
            if (key == PropertyKeys.WEAPON_WEIGHT_MODIFICATION)
            {
                currentInfo.projectileWeight++;
            }

            var currentUpgradeLevel = StorageManager.GetInt(key + currentInfo.index.ToString());
            StorageManager.StoreValue(key + currentInfo.index.ToString(), currentUpgradeLevel + 1);
            StorageManager.StoreValue(PropertyKeys.COIN_COUNT, StorageManager.GetInt(PropertyKeys.COIN_COUNT) - upgradeCost);
            StorageManager.Save();

            UpdateCurrentWeaponInfo(currentInfo);

            GD.Print($"Upgraded {key} successfully");
        },  $"Do you really want to upgrade \n for {upgradeCost} coins?");

    }
}
