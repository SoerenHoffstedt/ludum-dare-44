using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.Actors
{
    public enum Stats
    {
        Damage,
        Defense,
        Speed,
        Health,
        Fuel,
        Scanning,
        Count
    }

    public static class StatStuff
    {

        public static int MaxForStat(Stats stat)
        {
            switch (stat)
            {
                default:
                case Stats.Damage:
                case Stats.Defense:
                case Stats.Scanning:
                case Stats.Speed:
                    return 5;                    
                case Stats.Health:
                    return 10;
                case Stats.Fuel:
                    return 30;
            }
        }

    }

}
