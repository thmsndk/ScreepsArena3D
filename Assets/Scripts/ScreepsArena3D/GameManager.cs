using Assets.Scripts;
using Assets.Scripts.Common;
using Assets.Scripts.ScreepsArena3D.Views;
using Assets.Scripts.ScreepsArenaApi;
using Assets.Scripts.ScreepsArenaApi.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This class is responsible for swapping scenes as well as initializing prefab loaders and the likes
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string ArenaCaptureTheFlagBasicId = "606873c364da921cb49855f7";

    private static Transform shardContainer;
    public void Awake()
    {
        // Configure unity stacktrace log output
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
    }

    private ReplayDataProvider replayDataProvider;
    private IEnumerator renderTicks;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: wire up steam
        // TODO: the steam ticket we copy + paste from fiddler seams to expire when we close the game.
        ////var http = new Http();
        ////StartCoroutine(http.ScreepsArenaLogin("XXX", auth =>
        ////{
        ////    Debug.Log($"Authenticad with Screeps Arena servers, welcome {auth.username}");

        ////    StartCoroutine(http.GetLastGames(ArenaCaptureTheFlagBasicId, response =>
        ////    {
        ////        var latestGame = response;
        ////        StartCoroutine(GetReplayData(http, response.games[0]));
        ////    }));
        ////}));

        PrefabLoader.Init();
        PoolLoader.Init();

        // initialize "shard" container
        shardContainer = new GameObject("Shards").transform;

        // Load a shard
        var shard = new GameObject("shard", typeof(ShardView));
        shard.transform.SetParent(shardContainer);

        var shardView = shard.GetComponent<ShardView>();

        // TODO: some sort of logic that places a room next to other rooms if we load a new one.
        var room = PoolLoader.Load("Prefabs/RoomView", shardView.transform);
        var roomView = room.GetComponent<RoomView>();

        // TODO: when loading a room, we first need to feed it game data, then we feed it ticks
        string jsonFilePath = @$"{Application.persistentDataPath}\Replays\606873c364da921cb49855f7\609989f6891dffcde3f09554\game.json";
        var gameResponse = ReplayDataProvider.ReadJsonFromFile<GameResponseGame>(jsonFilePath);

        var terrainView = room.GetComponentInChildren<TerrainView>();
        roomView.Init(100);
        terrainView.Init(gameResponse.game.terrain, 100); // TODO: determine arena size?
        // TODO: Something that can feed data to all rooms.

        // download latest replay


        replayDataProvider = new ReplayDataProvider(gameResponse.game._id);

        // TODO: Global "tick" processor that ticks all rooms. If you set a specific tick, all rooms should be fed tick data for that specific tick.
        renderTicks = RenderTicks(roomView, gameResponse, 0.5f);
    }

    /*
     * TODO: Move this to some sort of component, the component should be instructed to load a replay
     * The loading of a replay should have multiple modes depending on what the game state of the replay is.
     * if it is running, we should poll for a new game state untill finished
     * once finished, we should start loading tick chunks.
     * the component should be able to autobuffer or be manual controlled so tick chunks can be requested to be loaded.
     * it should check it's replay cache on disc before making a request to the arena servers.
     */
    private IEnumerator GetReplayData(Http http, ArenaLastGamesResponseGame game)
    {

        GameResponse latestGame = null;
        yield return http.GetGame(game.game._id, gameResponse =>
        {
            latestGame = gameResponse;
        });

        while (latestGame == null)
        {
            yield return new WaitForSeconds(1);
        }

        var replay = new Replay(latestGame);

        var ticks = latestGame.game.meta.ticks;
        var REPLAY_CHUNK_SIZE = 100;

        var neededChunks = Math.Ceiling((float)ticks / REPLAY_CHUNK_SIZE);
        for (int chunk = 0; chunk <= neededChunks; chunk++)
        {
            int chunkId = GetChunkId(ticks, REPLAY_CHUNK_SIZE, neededChunks, chunk);

            ReplayChunkResponse replayChunkResponse = null;
            yield return http.GetReplayChunk(game.game._id, chunkId, chunkResponse =>
            {
                replayChunkResponse = chunkResponse;
            });

            while (replayChunkResponse == null)
            {
                yield return new WaitForSeconds(1);
            }

            replay.ReplayChunks.Add(chunkId, replayChunkResponse);
        }

        StartCoroutine(PersistReplay(replay));
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

    private IEnumerator PersistReplay(Replay replay)
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

    // concepts / ideas
    // Showing multiple replays at the same time, render each arena as a "room"
    // a room can get updates to state, this should not be tightly coupled, it should be able to get state updates anywhere and adjust the rendering accordingly.
    // it makes sense for a roomobject to "enter" the room

    // domain / data structure
    // "server" -> "shard" -> "room/arena" -> roomobjects
    // The concept of "views" doing the actual rendering seems nice.
    // we need a room prefab, a room has a width and a height. usually square
    // A room can be loaded, it can also be unloaded
    // We also need an api implementation
    private IEnumerator RenderTicks(RoomView roomView, GameResponseGame gameResponse, float ticksPerScond)
    {
        var ticks = gameResponse.meta.ticks;
        var REPLAY_CHUNK_SIZE = 100;

        var neededChunks = Math.Ceiling((float)ticks / REPLAY_CHUNK_SIZE);
        for (int chunk = 0; chunk <= neededChunks; chunk++)
        {
            // TODO: this replay data provider should be providing data for a specific replay, it should be initialized with a gameId
            int chunkId = GetChunkId(ticks, REPLAY_CHUNK_SIZE, neededChunks, chunk);

            var response = replayDataProvider.GetReplayChunk(chunkId);
            foreach (var tick in response.Ticks)
            {
                roomView.Tick(tick);

                yield return new WaitForSecondsRealtime(1 / ticksPerScond);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            renderTicks.MoveNext();
        }
    }
}
