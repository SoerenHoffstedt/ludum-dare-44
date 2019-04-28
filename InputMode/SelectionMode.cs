using Barely.Util;
using LD44.Actors;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.InputModes
{
    public class SelectionMode : InputMode
    {
        Tile lastTile = null;

        public SelectionMode(GameScene scene) : base(scene)
        {
        }

        public override InputPreviewData GetInputPreviewData()
        {
            return null;
        }

        public override void Select()
        {
            
        }

        public override void Unselect()
        {
            
        }

        public override void Update(double dt)
        {
            Tile t = scene.GetMouseOverTile();

            if(scene.state != GameScene.State.EnemyFly)
            {
                if (t != null && t.Type != PlanetType.Empty && lastTile != t)
                {
                    Sounds.Play("click");
                }

                if (Input.GetLeftMouseUp())
                {
                    scene.MoveShipTo(t);
                }
            }            
            lastTile = t;
        }
    }
}
