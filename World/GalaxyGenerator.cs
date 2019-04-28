using Barely.Util;
using LD44.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.World
{
    public class GalaxyGenerator
    {
        const int PLANET_COUNT = 28;
        Tile[,] tiles;
        public Tile HomePlanet;
        Point Size = Galaxy.Size;
        Random random = new Random();

        public Tile[,] Generate()
        {                      
            PlanetType[,] planets = new PlanetType[Size.X, Size.Y];                                     

            tiles = new Tile[Size.X, Size.Y];
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Sprite sprite = null;
                    if (random.NextDouble() > 0.8f)
                        sprite = Assets.GetRandomBackgroundSprite(random);
                    tiles[x, y] = new Tile(PlanetType.Empty, new Point(x, y), sprite, null, random);
                    planets[x, y] = PlanetType.Empty;
                }
            }            

            List<Point> placementPoints = GetPlanetPositions();

            GenerateTypes(planets, placementPoints);            

            return tiles;
        }

        public Tile Tile(Point p)
        {
            return tiles[p.X, p.Y];
        }

        const float PERC_SHOP = 0.30f;
        const float PERC_RAND = 0.50f;
        const float PERC_BATT = 0.20f;

        const int MAX_DIST_BETWEEN_SHOPS = 15;

        void GenerateTypes(PlanetType[,] planets, List<Point> placementPoints)
        {
            var planetNames = Assets.PlanetNames;
            Debug.Assert(planetNames.Count >= placementPoints.Count);
            planetNames.Shuffle(random);

            int numShop     = (int)(placementPoints.Count * PERC_SHOP);
            int numBattle   = (int)(placementPoints.Count * PERC_BATT);
            int numRandom   = placementPoints.Count - numShop - numBattle;

            for (int i = 0; i < placementPoints.Count; ++i)
            {
                Point p = placementPoints[i];
                PlanetType type;             

                if (i < numShop)
                    type = PlanetType.Shop;
                else if (i < numShop + numBattle)
                    type = PlanetType.EnemyBase;
                else
                    type = PlanetType.RandomEvent;

                tiles[p.X, p.Y] = new Tile(type, p, Assets.GetRandomPlanetSprite(random), planetNames[i], random);
                planets[p.X, p.Y] = type;
            }


            //get home planet:
            Point mostFarPlanet = new Point(-1, -1);
            for (int i = 0; i < placementPoints.Count; ++i)
            {
                Point p = placementPoints[i];
                if (p.X > mostFarPlanet.X)
                    mostFarPlanet = p;
                else if (p.X == mostFarPlanet.X)
                {
                    if (Math.Abs(p.Y - Size.Y / 2) < Math.Abs(mostFarPlanet.Y - Size.Y / 2))
                        mostFarPlanet = p;
                }                
            }

            Tile home = Tile(mostFarPlanet);
            home.Sprite = Assets.OtherSprites["homePlanet"];
            home.Name = "Home";
            home.Type = PlanetType.Home;
            HomePlanet = home;
        }


        #region Pushing planets apart

        private const int RECT_PUSH_DIST = 20;
        private const int PUSHED_OUT_OF_BOUNDS_MAX = 5;

        class PlacementInfo
        {
            public Rectangle rect;
            public int size;
            public int outOfBoundsCount;

            public PlacementInfo(Rectangle rect, int size)
            {
                this.rect = rect;
                this.size = size;
                outOfBoundsCount = 0;
            }

            public Point GetCityCenter()
            {
                return rect.Center;
            }
        }

        public List<Point> GetPlanetPositions()
        {
            List<PlacementInfo> placenemtInfos = new List<PlacementInfo>(PLANET_COUNT);

            for (int i = 0; i < PLANET_COUNT; i++)
            {                
                int size = 2;
                if (size >= Size.X)
                    size = Size.X - 1;
                if (size >= Size.Y)
                    size = Size.Y - 1;

                int x = random.Next(0, Size.X - size);
                int y = random.Next(0, Size.Y - size);
                //size is used as num of blocks, so take the sqrt here assuming a quadratic city. take 1.5x for padding between cities:
                Rectangle rect = new Rectangle(x, y, size, size);
                placenemtInfos.Add(new PlacementInfo(rect, size));
            }

            int notPlaceablePlanets = 0;

            //now move the rects appart!
            while (true)
            {
                bool smthMoved = false;
                bool skip = false;

                for (int a = 0; a < placenemtInfos.Count && !skip; a++)
                {
                    for (int b = a + 1; b < placenemtInfos.Count && !skip; b++)
                    {
                        Rectangle rectA = placenemtInfos[a].rect;
                        Rectangle rectB = placenemtInfos[b].rect;

                        if (rectA.Intersects(rectB))
                        {
                            //in which direction do the rects have to be seperated? take the axis they are seperated more.
                            int xMove = rectA.X - rectB.X;
                            int yMove = rectA.Y - rectB.Y;

                            if (Math.Abs(xMove) >= Math.Abs(yMove))
                            {
                                //if xMove is positive, push rectA in positive direction, and rectB in negative. if xMove is negative do it the other way around
                                int modifier = xMove > 0 ? 1 : -1;
                                rectA.X += RECT_PUSH_DIST * modifier;
                                rectB.X -= RECT_PUSH_DIST * modifier;
                            }
                            else
                            {
                                int modifier = yMove > 0 ? 1 : -1; //see comment above
                                rectA.Y += RECT_PUSH_DIST * modifier;
                                rectB.Y -= RECT_PUSH_DIST * modifier;
                            }
                            smthMoved = true;

                            //check if one of the rects is pushed out of bounds and move it inside bounds again.
                            if (IsOutOfBounds(rectA))
                            {
                                MoveIntoBounds(ref rectA);
                                placenemtInfos[a].outOfBoundsCount += 1;
                            }

                            if (IsOutOfBounds(rectB))
                            {
                                MoveIntoBounds(ref rectB);
                                placenemtInfos[b].outOfBoundsCount += 1;
                            }

                            placenemtInfos[a].rect = rectA;
                            placenemtInfos[b].rect = rectB;

                            //if a city is pushed out of bounds again and again, it could end in endless loop, so after X of pushed, remove a city
                            if (placenemtInfos[a].outOfBoundsCount > PUSHED_OUT_OF_BOUNDS_MAX)
                            {
                                placenemtInfos.RemoveAt(a);
                                notPlaceablePlanets += 1;
                                skip = true;
                                b -= 1; //a is smaller than b, so decrease b by one.
                            }
                            if (placenemtInfos[b].outOfBoundsCount > PUSHED_OUT_OF_BOUNDS_MAX)
                            {
                                placenemtInfos.RemoveAt(b);
                                notPlaceablePlanets += 1;
                                skip = true;
                            }

                        }
                    }
                }

                if (!smthMoved)
                    break;
            }

            Console.WriteLine($"Not Placeable planets: {notPlaceablePlanets}");

            List<Point> centers = new List<Point>(placenemtInfos.Count);
            placenemtInfos.ForEach(pi => centers.Add(pi.rect.Center));
            return centers;
        }


        /// <summary>
        /// Move the referenced rectangle into the bounds of the map.
        /// </summary>
        /// <param name="rect">A reference to the rectangle to be moved inside the map bounds.</param>
        private void MoveIntoBounds(ref Rectangle rect)
        {
            if (rect.X < 0)
                rect.X = 4;
            if (rect.Right >= Size.X)
                rect.X = Size.X - rect.Size.X - 4;
            if (rect.Y < 0)
                rect.Y = 4;
            if (rect.Y + rect.Size.Y >= Size.Y)
                rect.Y = Size.Y - rect.Size.Y - 4;
        }

        /// <summary>
        /// Check if a rectangle is out of the bounds of the map.
        /// </summary>
        /// <param name="r">The rectangle to be checked.</param>
        /// <returns>Boolean indicating if the rectangle is out of bounds.</returns>
        private bool IsOutOfBounds(Rectangle r)
        {
            Point p = r.Location;
            if (p.X < 0 || p.X >= Size.X || p.Y < 0 || p.Y >= Size.Y)
                return true;
            p = r.Location + r.Size;
            if (p.X < 0 || p.X >= Size.X || p.Y < 0 || p.Y >= Size.Y)
                return true;
            return false;
        }

        #endregion

    }
}
