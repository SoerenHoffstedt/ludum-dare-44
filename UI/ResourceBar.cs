﻿using Barely.Util;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Actors;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;


namespace LD44.UI
{
    public class ResourceBar : HorizontalLayout
    {
        Image healthIcon;
        Text healthCount;

        Image fuelIcon;
        Text fuelCount;

        Image speedIcon;
        Text speedValue;

        Image damageIcon;
        Text damageValue;

        Image defenseIcon;
        Text defenseValue;

        Image scanningIcon;
        Text scanningValue;

        public Button soundButton;
        public Button iconButton;

        public ResourceBar(GameScene scene) : base()
        {
            var playerShip = scene.GetPlayerShip();

            healthIcon = new Image(Assets.OtherSprites["healthIcon"]);
            healthCount = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Health).ToString());

            fuelIcon = new Image(Assets.OtherSprites["fuelIcon"]);
            fuelCount = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Fuel).ToString());

            speedIcon = new Image(Assets.OtherSprites["speedIcon"]);
            speedValue = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Speed).ToString());

            damageIcon = new Image(Assets.OtherSprites["damageIcon"]);
            damageValue = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Damage).ToString());

            defenseIcon = new Image(Assets.OtherSprites["defenseIcon"]);
            defenseValue = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Defense).ToString());

            scanningIcon = new Image(Assets.OtherSprites["scanningIcon"]);
            scanningValue = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Scanning).ToString());


            Button menuButton = new Button(Assets.OtherSprites["menuIcon"]);            
            menuButton.OnMouseClick = () => scene.LeaveToMenu();

            Button helpButton = new Button(Assets.OtherSprites["helpIcon"]);
            helpButton.OnMouseClick = scene.ShowHelp;

            soundButton = new Button(Assets.OtherSprites["soundOn"]);            
            soundButton.OnMouseClick = () => {
                scene.ToggleMusic();                
            };

            iconButton = new Button(Assets.OtherSprites["iconToggleOn"]);            
            iconButton.OnMouseClick = () => {
                scene.ToggleIcons();                
            };

            Button giveUp = new Button("giveUp");
            giveUp.OnMouseClick = () => scene.GameOver(false);

            AddChild(new UIElement[] { menuButton, helpButton, soundButton, iconButton, new Space(16), healthIcon, healthCount, fuelIcon, fuelCount, speedIcon, speedValue, damageIcon, damageValue, defenseIcon, defenseValue, scanningIcon, scanningValue, giveUp });

            
        }


    }
}
