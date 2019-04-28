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
        Action<Ship> OnEnemyDeath;
        RandomBattleInfo info;
        Ship enemyShip;
        Ship playerShip;
        GameScene scene;

        ShipBox playerBox;
        ShipBox enemyBox;

        Text actionText;
        Button attackButton;
        Button counterButton;
        Button fleeButton;

        public BattleScreen(GameScene scene, Canvas canvas) : base(Texts.Get("battleScreenTitle"), canvas)
        {
            this.scene = scene;
            playerShip = scene.GetPlayerShip();
            DeactivateCloseButton();

            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("randomBattle");
            Size = new Point(700, 700);


            VerticalLayout layout = new VerticalLayout();

            HorizontalLayout shipBoxes = new HorizontalLayout();
            Style.PushStyle("panelSprite");
            playerBox   = new ShipBox("playerShip");
            enemyBox    = new ShipBox("enemyShip");
            Style.PopStyle("panelSprite");
            shipBoxes.AddChild(playerBox, new Space(80), enemyBox);


            HorizontalLayout buttons = new HorizontalLayout();
            attackButton    = new Button("attack", new Point(200, 40));
            counterButton    = new Button("counter", new Point(200, 40));
            fleeButton      = new Button("flee", new Point(200, 40));
            attackButton.OnMouseClick = Attack;
            counterButton.OnMouseClick = Counter;
            fleeButton.OnMouseClick   = Flee;
            buttons.AddChild(attackButton, counterButton, fleeButton);

            actionText = new Text("enemyAction will  be coming, or your action, or something? more text please to widen it\nasdasdas\nasd\nasd\nasda\nasd");
            actionText.wrapText = true;

            layout.AddChild(shipBoxes, actionText, buttons);
            SetContentPanel(layout);
            OnClose += () => info = null;
            Close();

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("randomBattle");
        }


        public void OpenFor(Ship enemy, Action<Ship> OnEnemyDeath) {
            this.OnEnemyDeath = OnEnemyDeath;
            Open();

            SetTitleText($"Battle: Attacked by {enemy.Name}");

            damageTakenPlayer = 0;
            damageTakenEnemy = 0;

            enemyShip = enemy;

            actionText.SetText("battleStartInfo");
            UpdateTexts();
        }

        public void OpenFor(Tile target)
        {
            Open();

            SetTitleText(Texts.Get("battleScreenTitle") + $" on {target.Name}");

            damageTakenPlayer = 0;
            damageTakenEnemy = 0;

            info = (RandomBattleInfo)target.planetInfo;
            if(info.enemyBlueprint != null)
            {
                enemyShip = new Ship(info.enemyBlueprint, new Point(0, 0), null);
                actionText.SetText("battleStartInfo");
                UpdateTexts();
            } else
            {
                enemyBox.UpdateText(null);
                actionText.SetText("noBattle");
                attackButton.ChangeText("close");
                attackButton.OnMouseClick = Close;
                counterButton.Close();
                fleeButton.Close();
            }
        }
        
        private void UpdateTexts()
        {
            playerBox.UpdateText(playerShip);
            enemyBox.UpdateText(enemyShip);

            if (playerShip.GetStat(Stats.Health) == 0 || enemyShip.GetStat(Stats.Health) == 0)
                ShowEnd();                        
            else
                ShowOnGoing();                        
        }

        const int BASE_FUEL_GAIN = 2;
        const int OFFSET_FUEL_GAIN = 5;

        void ShowEnd()
        {
            if (enemyShip.GetStat(Stats.Health) == 0)
            {
                Sounds.Play("success");
                int fuelGained = (int)(BASE_FUEL_GAIN + new Random().NextDouble() * OFFSET_FUEL_GAIN);
                playerShip.ChangeStat(Stats.Fuel, fuelGained);
                actionText.SetText($"Nice, you won the battle. You gained {fuelGained} fuel units.");
                attackButton.ChangeText("close");
                attackButton.OnMouseClick = Close;
                if (OnEnemyDeath != null)
                    OnEnemyDeath(enemyShip);
                if(info != null)
                    info.enemyBlueprint = null;
                enemyShip = null;
            }
            else
            {
                Sounds.Play("gameOver");
                actionText.SetText("battleLost");
                attackButton.ChangeText("gameOver");
                attackButton.OnMouseClick = () => scene.GameOver(true);
            }
                                    
            counterButton.Close();
            fleeButton.Close();
        }

        void ShowOnGoing()
        {
            attackButton.ChangeText("attack");
            counterButton.ChangeText("counter");
            fleeButton.ChangeText("flee");
            attackButton.OnMouseClick = Attack;
            counterButton.OnMouseClick = Counter;
            fleeButton.OnMouseClick = Flee;
            attackButton.Open();
            counterButton.Open();
            fleeButton.Open();

            if(damageTakenPlayer != 0 || damageTakenEnemy != 0)
            {
                actionText.SetText($"You took {damageTakenPlayer} damage.\nThe enemy took {damageTakenEnemy} damage.");
            }
                
        }

        void Attack()
        {
            Attacking(playerShip, enemyShip);
            if(enemyShip.GetStat(Stats.Health) > 0)
                Attacking(enemyShip, playerShip);
            UpdateTexts();
        }

        const int COUNTER_BONUS = 2;

        void Counter()
        {
            Attacking(enemyShip, playerShip);
            if (playerShip.GetStat(Stats.Health) > 0)
                Attacking(playerShip, enemyShip, COUNTER_BONUS);
            UpdateTexts();
        }

        int damageTakenPlayer = 0;
        int damageTakenEnemy = 0;

        void Attacking(Ship attacker, Ship attacked, int attackBonus = 0)
        {
            int damage = attacker.GetStat(Stats.Damage) - attacked.GetStat(Stats.Defense) + attackBonus;
            attacked.ChangeStat(Stats.Health,-damage);
            Sounds.Play("attack");
            if (attacked.Faction == ShipFaction.Enemy)
                damageTakenEnemy = damage;
            else
                damageTakenPlayer = damage;
        }

        void Flee()
        {
            float threshhold = 0.25f + playerShip.GetStat(Stats.Speed) * 0.1f;
            bool success = new Random().NextDouble() >= threshhold;
            if (success)
            {
                actionText.SetText("fleeSuccessful");
                Sounds.Play("success");
                attackButton.ChangeText("close");
                attackButton.OnMouseClick = Close;
                counterButton.Close();
                fleeButton.Close();
            }
            else
            {
                Sounds.Play("fail");
                Attacking(enemyShip, playerShip);    
                actionText.SetText(Texts.Get("fleeFailed") + $" You took {damageTakenPlayer} damage.");
            }
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
            this.title.color = Color.CadetBlue;
            shipName    = new Text("this is a ships name");
            shipName.color = Color.IndianRed;
            healthText  = new KeyValueText("hp", "12");
            damageText  = new KeyValueText("damage", "12");
            defenseText = new KeyValueText("defense", "12");
            speedText   = new KeyValueText("speed", "12");
            AddChild(this.title, new Space(5), shipName, healthText, damageText, defenseText, speedText);
        }

        public void UpdateText(Ship ship) {
            if(ship == null)
            {
                Close();
            } else
            {
                Open();
                shipName.SetText(ship.Name);
                healthText.SetValue(ship.GetStat(Stats.Health).ToString());
                damageText.SetValue(ship.GetStat(Stats.Damage).ToString());
                defenseText.SetValue(ship.GetStat(Stats.Defense).ToString());
                speedText.SetValue(ship.GetStat(Stats.Speed).ToString());
            }
        }
    }
}
