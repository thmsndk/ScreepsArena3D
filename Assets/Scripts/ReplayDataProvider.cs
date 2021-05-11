using Assets.Scripts.ScreepsArenaApi.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    // This is an implementation of a data provider that fetches replay data from a saved replay / on disk
    public class ReplayDataProvider
    {

        public ReplayDataProvider(string gameId)
        {
            // TODO: coroutines
            // first it should attempt to find the replay in the "saved replays" folder, if that fails it should start fetching them from the api. when fetching a chunk, it should always check if we have it before asking the api.
            // TODO: we should be able to unzip a replay in memory, to process data.
            // TODO: we need to be able to persist a replay once all chunks are fetched. instead of the provider, it should probably be a replay streamer / buffer

        }
        
        public ReplayChunkResponse GetReplayChunk(int chunk)
        {

            string jsonFilePath = @$"{Application.persistentDataPath}\Replays\606873c364da921cb49855f7\609989f6891dffcde3f09554\{chunk}.json";

            return new ReplayChunkResponse { Ticks = ReadJsonFromFile<ReplayChunkTick[]>(jsonFilePath) };
        }

        public static T ReadJsonFromFile<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
