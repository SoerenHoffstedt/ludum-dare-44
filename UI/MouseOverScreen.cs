using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarelyUI;
using BarelyUI.Styles;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;

namespace LD44.UI
{
    public class MouseOverScreen : VerticalLayout
    {
        GameScene gameScene;

        Tile lastTile = null;
        Text name;
        KeyValueText type;
        KeyValueText fuelCost;


        public MouseOverScreen(GameScene gameScene) : base()
        {
            this.gameScene = gameScene;

            name = new Text("whatanamewhata", false);
            name.SetFont(Style.fonts["textLarge"]);
            name.SetColor(Color.CadetBlue);

            type = new KeyValueText("type", "whata type");
            fuelCost = new KeyValueText("travelCost", "1234");

            AddChild(name, type, fuelCost);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);        

            Tile t = gameScene.GetMouseOverTile();        

            if (t == null || t.Type == PlanetType.Empty)
            {                
                CloseAllChildren();                
            }
            else if(t != lastTile)
            {                
                OpenAllChildren();
                SetTexts(t);
            } else if(t == lastTile)
            {
                fuelCost.SetValue($"{gameScene.GetFuelCost(t.Coord)}");
            }
            lastTile = t;
        }        

        private void SetTexts(Tile t)
        {
            name.SetText(t.Name);
            type.SetValue(t.Type.ToString());
            fuelCost.SetValue($"{gameScene.GetFuelCost(t.Coord)}");
        }

        private void OpenAllChildren()
        {
            isOpen = true;
            /*foreach(UIElement e in childElements)
            {
                e.Open();
            }*/
        }

        private void CloseAllChildren()
        {
            isOpen = false;
            /*foreach (UIElement e in childElements)
            {
                e.Close();
            }*/
        }

    }
}
