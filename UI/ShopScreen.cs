using Barely.Util;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Actors;
using LD44.Scenes;
using LD44.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.UI
{
    public class ShopScreen : ModalWindow
    {
        Ship playerShip;
        ShopInfo info;
        KeyValueText fuelToBuy;
        Text buyWith;
        Dictionary<Stats, Button> buttons = new Dictionary<Stats, Button>();
        Stats[] s = { Stats.Damage, Stats.Defense, Stats.Speed };

        public ShopScreen(GameScene scene, Canvas canvas) : base(Texts.Get("shopScreenTitle"), canvas)
        {          
            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("planetScreenContent");            
            
            playerShip = scene.GetPlayerShip();

            VerticalLayout layout = new VerticalLayout();

            fuelToBuy = new KeyValueText("fuelToBuy", "1234");
            buyWith = new Text("buyWith");

            HorizontalLayout buttonLayout = new HorizontalLayout();
            
            foreach(Stats stat in s)
            {
                Button b = new Button(stat.ToString());
                Stats innerCopy = stat;
                b.OnMouseClick = () => HandleClick(innerCopy);
                buttonLayout.AddChild(b);
                buttons.Add(stat, b);
            }


            layout.AddChild(fuelToBuy, new Space(10), buyWith, buttonLayout);

            SetContentPanel(layout);
            OnClose += () => info = null;
            Close();

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("planetScreenContent");
        }

        public void OpenFor(Tile planet)
        {
            Sounds.Play("openScreen");
            Open();
            info = (ShopInfo)planet.planetInfo;
            UpdateTexts();
            SetTitleText($"{Texts.Get("shopScreenTitle")} on {planet.Name}");            
        }        

        private void UpdateTexts()
        {            
            fuelToBuy.SetValue($"{info.FuelToBuy}");

            foreach(Stats stat in s)
            {
                Button b = buttons[stat];
                bool playerHasEnough = playerShip.GetStat(stat) > 0 && info.FuelToBuy > 0;
                if (playerHasEnough)
                {
                    b.Interactable = true;
                    b.ChangeColor(Button.ButtonColors.Normal);
                }
                else
                {
                    b.Interactable = false;
                    b.ChangeColor(Button.ButtonColors.Inactive);
                }
            }

        }        

        private void HandleClick(Stats stat)
        {
            if(playerShip.GetStat(stat) > 0)
            {
                playerShip.ChangeStat(Stats.Fuel, info.FuelToBuy);
                playerShip.ChangeStat(stat, -1);
                info.FuelToBuy = 0;
            } else
            {
                Sounds.Play("error");
            }
            UpdateTexts();
        }

    }
}
