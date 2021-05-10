using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScreepsArenaApi.Responses
{
    public class ReplayChunkResponse
    {
        public ReplayChunkTick[] Ticks;
    }

    public class ReplayChunkTick
    {
        public int gameTime;
        public ReplayChunkRoomObject[] objects;
        public ReplayChunkUsers users;
    }

    public class ReplayChunkUsers
    {
        public ReplayChunkPlayer player1;
        public ReplayChunkPlayer player2;
    }

    public class ReplayChunkPlayer
    {
        public string _id;
        public string username;
        public string color;
    }

    public class ReplayChunkRoomObject
    {
        public string type;
        public string prototypeName;
        public int x;
        public int y;
        public int hits;
        public int hitsMax;
        public ReplayChunkBody[] body;
        public bool spawning;
        public int ageTime;
        public int fatigue;
        public int storeCapacity;
        public ReplayChunkStore store;
        public string user;
        public ReplayChunkActionLog actionLog;
        public string _id;
    }

    public class ReplayChunkStore
    {
    }

    public class ReplayChunkActionLog
    {
    }

    public class ReplayChunkBody
    {
        public string type;
        public int hits;
    }

}
