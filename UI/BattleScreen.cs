using Barely.Util;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Actors;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.UI
{
    public class BattleScreen : ModalWindow
    {
        RandomBattleInfo info;
        GameScene scene;

        ShipBox playerBox;
        ShipBox enemyBox;

        Text enemyActionText;
        Button attackButton;
        Button defendButton;
        Button fleeButton;

        public BattleScreen(GameScene scene, Canvas canvas) : base(Texts.Get("battleScreenTitle"), canvas)
        {
            this.scene = scene;
            DeactivateCloseButton();

            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("randomEvent");
            Size = new Point(500, 500);


            VerticalLayout layout = new VerticalLayout();



            HorizontalLayout shipBoxes = new HorizontalLayout();
            playerBox   = new ShipBox("playerShip");
            enemyBox    = new ShipBox("enemyShip");
            shipBoxes.AddChild(playerBox, enemyBox);


            HorizontalLayout buttons = new HorizontalLayout();
            attackButton    = new Button("attack");
            defendButton    = new Button("defend");
            fleeButton      = new Button("flee");
            buttons.AddChild(attackButton, defendButton, fleeButton);

            layout.AddChild(shipBoxes, enemyActionText, buttons);
            SetContentPanel(layout);
            OnClose += () => info = null;
            Close();

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("randomEvent");
        }


        public void OpenFor(Tile target)
        {
            info = (RandomBattleInfo)target.planetInfo;
            UpdateTexts();
        }
        
        private void UpdateTexts()
        {

        }

    }

    class ShipBox : VerticalLayout
    {
        Text title;
        Text shipName;
        KeyValueText healthText;
        KeyValueText damageText;
        KeyValueText defenseText;
        KeyValueText speedText;


        public ShipBox(string title)
        {
            this.title = new Text(title);
            shipName    = new Text("this is a ships name");
            healthText  = new KeyValueText("hp", "12");
            damageText  = new KeyValueText("damage", "12");
            defenseText = new KeyValueText("defense", "12");
            speedText   = new KeyValueText("speed", "12");
            AddChild(this.title, shipName, damageText, defenseText, speedText);

        }

        void UpdateText(Ship ship) {
            shipName.SetText(ship.name);
            healthText.SetValue(ship.GetStat(Stats.Health).ToString());
            damageText.SetValue(ship.GetStat(Stats.Damage).ToString());
            defenseText.SetValue(ship.GetStat(Stats.Defense).ToString());
            speedText.SetValue(ship.GetStat(Stats.Speed).ToString());
        }
    }
}
