using Barely.Util;
using LD44.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.InputModes
{

    public enum InputModeType
    {
        Selection,        
        Count
    }

    public abstract class InputMode
    {
        protected Galaxy map;        

        public InputMode(Galaxy map)
        {
            this.map = map;            
        }

        /// <summary>
        /// Method called on selecting this input mode.
        /// </summary>
        public abstract void Select();        

        /// <summary>
        /// Method called on unselecting this input mode.
        /// </summary>
        public abstract void Unselect();

        /// <summary>
        /// Update function called once per frame to be used to update input state.
        /// </summary>
        /// <param name="dt"></param>
        public abstract void Update(double dt);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract InputPreviewData GetInputPreviewData();

    }


    public class InputPreviewData
    {
        public HashSet<Point> PreviewPoints;
        public Func<Point, Sprite> TileToSprite;
        public Func<Point, Color> TileToColor;
        public Func<Point, Sprite, Tile, Point> OffsetDrawPosition;

        public InputPreviewData(HashSet<Point> PreviewPoints, Func<Point, Sprite> TileToSprite, Func<Point, Color> TileToColor, Func<Point, Sprite, Tile, Point> OffsetDrawPosition)
        {
            this.PreviewPoints      = PreviewPoints;
            this.TileToSprite       = TileToSprite;
            this.TileToColor        = TileToColor;
            this.OffsetDrawPosition = OffsetDrawPosition;
        }

    }



}
