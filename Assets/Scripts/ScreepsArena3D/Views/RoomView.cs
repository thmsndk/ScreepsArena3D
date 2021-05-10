using Assets.Scripts.Common;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Tick(ReplayChunkTick tick)
    {
        var users = tick.users; // TODO: handle more than two users in the future.
                                //var me = tick.users.player1.username == gameResponse.game.users.Single(u => u._id == gameResponse.game.user).username
        var remainingObjects = gameState.Keys.ToList();
        foreach (var roomObject in tick.objects)
        {
            if (remainingObjects.Contains(roomObject._id))
            {
                remainingObjects.Remove(roomObject._id);
            }
            else
            {
                // An object has spawned...
            }

            if (roomObject.type == "creep")
            {
                gameState.TryGetValue(roomObject._id, out var creep);

                if (tick.gameTime == 0)
                {
                    var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                    ColorUtility.TryParseHtmlString(ownerColorHex, out var color);

                    creep = PoolLoader.Load("Prefabs/RoomObjects/Creep", this.transform);
                    creep.name = $"creep-{roomObject._id}";
                    var renderer = creep.GetComponent<Renderer>();
                    renderer.material.SetColor("_BaseColor", color);
                    gameState.Add(roomObject._id, creep);
                }

                creep.transform.position = new Vector3(roomObject.x, 0f, roomObject.y);
            }
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
