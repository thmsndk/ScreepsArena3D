using Assets.Scripts.Common;
using Assets.Scripts.ScreepsArena3D.Views;
using Assets.Scripts.ScreepsArenaApi.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomView : MonoBehaviour
{
    // object id and a reference to the game object.
    private Dictionary<string, GameObject> gameState = new Dictionary<string, GameObject>();
    private TerrainView terrainView;
    private int size;

    // Start is called before the first frame update
    void Start()
    {
        this.terrainView = this.GetComponent<TerrainView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO: Load Terrain from game
    // 
    public void Init(int size)
    {
        this.size = size;
    }

    internal void Tick(ReplayChunkTick tick)
    {
        

        var users = tick.users; // TODO: handle more than two users in the future.
                                //var me = tick.users.player1.username == gameResponse.game.users.Single(u => u._id == gameResponse.game.user).username
        var remainingObjects = gameState.Keys.ToList();
        foreach (var roomObject in tick.objects)
        {
            gameState.TryGetValue(roomObject._id, out var go);

            if (remainingObjects.Contains(roomObject._id))
            {
                remainingObjects.Remove(roomObject._id);
            }
            else
            {
                if (go == null)
                {
                    // An object has spawned...
                    go = PoolLoader.Load($"Prefabs/RoomObjects/{roomObject.type}", this.transform);
                    go.name = $"{roomObject.type}-{roomObject._id}";
                    // TODO: set color if we are moving back and forth in ticks and have creeps dying/being added
                }
            }

            if (roomObject.type == "creep")
            {
                if (tick.gameTime == 0)
                {
                    var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                    ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                    var renderer = go.GetComponent<Renderer>();
                    renderer.material.SetColor("_BaseColor", color);
                    gameState.Add(roomObject._id, go);
                }

            }

            if (roomObject.type == "constructedWall")
            {

            }

            if (roomObject.type == "tower")
            {
                var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                var renderer = go.GetComponentInChildren<Renderer>();
                renderer.material.SetColor("_BaseColor", color);
            }

            if (roomObject.type == "flag")
            {
                var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                var renderer = go.GetComponentInChildren<Renderer>();
                renderer.material.SetColor("_BaseColor", color);
            }

            // TODO: We need a utility class to convert screeps cordinates to world cordinates.
            go.transform.position = new Vector3(roomObject.y, 0f, roomObject.x);
        }

        // theese objects where not in this tick. does that mean they died?
        foreach (var id in remainingObjects)
        {
            gameState.TryGetValue(id, out var gameObject);
            if (gameObject)
            {
                Destroy(gameObject);
                gameState.Remove(id);
            }
        }
    }
}
