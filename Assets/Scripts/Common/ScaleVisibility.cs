using System;
using Screeps3D.RoomObjects.Views;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class ScaleVisibility : BaseVisibility
    {
        protected virtual void Scale(float amount)
        {
            transform.localScale = Vector3.one * amount;
        }

        protected override void Modify(float amount)
        {
            Scale(amount);
        }
    }
}