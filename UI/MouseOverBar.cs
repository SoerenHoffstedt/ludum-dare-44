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
    public class MouseOverBar : VerticalLayout
    {
        GameScene gameScene;

        Tile lastTile = null;
        Text name;
        KeyValueText type;
        KeyValueText fuelCost;


        public MouseOverBar(GameScene gameScene) : base()
        {
            this.gameScene = gameScene;

            name = new Text("whatanamewhata", false);
            name.SetFont(Style.fonts["textLarge"]);
            name.SetColor(Color.CadetBlue);

            type = new KeyValueText("type", "whata type");
            fuelCost = new KeyValueText("travelCost", "1234");

            AddChild(name, type, fuelCost);
            SetTexts(null);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);        

            Tile t = gameScene.GetMouseOverTile();
            SetTexts(t);
            /*if (t != lastTile)
            {                                
                SetTexts(t);
            } else if(t != null && t == lastTile)
            {
                fuelCost.SetValue($"{gameScene.GetFuelCost(t.Coord)}");
            }*/
            lastTile = t;
        }        

        private void SetTexts(Tile t)
        {
            if (t == null)
                CloseAllChildren();
            else if (t.Type != PlanetType.Empty)
            {
                OpenAllChildren();
                if(gameScene.IsInPlayerScanningRange(t.Coord * Galaxy.TileSize) || t.Type == PlanetType.Home)
                {
                    name.SetText(t.Name);
                    type.SetValue(t.Type.ToString());
                    fuelCost.SetValue($"{gameScene.GetFuelCost(t.Coord)}");
                } else
                {
                    name.SetText("???");
                    type.SetValue("???");
                    fuelCost.SetValue($"{gameScene.GetFuelCost(t.Coord)}");
                }

            } else
            {
                name.Close();
                type.Close();
                fuelCost.Open();
                fuelCost.SetValue($"{gameScene.GetFuelCost(t.Coord)}");
            }
            
        }

        private void OpenAllChildren()
        {
            foreach(UIElement e in childElements)
            {
                e.Open();
            }
        }

        private void CloseAllChildren()
        {
            foreach (UIElement e in childElements)
            {
                e.Close();
            }
        }

    }
}
