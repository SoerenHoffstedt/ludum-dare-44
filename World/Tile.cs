using Barely.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.World
{

    public enum PlanetType
    {
        Empty,
        RandomEvent,
        Shop,
        EnemyBase,
        Home
    }

    public class Tile
    {
        public string Name;
        public PlanetType Type;
        public Point Coord;
        public Sprite Sprite;
        public bool IsDiscovered;
        public PlanetInfo planetInfo;

        public Tile(PlanetType type, Point coord, Sprite sprite, string name, Random rng)
        {
            Type = type;
            Coord = coord;
            Sprite = sprite;
            Name = name;
            IsDiscovered = true; // false;
            planetInfo = PlanetInfo.CreatePlanetInfo(type, rng);
        }
        
    }
}
