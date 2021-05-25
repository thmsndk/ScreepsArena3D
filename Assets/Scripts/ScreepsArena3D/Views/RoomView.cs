using Assets.Scripts.Common;
using Assets.Scripts.ScreepsArena3D.Views;
using Assets.Scripts.ScreepsArenaApi.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ScreepsArena3D
{
    public class RoomView : MonoBehaviour
    {
        // object id and a reference to the game object.
        private Dictionary<string, GameObject> gameState = new Dictionary<string, GameObject>();
        private TerrainView terrainView;
        private int size;

        // Start is called before the first frame update
        void Start()
        {
            terrainView = GetComponent<TerrainView>();
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
            // new up a data provider, start listening for events. the data provider should also subscribe to the replay controls, though that seems tightly coupled.
            // We should listen for terrain and tick data, perhaps we want to wire all this up in RoomManager instead.
        }

        internal void Tick(ReplayChunkTick tick)
        {


            var users = tick.users; // TODO: handle more than two users in the future.
                                    //var me = tick.users.player1.username == gameResponse.game.users.Single(u => u._id == gameResponse.game.user).username
            var remainingObjects = gameState.Keys.ToList();
            foreach (var roomObject in tick.objects)
            {
                gameState.TryGetValue(roomObject._id, out var go);
                RoomObjectView view;

                if (remainingObjects.Contains(roomObject._id))
                {
                    view = go.GetComponent<RoomObjectView>();
                    remainingObjects.Remove(roomObject._id);
                }
                else
                {
                    if (go == null)
                    {
                        // An object has spawned...
                        go = PoolLoader.Load($"Prefabs/RoomObjects/{roomObject.type}", transform);
                        go.name = $"{roomObject.type}-{roomObject._id}";
                        // TODO: set color if we are moving back and forth in ticks and have creeps dying/being added
                        gameState.Add(roomObject._id, go);
                        // TODO: we might want to persist a RoomObjectView instead of the gameobject, allowing us to call .Init/Load .Unload or .Tick
                        view = go.GetComponent<RoomObjectView>();
                        if (view != null)
                        {
                            view.Init();
                            view.Load(roomObject);
                        }
                        else
                        {
                            Debug.LogWarning($"{go.name} prefab has no RoomObjectView component assigned.");
                        }
                    }
                    else
                    {
                        // TODO: we should probably store the objectview in the gamestate instead of the game object.
                        view = go.GetComponent<RoomObjectView>();
                    }
                }

                view?.Tick(roomObject);

                if (roomObject.type == Constants.TypeCreep)
                {
                    if (tick.gameTime == 0)
                    {
                        var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                        ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                        var renderer = go.GetComponentInChildren<Renderer>();
                        //renderer.material.SetColor("_BaseColor", color);
                        renderer.material.SetColor("_EmissionColor", color);

                    }
                    
                    //foreach (var action in roomObject.actionLog) actionlog seems to be a dictionary, it's either null or an empty object in the replay I have currently though.
                    //{

                    //}
                }

                if (roomObject.type == Constants.TypeConstructedWall)
                {

                }

                if (roomObject.type == Constants.TypeTower)
                {
                    var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                    ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                    var renderer = go.GetComponentInChildren<Renderer>();
                    renderer.material.SetColor("_BaseColor", color);
                }

                // TODO: move to flagview
                if (roomObject.type == Constants.TypeFlag)
                {
                    // TODO: this kind of logic belongs in "views" and theese views should be on the prefabs of the room objects
                    var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                    ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                    var renderer = go.GetComponentInChildren<Renderer>();
                    //renderer.material.SetColor("_BaseColor", color);

                    renderer.material.SetColor(ShaderKeys.FlagShader.PrimaryColor, color);
                    renderer.material.SetColor(ShaderKeys.FlagShader.SecondaryColor, color);
                }
            }

            // theese objects where not in this tick. does that mean they died?
            foreach (var id in remainingObjects)
            {
                gameState.TryGetValue(id, out var gameObject);
                if (gameObject)
                {
                    var view = gameObject.GetComponent<RoomObjectView>();
                    view?.Unload();

                    var roomObjectType = gameObject.name.Substring(0, gameObject.name.IndexOf("-"));
                    var poolLoaderPath = $"Prefabs/RoomObjects/{roomObjectType}";
                    Debug.Log($"Returning {gameObject.name} to the pool for {roomObjectType}");
                    PoolLoader.Return(poolLoaderPath, gameObject);
                    gameState.Remove(id);
                }
            }
        }
    }
}