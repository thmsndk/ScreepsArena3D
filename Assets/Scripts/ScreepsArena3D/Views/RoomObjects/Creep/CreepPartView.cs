//using Assets.Scripts.ScreepsArenaApi.Responses;
//using UnityEngine;

//namespace Screeps3D.RoomObjects.Views
//{
//    /// <summary>
//    /// Seems to be some sort of baseclass used by AttackView and WorkView, not sure if they are relevant anymore.
//    /// </summary>
//    public class CreepPartView : MonoBehaviour, IObjectViewComponent
//    {
//        [SerializeField] private Transform _partDisplay = default;

//        //internal Creep creep;
//        //internal CreepView view;

//        public void Init()
//        {
//        }

//        public virtual void Load(ReplayChunkRoomObject roomObject)
//        {
//            // TOOD: interfaces for the replaychunk room object?

//            //creep = roomObject as Creep;
//            //view = creep.View as CreepView;
//            // TODO: extract body parts
//        }

//        public virtual void Tick(ReplayChunkRoomObject roomObject)
//        {
//        }

//        public void Unload(ReplayChunkRoomObject roomObject)
//        {
//        }

//        protected void AdjustSize(string partType, float min, float flex)
//        {
//            //var amount = 0f;
//            //foreach (var part in creep.Body.Parts)
//            //{
//            //    if (part.Type != partType)
//            //        continue;
//            //    amount += part.Hits;
//            //}

//            //var scaleAmount = 0f;
//            //if (amount > 0)
//            //{
//            //    scaleAmount = (amount / 5000) * flex + min;
//            //}

//            //_partDisplay.transform.localScale = Vector3.one * scaleAmount;
//        }

//        //protected Vector3 GetActionVector(JSONObject data)
//        //{
//        //    return new Vector3(data["x"].n - creep.X, 0, creep.Y - data["y"].n);
//        //}
//    }
//}