using Assets.Scripts.ScreepsArenaApi;
using Assets.Scripts.ScreepsArenaApi.Responses;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    // This is an implementation of a data provider that fetches replay data from a saved replay / on disk
    public class ReplayDataProvider : MonoBehaviour
    {
        private const int REPLAY_CHUNK_SIZE = 100;
        private Http http;
        private string arenaId;
        private string gameId;
        private ScreepsArenaApi.ReplayData replay;

        public Action<string, int> OnTerrain { get; internal set; }

        private void Start()
        {
            
        }

        private GameResponseGame GetGameFromCache()
        {
            string jsonFilePath = @$"{Application.persistentDataPath}\Replays\{arenaId}\{gameId}\game.json";
            Debug.Log($"Getting game from cache: {jsonFilePath}");
            return ReadJsonFromFile<GameResponseGame>(jsonFilePath);
        }

        private ReplayChunkResponse GetReplayChunkFromCache(int chunk)
        {

            string jsonFilePath = @$"{Application.persistentDataPath}\Replays\{arenaId}\{gameId}\{chunk}.json";

            return new ReplayChunkResponse { Ticks = ReadJsonFromFile<ReplayChunkTick[]>(jsonFilePath) };
        }

        public static T ReadJsonFromFile<T>(string filePath)
        {
            // TODO: verify if file exists
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        internal ReplayChunkTick GetTick(int tick)
        {
            // do we have the tick in memory?
            // do we have the chunk the tick belongs to on disc?
            // make request to arena api for replay data.

            // TODO: determine the chunk of the tick, fetch it then fetch the tick. 
            // TODO: keep a reference around for current chunk

            foreach (var chunk in replay.ReplayChunks)
            {
                var replayTick = chunk.Value.Ticks.SingleOrDefault(t => t.gameTime == tick);

                if (replayTick != null)
                {
                    return replayTick;
                }
            }

            // TODO: we need the tick size, we need it to determine if there is any replay data to show for this tick.

            return null;
        }

        internal void Init(Http http, string arenaId, string gameId)
        {
            this.http = http;
            this.arenaId = arenaId;
            this.gameId = gameId;

            // TODO: coroutines
            // first it should attempt to find the replay in the "saved replays" folder, if that fails it should start fetching them from the api. when fetching a chunk, it should always check if we have it before asking the api.
            // TODO: we should be able to unzip a replay in memory, to process data.
            // TODO: we need to be able to persist a replay once all chunks are fetched. instead of the provider, it should probably be a replay streamer / buffer

            StartCoroutine(GetReplayData());
        }


        /*
         * TODO: Move this to some sort of component, the component should be instructed to load a replay
         * The loading of a replay should have multiple modes depending on what the game state of the replay is.
         * if it is running, we should poll for a new game state untill finished
         * once finished, we should start loading tick chunks.
         * the component should be able to autobuffer or be manual controlled so tick chunks can be requested to be loaded.
         * it should check it's replay cache on disc before making a request to the arena servers.
         */
        private IEnumerator GetReplayData()
        {
            Debug.Log("GetReplayData");

            GameResponseGame latestGame = GetGameFromCache();

            // Get cached game

            // else query arena api
            //yield return http.GetGame(gameId, gameResponse =>
            //{
            //    latestGame = gameResponse;
            //    // TODO: add to cache
            //});

            while (latestGame == null)
            {
                Debug.Log("Waiting for GameResponseGame");
                yield return new WaitForSeconds(1);
            }

            replay = new ScreepsArenaApi.ReplayData(latestGame);

            if (OnTerrain != null)
            {
                Debug.Log("terrain:" + replay.Game.game.terrain);
                OnTerrain.Invoke(replay.Game.game.terrain, 100); // TODO: make terrain size dynamic somehow
            }

            var ticks = latestGame.meta.ticks;

            var neededChunks = Math.Ceiling((float)ticks / REPLAY_CHUNK_SIZE);
            for (int chunk = 0; chunk <= neededChunks; chunk++)
            {
                int chunkId = GetChunkId(ticks, REPLAY_CHUNK_SIZE, neededChunks, chunk);

                Debug.Log($"Cache: ReplayChunkResponse({chunkId})");
                ReplayChunkResponse replayChunkResponse = GetReplayChunkFromCache(chunkId);
                // Get cached chunk

                // else querey api
                //yield return http.GetReplayChunk(gameId, chunkId, chunkResponse =>
                //{
                //    replayChunkResponse = chunkResponse;
                //    // TODO: add to cache / persist
                //});

                while (replayChunkResponse == null)
                {
                    Debug.Log($"Waiting for ReplayChunkResponse({chunkId})");
                    yield return new WaitForSeconds(1);
                }

                replay.ReplayChunks.Add(chunkId, replayChunkResponse);
            }

            Debug.Log($"Done fetching replay data for {gameId}");

            //StartCoroutine(PersistReplay(replay));
        }

        private static int GetChunkId(int ticks, int REPLAY_CHUNK_SIZE, double neededChunks, int chunk)
        {
            var chunkId = chunk * REPLAY_CHUNK_SIZE;

            // The last chunk is ticks % REPLAY_CHUNK_SIZE in size
            if (chunk == neededChunks)
            {
                // 282 ticks, last chunk should be 282 not 300
                chunkId -= REPLAY_CHUNK_SIZE - (ticks % REPLAY_CHUNK_SIZE);
            }

            return chunkId;
        }

        private IEnumerator PersistReplay(Assets.Scripts.ScreepsArenaApi.ReplayData replay)
        {
            Debug.Log($"Persisting replay for {replay.Game.game._id}");

            var basePath = $"{Application.persistentDataPath}/Replays/{replay.Game.arena}/{replay.Game.game._id}";
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string gameFilename = $"{basePath}/game.json";
            using (StreamWriter file = File.CreateText(gameFilename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, replay.Game);
                Debug.Log(gameFilename);
            }

            foreach (var chunk in replay.ReplayChunks)
            {
                string chunkFilename = $"{basePath}/{chunk.Key}.json";
                using (StreamWriter file = File.CreateText(chunkFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, chunk.Value.Ticks);
                    Debug.Log(chunkFilename);
                }
            }

            // TODO: Zip it up.

            yield break;
        }
    }
}
