using Assets.Scripts.ScreepsArenaApi.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScreepsArenaApi
{
    public struct ReplayData
    {
        public ReplayData(GameResponseGame game)
        {
            Game = game;
            ReplayChunks = new Dictionary<int, ReplayChunkResponse>();
        }

        public GameResponseGame Game { get; }

        public Dictionary<int, ReplayChunkResponse> ReplayChunks { get; }
    }
}
