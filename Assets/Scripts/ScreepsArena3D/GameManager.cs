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

    [SerializeField]
    private ReplayControl replayControl;

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

        
        replayDataProvider = room.AddComponent<ReplayDataProvider>();
        
        var terrainView = room.GetComponentInChildren<TerrainView>();
        replayDataProvider.OnTerrain += (terrain, size) =>
        {
            roomView.Init(size);
            terrainView.Init(terrain, size);
        };


        replayControl.OnTick += (tick) =>
        {
            // TODO: should this trigger on each room instead?
            // TODO: Get tick data from (replay) data provider. feed data to the roomview
            // The data provider just provides chunks of data as it is now, it should cache data in memory so it can provide data for a spcific tick, and request / load chunks when needed.
            // in the room view, we should iterate roomobject data and update their data / movement target.
            // ReceiveData

            // OnTick kinda works like the websocket connection, when it ticks, data should be fed to the room. thus OnTick itself should be detatched from the room

            var tickData = replayDataProvider.GetTick(tick);

            if (tickData != null)
            {
                roomView.Tick(tickData);
            }
        };

        replayDataProvider.Init(null, "606873c364da921cb49855f7", "609989f6891dffcde3f09554");

        // TODO: Global "tick" processor that ticks all rooms. If you set a specific tick, all rooms should be fed tick data for that specific tick.
        //renderTicks = RenderTicks(roomView, gameResponse, 0.5f);

        // TODO: register onTick for each room?

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
    //private IEnumerator RenderTicks(RoomView roomView, GameResponseGame gameResponse, float ticksPerScond)
    //{
    //    var ticks = gameResponse.meta.ticks;
    //    var REPLAY_CHUNK_SIZE = 100;

    //    var neededChunks = Math.Ceiling((float)ticks / REPLAY_CHUNK_SIZE);
    //    for (int chunk = 0; chunk <= neededChunks; chunk++)
    //    {
    //        // TODO: this replay data provider should be providing data for a specific replay, it should be initialized with a gameId
    //        int chunkId = GetChunkId(ticks, REPLAY_CHUNK_SIZE, neededChunks, chunk);

    //        var response = replayDataProvider.GetReplayChunk(chunkId);
    //        foreach (var tick in response.Ticks)
    //        {
    //            roomView.Tick(tick);

    //            yield return new WaitForSecondsRealtime(1 / ticksPerScond);
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    renderTicks.MoveNext();
        //}
    }
}
