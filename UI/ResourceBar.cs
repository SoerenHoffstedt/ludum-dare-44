using Barely.Util;
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

        //Image scanningIcon;
        //Text scanningValue;

        bool soundOn = true;
        bool iconsOn = true;

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

            //scanningIcon = new Image(Assets.OtherSprites["scanningIcon"]);
            //scanningValue = new Text("1234").SetTextUpdateFunction(() => playerShip.GetStat(Stats.Scanning).ToString());


            Button menuButton = new Button(Assets.OtherSprites["menuIcon"]);
            menuButton.tooltipID = "mainMenu";
            menuButton.OnMouseClick = () => scene.LeaveToMenu();

            Button soundButton = new Button(Assets.OtherSprites["soundOn"]);
            soundButton.tooltipID = "toggleMusic";
            soundButton.OnMouseClick = () => {
                scene.ToggleMusic();
                soundOn = !soundOn;
                if (soundOn)
                    soundButton.sprite = Assets.OtherSprites["soundOn"];
                else
                    soundButton.sprite = Assets.OtherSprites["soundOff"];
            };

            Button iconButton = new Button(Assets.OtherSprites["iconToggleOn"]);
            iconButton.tooltipID = "toggleIcons";
            iconButton.OnMouseClick = () => {
                scene.ToggleIcons();
                iconsOn = !iconsOn;
                if (iconsOn)
                    iconButton.sprite = Assets.OtherSprites["iconToggleOn"];
                else
                    iconButton.sprite = Assets.OtherSprites["iconToggleOff"];
            };

            AddChild(new UIElement[] { menuButton, soundButton, iconButton, new Space(16), healthIcon, healthCount, fuelIcon, fuelCount, speedIcon, speedValue, damageIcon, damageValue, defenseIcon, defenseValue });

            
        }


    }
}
