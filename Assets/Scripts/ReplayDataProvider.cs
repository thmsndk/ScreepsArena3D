using Assets.Scripts.ScreepsArenaApi.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    // This is an implementation of a data provider that fetches replay data from a saved replay / on disk
    public class ReplayDataProvider
    {
        public ReplayChunkResponse GetReplayChunk(int tick)
        {
            if (tick == 0)
            {
                string jsonFilePath = "SavedReplay/test/0";
                TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);
                
                return new ReplayChunkResponse { Ticks = JsonConvert.DeserializeObject<ReplayChunkTick[]>(loadedJsonFile.text) };
            }

            if (tick > 0 && tick <= 100)
            {
                string jsonFilePath = "SavedReplay/test/100";
                TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);

                return new ReplayChunkResponse { Ticks = JsonConvert.DeserializeObject<ReplayChunkTick[]>(loadedJsonFile.text) };
            }

            return null;
        }

    }
}
