using Barely.Util;
using LD44.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        public static Point Size = new Point(32, 16);

        GameScene game;

        public Tile[,] tiles;        


        public Galaxy(GameScene game)
        {
            this.game = game;
            Generate();
        }

        private void Generate()
        {
            Random random = new Random();

            const int PLANET_COUNT = 20;

            tiles = new Tile[Size.X, Size.Y];
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Sprite sprite = null;
                    if (random.NextDouble() > 0.8f)
                        sprite = Assets.GetRandomBackgroundSprite(random);
                    tiles[x, y] = new Tile(PlanetType.Empty, new Point(x, y), sprite, null);
                }
            }


            Point mostFarPlanet = new Point(-1, -1);
            for(int i = 0; i < PLANET_COUNT; ++i)
            {
                //TODO: better generation to distribute planets. Place random and push apart?
                Point p = Point.Zero;
                do
                {
                    p.X = random.Next(Size.X);
                    p.Y = random.Next(Size.Y);
                } while (Tile(p).Type != PlanetType.Empty);

                if (p.X > mostFarPlanet.X)
                    mostFarPlanet = p;
                else if(p.X == mostFarPlanet.X)
                {
                    if (Math.Abs(p.Y - Size.Y / 2) < Math.Abs(mostFarPlanet.Y - Size.Y / 2))
                        mostFarPlanet = p;
                }

                tiles[p.X, p.Y] = new Tile(PlanetType.RandomEvent, p, Assets.GetRandomPlanetSprite(random), $"Planet {i + 1}");
            }

            Tile home = Tile(mostFarPlanet);
            home.Sprite = Assets.OtherSprites["homePlanet"];
            home.Name = "Home";
            home.Type = PlanetType.Home;
             
        }
                

        public void Update(float dt)
        {

        }
      
        public void Render(SpriteBatch spriteBatch)
        {
            Sprite coveredSprite = Assets.OtherSprites["coveredTile"];


            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Tile t = tiles[x, y];
                    if (t != null)
                    {
                        Point pos = new Point(x, y) * TileSize;
                        if (!t.IsDiscovered) 
                            coveredSprite.Render(spriteBatch, pos);
                        else if (t.Sprite != null)
                            t.Sprite.Render(spriteBatch, pos);
                        
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
