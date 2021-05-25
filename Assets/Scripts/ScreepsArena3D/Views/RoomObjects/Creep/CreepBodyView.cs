using Assets.Scripts.ScreepsArena3D;
using Assets.Scripts.ScreepsArenaApi.Responses;
using UnityEngine;

namespace Assets.Scripts.ScreepsArena3D.Views.RoomObjects.Creep
{
    public class CreepBodyView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer _rend = default;
        private ReplayChunkRoomObject _creep; // TODO: interfaces
        private Texture2D _texture;

        public void Init()
        {
        }

        public void Load(ReplayChunkRoomObject roomObject)
        {
            if (_texture == null)
            {
                InitTexture();
            }
            _creep = roomObject/* as Creep*/;
            UpdateView(roomObject);
        }

        private void InitTexture()
        {
            _texture = new Texture2D(50, 1); // This assumes a max of 50 body parts
            _texture.filterMode = FilterMode.Point;
            //_rend.material.mainTexture = _texture;
            //_rend.material.SetTexture("_MainTex", _texture);
            // _rend.material.SetTexture("_BaseColorMap", _texture);
            _rend?.material.SetTexture("BodyMap", _texture);

        }

        public void Tick(ReplayChunkRoomObject data)
        {
            var bodyObj = data.body; // Do we receive delta bodies?
            if (bodyObj == null)
                return;

            UpdateView(data);
        }

        public void Unload()
        {
        }

        private void UpdateView(ReplayChunkRoomObject data)
        {
            var frontIndex = 0;
            for (var i = 0; i < PartCount("ranged_attack"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.RangedAttack);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("attack"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Attack);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("heal"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Heal);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("work"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Work);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("claim"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Claim);
                frontIndex++;
            }

            var backIndex = 0;
            for (; backIndex < PartCount("move"); backIndex++)
            {
                _texture.SetPixel(49 - backIndex, 0, Constants.CreepBodyPartColors.Move);
            }

            var toughAlpha = Mathf.Min(PartCount("tough") / 10f, 1);
            var toughColor = new Color(1, 1, 1, toughAlpha);
            for (; frontIndex < 49 - (backIndex - 1); frontIndex++)
            {
                _texture.SetPixel(frontIndex, 0, toughColor);
            }
            _texture.Apply();
        }

        private int PartCount(string type)
        {
            var count = 0;
            foreach (var part in _creep.body)
            {
                if (part.type == type && part.hits > 0)
                {
                    count++;
                }
            }
            return count;
        }
    }
}