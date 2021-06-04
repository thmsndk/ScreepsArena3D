using Assets.Scripts.ScreepsArenaApi.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class PosUtility
    {
        public static Vector3 Convert(int x, int y, int size)
        {
            return new Vector3(x, 0f, size - 1 - y);
        }

        // TODO: IRoomPosition
        public static Vector3 Convert(ReplayChunkAction action, int size)
        {
            return new Vector3(action.x, 0f, size - 1 - action.y);
        }

        // TODO: one that can translate roomobject to a pos.
    }
}
