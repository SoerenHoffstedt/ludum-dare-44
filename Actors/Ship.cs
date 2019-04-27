using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.Actors
{
    public class Ship
    {
        public string name;
        private int[] stats;

        public Ship()
        {
            stats = new int[(int)Stats.Count];
        }

        public int GetStat(Stats s)
        {
            return stats[(int)s];
        }

    }
}
