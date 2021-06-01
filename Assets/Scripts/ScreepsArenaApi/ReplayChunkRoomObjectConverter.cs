using Assets.Scripts.ScreepsArena3D;
using Assets.Scripts.ScreepsArenaApi.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScreepsArenaApi
{
    /// <summary>
    /// https://stackoverflow.com/a/12833954/28145
    /// </summary>
    public class ReplayChunkRoomObjectConverter : Newtonsoft.Json.Converters.CustomCreationConverter<ReplayChunkRoomObject>
    {
        public override ReplayChunkRoomObject Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public ReplayChunkRoomObject Create(Type objectType, JObject jObject)
        {
            var type = (string)jObject.Property("type");

            switch (type)
            {
                case Constants.TypeCreep:
                    return new ReplayChunkRoomObjectCreep();
                case Constants.TypeSpawn:
                    return new ReplayChunkRoomObjectSpawn();
                default:
                    return new ReplayChunkRoomObject();
            }

            throw new ApplicationException(String.Format("The animal type {0} is not supported!", type));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream 
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject 
            var target = Create(objectType, jObject);

            // Populate the object properties 
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }
}
