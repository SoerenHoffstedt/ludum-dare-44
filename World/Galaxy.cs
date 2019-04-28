using Barely.Util;
using LD44.Actors;
using LD44.Scenes;
using LD44.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.World
{
    public class Galaxy
    {
        public static Point TileSize = new Point(16, 16);
        public static Point HalfTileSize { get { return new Point(TileSize.X / 2, TileSize.Y / 2); } }
        public static Point Size = new Point(48, 20);

        GameScene game;

        public Tile[,] tiles;
        public Tile homePlanet;
        Sprite highlightHomeSprite;
        Sprite iconShop;
        Sprite iconBattle;
        Sprite iconRandom;
        Sprite iconUnknown;

        public Galaxy(GameScene game)
        {
            this.game = game;

            GalaxyGenerator generator = new GalaxyGenerator();
            tiles = generator.Generate();
            homePlanet = generator.HomePlanet;

            highlightHomeSprite = Assets.OtherSprites["homeHighlight"];
            iconShop = Assets.OtherSprites["iconShop"];
            iconBattle = Assets.OtherSprites["iconBattle"];
            iconRandom = Assets.OtherSprites["iconRandom"];
            iconUnknown = Assets.OtherSprites["iconUnknown"];
        }        
        

        public void Render(SpriteBatch spriteBatch, bool drawIcons)
        {
            Point playerPos = game.GetPlayerShipPosition();
            int playerScanningRange = game.GetPlayerShipScanningRange() * Galaxy.TileSize.X;
            playerScanningRange *= playerScanningRange; //use squared distance.


            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Tile t = tiles[x, y];
                    if (t != null)
                    {
                        Point pos = new Point(x, y) * TileSize;
                        
                        if (t.Sprite != null)
                            t.Sprite.Render(spriteBatch, pos);

                        if (drawIcons)
                        {
                            float dist = (pos - playerPos).ToVector2().LengthSquared();
                            Point p2 = pos - new Point(0, 16);

                            if (dist <= playerScanningRange)
                            {
                                if (t.Type == PlanetType.RandomEvent)
                                    iconRandom.Render(spriteBatch, p2);
                                else if (t.Type == PlanetType.EnemyBase)
                                    iconBattle.Render(spriteBatch, p2);
                                else if (t.Type == PlanetType.Shop)
                                    iconShop.Render(spriteBatch, p2);
                            }
                            else if(t.Type != PlanetType.Empty && t.Type != PlanetType.Home)
                                iconUnknown.Render(spriteBatch, p2);
                            
                        }                        

                        if (t == homePlanet)
                        {
                            highlightHomeSprite.Render(spriteBatch, pos - new Point(0, 10));
                        }

                    }
                }
            }
        }

        #region Helper

        public Tile Tile(Point p)
        {
            return tiles[p.X, p.Y];
        }

        public IEnumerable<Point> IterateNeighboursFourDir(int x, int y)
        {
            if (IsInRange(x - 1, y))
                yield return new Point(x - 1, y);
            if (IsInRange(x + 1, y))
                yield return new Point(x + 1, y);
            if (IsInRange(x, y - 1))
                yield return new Point(x, y - 1);
            if (IsInRange(x, y + 1))
                yield return new Point(x, y + 1);
        }

        public bool IsInRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Size.X && y < Size.Y;
        }

        public bool IsInRange(Point p)
        {
            return IsInRange(p.X, p.Y);
        }

        public Tile this[int x, int y]
        {
            get { return tiles[x, y]; }
            set { tiles[x, y] = value; }
        }

        public Tile this[Point p]
        {
            get { return tiles[p.X, p.Y]; }
            set { tiles[p.X, p.Y] = value; }
        }

        #endregion

    }
}
