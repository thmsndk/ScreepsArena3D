using Assets.Scripts.ScreepsArenaApi.Responses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectView : MonoBehaviour
{
    internal IObjectViewComponent[] components;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Init()
    {
        components = GetComponentsInChildren<IObjectViewComponent>();
        //_vis = GetComponent<ScaleVisibility>();
        //_vis.OnFinishedAnimation += OnFinishedAnimation;

        foreach (var component in components)
        {
            component.Init();
        }
    }

    internal virtual void Load(ReplayChunkRoomObject roomObject)
    {
        //RoomObject = roomObject;
        //transform.position = RoomObject.Position;
        //roomObject.OnShow += Show;

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
    void Init();
    void Load(ReplayChunkRoomObject roomObject); // TODO: interfaces?

    void Tick(ReplayChunkRoomObject data);
    void Unload();
}