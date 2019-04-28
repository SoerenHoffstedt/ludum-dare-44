using Barely.Util;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.UI
{
    public class EndScreen : ModalWindow
    {
        Text mainText;
        KeyValueText planetsCount;
        KeyValueText shopCount;
        KeyValueText battleCount;
        KeyValueText randomEventCount;

        KeyValueText partsSold;
        KeyValueText fuelBurned;
        KeyValueText distance;

        public EndScreen(GameScene scene, Canvas canvas) : base("endScreen", canvas)
        {
            DeactivateCloseButton();
            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("endScreen");
            VerticalLayout layout = new VerticalLayout();
            Size = new Point(600, 600);

            mainText = new Text("asdasdadasdasdasdadasdasdasdadasdasdaasd\nasdasd\n\nasd\nasd\n");
            mainText.wrapText = true;

            planetsCount    = new KeyValueText("planetCount", "1234");
            shopCount       = new KeyValueText("shopCount", "1234");
            battleCount     = new KeyValueText("battleCount", "1234");
            randomEventCount= new KeyValueText("randomEventCount", "1234");

            partsSold       = new KeyValueText("partsSold", "1234");
            fuelBurned       = new KeyValueText("fuelBurned", "1234");
            distance        = new KeyValueText("distanceTraveled", "1234");

            Button close = new Button("backToMenu");
            close.OnMouseClick = scene.LeaveToMenu;

            Button tryAgain = new Button("tryAgain");
            tryAgain.OnMouseClick = scene.NewGame;

            layout.AddChild(mainText, new Space(10), planetsCount, shopCount, battleCount, randomEventCount, new Space(10), partsSold, fuelBurned, distance, new Space(10), close, tryAgain);

            SetContentPanel(layout);

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("endScreen");

            Close();
        }

        public void Open(GameEndInfo info)
        {
            Open();

            if (info.success) {
                SetTitleText("endSuccessTitle");
                mainText.SetText("endSuccessText");
                Sounds.Play("gameWon");
            }
            else
            {
                SetTitleText("endFailTitle");
                if(info.lostBattle)
                    mainText.SetText("endFailTextBattle");
                else if(info.noFuel)
                    mainText.SetText("endFailTextFuel");
                else
                    mainText.SetText("endFailText");
                Sounds.Play("gameLost");
            }

            var stats = info.gameStats;
            planetsCount.SetValue(stats.PlanetsVisited.ToString());
            shopCount.SetValue(stats.ShopsVisited.ToString());
            battleCount.SetValue(stats.BattlesFought.ToString());
            randomEventCount.SetValue(stats.RandomEvents.ToString());

            partsSold.SetValue(stats.PartsSold.ToString());
            fuelBurned.SetValue(stats.FuelBurned.ToString());
            distance.SetValue(stats.DistanceTraveled.ToString());
        }
        

    }

    public class GameEndInfo
    {
        public bool success;
        public bool lostBattle;
        public bool noFuel;
        public GameStats gameStats;
    }
}
