using Assets.Scripts.Common;
using Assets.Scripts.ScreepsArena3D;
using Assets.Scripts.ScreepsArenaApi.Responses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ScreepsArena3D.Views.RoomObjects
{
    [RequireComponent(typeof(ScaleVisibility))]
    public class RoomObjectView : MonoBehaviour
    {
        internal IObjectViewComponent[] components;
        protected ScaleVisibility _vis;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void Init(RoomView roomView)
        {
            components = GetComponentsInChildren<IObjectViewComponent>();
            _vis = GetComponent<ScaleVisibility>();
            //_vis.OnFinishedAnimation += OnFinishedAnimation;

            foreach (var component in components)
            {
                component.Init(roomView);
            }
        }

        internal virtual void Load(ReplayChunkRoomObject roomObject)
        {
            //RoomObject = roomObject;
            //transform.position = RoomObject.Position;
            //roomObject.OnShow += Show;

            // TODO: We need a utility class to convert screeps cordinates to world cordinates.
            transform.position = PosUtility.Convert(roomObject.x, roomObject.y, 100); // TODO: We need a reference to the RoomView to acquire the size / position within the room

            foreach (var component in components)
            {
                component.Load(roomObject);
            }

            //Show(roomObject, true);
        }

        internal virtual void Tick(ReplayChunkRoomObject roomObject)
        {
            foreach (var component in components)
            {
                component.Load(roomObject);
            }
        }
        // In Screeps3D this is OnFinishedAnimation. i'm not exactly sure what animation it is that is finished. ScaleVisibility probably.
        internal virtual void Unload()
        {
            foreach (var component in components)
            {
                component.Unload();
            }
        }
    }

    internal interface IObjectViewComponent
    {
        void Init(RoomView roomView);
        void Load(ReplayChunkRoomObject roomObject); // TODO: interfaces?

        void Tick(ReplayChunkRoomObject data);
        void Unload();
    }
}