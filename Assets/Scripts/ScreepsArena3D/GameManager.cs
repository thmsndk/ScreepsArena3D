using Assets.Scripts;
using Assets.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for swapping scenes as well as initializing prefab loaders and the likes
/// </summary>
public class GameManager : MonoBehaviour
{
    private static Transform shardContainer;
    public void Awake()
    {
        // Configure unity stacktrace log output
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
    }

    private ReplayDataProvider replayDataProvider = new ReplayDataProvider();

    // Start is called before the first frame update
    void Start()
    {
        // TODO: wire up steam
        
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
        

        // TODO: Something that can feed data to all rooms.

        // TODO: Global "tick" processor that ticks all rooms. If you set a specific tick, all rooms should be fed tick data for that specific tick.
        StartCoroutine(RenderTicks(roomView, 10f));
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
    private IEnumerator RenderTicks(RoomView roomView, float ticksPerScond)
    {

        // TODO: this replay data provider should be providing data for a specific replay, it should be initialized with a gameId
        var response = replayDataProvider.GetReplayChunk(0);

        // initialize room, gameTime 0
        foreach (var tick in response.Ticks)
        {

            roomView.Tick(tick);

            yield return new WaitForSecondsRealtime(1 / ticksPerScond);
        }

        var chunk100 = replayDataProvider.GetReplayChunk(100);
        foreach (var tick in chunk100.Ticks)
        {
            roomView.Tick(tick);

            yield return new WaitForSecondsRealtime(1f / ticksPerScond);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
