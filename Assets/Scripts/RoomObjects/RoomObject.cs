using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RoomObjects
{
    public class RoomObject
    {
        public string Type { get; set; }

        // TODO: vector?
        public int X { get; set; }

        public int Y { get; set; }
    }

    public class Creep : RoomObject
    {
        public int Hits { get; set; }
        public int HitsMax { get; set; }

        public string user { get; set; } // a string with a user reference "player1,player2" we might want to switch that to a user object later.

        // TODO: body, store, actionlog, spawning, agetime,fatigue, storecapacity, store

        public Creep()
        {
            this.Type = "creep";
        }
    }


    public class Structure : RoomObject
    {
        public int Hits { get; set; }
        public int HitsMax { get; set; }
    }

    public class StructureTower : Structure
    {
        public StructureTower()
        {
            this.Type = "tower";
        }
    }

    public class StructureWall : Structure
    {
        public StructureWall()
        {

        }
    }

    // TODO: bodyparts, flags, they are arena specific though so we need to think about the structure of the data for later additions
}
