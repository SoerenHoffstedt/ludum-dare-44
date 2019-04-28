using Barely.Util;
using LD44.Actors;
using LD44.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LD44
{
    public static class Assets
    {
        public static Texture2D WorldTexture;
        public static Texture2D EffectsTexture;
        public static Texture2D UiTexture;

        public static Sprite[] TileSprites;
        public static Sprite[] PlanetSprites;
        public static Sprite[] ShipSprites;
        public static Sprite[] BackgroundSprites;
        public static Dictionary<string, Sprite> OtherSprites;

        public static Dictionary<string, ShipBlueprint> ShipBlueprints;
        public static ShipBlueprint[] EnemyShipBlueprints;
        public static RandomEvent[] RandomEvents;

        public static List<string> PlanetNames;

        private static ContentManager Content;

        public static void Load(ContentManager Content)
        {
            Assets.Content = Content;
            XmlDocument def = new XmlDocument();
            def.Load("Content/def.xml");

            WorldTexture = Content.Load<Texture2D>("graphics");
            EffectsTexture = Content.Load<Texture2D>("effects");
            LoadSprites(def);
            LoadPlanetSprites();
            LoadBackgroundSprites();
            LoadShipSprites();
            LoadPlanetNames();

            LoadShipBlueprints(def);
            LoadRandomEvents(def);
        }

        private static void LoadRandomEvents(XmlDocument def)
        {
            var list = def.SelectNodes("definitions/randomEvents/e");
            RandomEvents = new RandomEvent[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                RandomEvents[i] = new RandomEvent(list[i]);
            }
        }

        private static void LoadShipBlueprints(XmlDocument def)
        {
            var list = def.SelectNodes("definitions/ships/s");
            ShipBlueprints = new Dictionary<string, ShipBlueprint>(list.Count);
            EnemyShipBlueprints = new ShipBlueprint[list.Count - 2];
            int c = 0;
            for(int i = 0; i < list.Count; ++i)
            {                
                var blueprint = new ShipBlueprint(list[i]);
                ShipBlueprints.Add(blueprint.Id, blueprint);
                if (!blueprint.Id.StartsWith("player"))
                {
                    EnemyShipBlueprints[c] = blueprint;
                    ++c;
                }
            }
        }

        private static void LoadPlanetNames()
        {
            PlanetNames = File.ReadAllLines("Content/planet_names.txt").ToList();
        }

        private static void LoadSprites(XmlDocument def)
        {            
            var nodeList = def.SelectNodes("definitions/graphics/sprites/s");
            OtherSprites = new Dictionary<string, Sprite>(nodeList.Count);

            foreach(XmlNode spriteNode in nodeList)
            {
                string name = spriteNode.Attributes["name"].Value;

                Sprite sp = Sprite.Parse(spriteNode, Content);            
                OtherSprites.Add(name, sp);
            }

        }       

        private static void LoadPlanetSprites()
        {
            const int count = 7;
            PlanetSprites = new Sprite[count];
            Point start = new Point(0, 0);

            for (int i = 0; i < count; i++)
            {
                PlanetSprites[i] = new Sprite(WorldTexture, new Rectangle(start, Galaxy.TileSize));
                start.X += Galaxy.TileSize.X;
            }
        }

        private static void LoadBackgroundSprites()
        {
            const int count = 6;
            BackgroundSprites = new Sprite[count];
            Point start = new Point(32, 16);

            for (int i = 0; i < count; i++)
            {
                BackgroundSprites[i] = new Sprite(WorldTexture, new Rectangle(start, Galaxy.TileSize));
                start.X += Galaxy.TileSize.X;
            }
        }

        private static void LoadShipSprites()
        {
            const int count = 6;
            ShipSprites = new Sprite[count];
            Point start = new Point(0, 32);

            for (int i = 0; i < count; i++)
            {
                ShipSprites[i] = new Sprite(WorldTexture, new Rectangle(start, Galaxy.TileSize));
                start.X += Galaxy.TileSize.X;
            }
        }


        public static Sprite GetRandomBackgroundSprite(Random random)
        {
            return BackgroundSprites[random.Next(BackgroundSprites.Length)];
        }

        public static Sprite GetRandomPlanetSprite(Random random)
        {
            return PlanetSprites[random.Next(PlanetSprites.Length)];
        }

        public static RandomEvent GetRandomEvent(Random rng)
        {
            return RandomEvents[rng.Next(RandomEvents.Length)];
        }

        public static ShipBlueprint GetRandomEnemyShip(Random rng)
        {
            return EnemyShipBlueprints[rng.Next(EnemyShipBlueprints.Length)];
        }

    }
}
