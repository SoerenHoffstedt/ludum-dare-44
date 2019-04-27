using LD44.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.World
{
    public class PlanetInfo
    {



        public static PlanetInfo CreatePlanetInfo(PlanetType type, Random rng)
        {
            switch (type)
            {
                default:
                case PlanetType.Home:
                case PlanetType.Empty:
                    return null;
                case PlanetType.EnemyBase:
                    return new RandomBattleInfo(rng);
                case PlanetType.RandomEvent:
                    return new RandomEventInfo(rng);
                case PlanetType.Shop:
                    return new ShopInfo(rng);
            }

        }
    }

    public class RandomBattleInfo : PlanetInfo
    {
        public ShipBlueprint enemyBlueprint;

        public RandomBattleInfo(Random rng)
        {
            enemyBlueprint = Assets.GetRandomEnemyShip(rng);
        }
    }

    public class RandomEventInfo : PlanetInfo
    {
        public RandomEvent RandomEvent;
        public bool AlreadyExecuted;

        public RandomEventInfo(Random rng)
        {
            RandomEvent = Assets.GetRandomEvent(rng);
            AlreadyExecuted = false;
        }
    }

    public class ShopInfo : PlanetInfo
    {
        private const int FUEL_BASE = 15;
        private const int FUEL_OFFSET = 10;

        public int FuelToBuy;

        public ShopInfo(Random rng)
        {
            FuelToBuy = (int)(FUEL_BASE + rng.NextDouble() * FUEL_OFFSET); 
        }

    }
}
