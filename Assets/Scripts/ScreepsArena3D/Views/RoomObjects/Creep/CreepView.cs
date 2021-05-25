using Assets.Scripts.Common;
using Assets.Scripts.ScreepsArena3D;
using Assets.Scripts.ScreepsArena3D.Views.RoomObjects;
using Assets.Scripts.ScreepsArenaApi.Responses;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class CreepView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer _badge = default;
        [SerializeField] private Renderer _creepCore = default;
        //[SerializeField] private Renderer _doritoCore = default;
        [SerializeField] private Transform _rotationRoot = default;
        [SerializeField] private Light _underLight = default;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private ReplayChunkRoomObject _creep; // TODO: ICreep or something
        private bool _dead;

        private Color32 _initialUnderlightColor;

        public Vector3 PrevPosition { get; protected set; }
        public Vector3 BumpPosition { get; private set; }
        public Quaternion Rotation { get; private set; }

        public Vector3? ActionTarget { get; set; }
        public RoomView _roomView { get; private set; }

        private void Awake()
        {
            _initialUnderlightColor = _underLight.color;
        }

        //private void setDorito(bool isDorito)
        //{
        //    if (isDorito)
        //    {
        //        _doritoCore.gameObject.SetActive(true);
        //        _creepCore.gameObject.SetActive(false);
        //        _underLight.color = Color.red;
        //    }
        //    else
        //    {
        //        _creepCore.gameObject.SetActive(true);
        //        _doritoCore.gameObject.SetActive(false);
        //        _underLight.color = _initialUnderlightColor;
        //    }
        //}


        public void Load(ReplayChunkRoomObject roomObject)
        {
            _creep = roomObject;

            if (_roomView.Badges.TryGetValue(roomObject.user, out var badge))
            {
                _badge.materials[0].SetTexture("EmissionTexture", badge);
                _badge.materials[0].SetFloat("EmissionStrength", .1f);
            }

            // TODO: owner, also set underlight color?
            //if (_creep?.Owner?.Badge == null)
            //{
            //    Debug.LogError("A creep with no owner?");
            //}
            //else
            //{
            //    _badge.materials[0].SetTexture("EmissionTexture", _creep?.Owner?.Badge);
            //    _badge.materials[0].SetFloat("EmissionStrength", .1f);
            //}


            _rotTarget = transform.rotation;
            _posTarget = PosUtility.Convert(roomObject.x, roomObject.y, 100); // TODO: We need a reference to the RoomView to acquire the size / position within the room

            ScaleCreepSize();
        }

        private void ScaleCreepSize()
        {
            var percentage = _creep.body.Length / 50f;
            _dead = false;
            if (percentage == 0)
            {
                _dead = true;
            }

            var minVisibility = 0.001f; /*to keep it visible and selectable*/
            var maxVisibility = 1f;

            // http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            float minimum = Mathf.Log(minVisibility);
            float maximum = Mathf.Log(maxVisibility);

            // Scale the visibility in such a way that a lot of the model is rendered above 25 body parts
            float current = Mathf.Log(percentage == 0 ? minVisibility : percentage);

            // Map range to visibility range
            var visibility = minVisibility + (maxVisibility - minVisibility) * ((current - minimum) / (maximum - minimum));

            // TODO: scale creep
            //_vis.SetVisibility(visibility, true);
        }

        public void Tick(ReplayChunkRoomObject data)
        {
            // TODO: update body part hits and other data?

            //_underLight.intensity = _creep.Hits / _creep.HitsMax * .1f;
            _underLight.intensity = data.hits / data.hitsMax * .1f;

            var pos = PosUtility.Convert(data.x, data.y, 100); // TODO: We need a reference to the RoomView to acquire the size / position within the room
            //var posDelta = _posTarget - RoomObject.Position;
            var posDelta = _posTarget - pos;

            if (posDelta.sqrMagnitude > .01)
            {
                _posTarget = pos;
            }

            ScaleCreepSize();

            AssignBumpPosition(data);
            AssignRotation(data);

            // TODO: move to Unload?
            //_dead = _dead || _creep.TTL == 1;
            //if (_dead)
            //{
            //    _underLight.gameObject.SetActive(false);
            //    _underLight = null;
            //}
        }


        private void AssignBumpPosition(ReplayChunkRoomObject data)
        {
            // TODO: handle bump positions once we have actions to bump towards
            //BumpPosition = default(Vector3);
            //foreach (var kvp in Constants.ContactActions)
            //{
            //    if (!kvp.Value)
            //        continue;
            //    var action = kvp.Key;
            //    if (!Actions.ContainsKey(action))
            //        continue;
            //    var actionData = Actions[action];
            //    if (actionData.IsNull)
            //        continue;
            //    BumpPosition = PosUtility.Convert(action.x, action.y, 100); // TODO: We need a reference to the RoomView to acquire the size / position within the room;
            //}
        }

        private void AssignRotation(ReplayChunkRoomObject data)
        {
            // TODO: perhaps we want to store the position in a property and update it on Tick

            var pos = PosUtility.Convert(data.x, data.y, 100); // TODO: We need a reference to the RoomView to acquire the size / position within the room

            Vector3 newForward = Vector3.zero;
            if (BumpPosition != default(Vector3))
                newForward = pos - BumpPosition;
            // checking for 0,0,0 so creeps won't look away from world center when initially placed
            if (PrevPosition != pos && PrevPosition != Vector3.zero)
                newForward = PrevPosition - pos;

            if (newForward != Vector3.zero)
                Rotation = Quaternion.LookRotation(newForward);
        }

        private void Update()
        {
            if (_creep == null)
                return;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _posTarget, ref _posRef, .5f);

            // TODO: this is set by CreepActionView
            //if (_creep.ActionTarget.HasValue)
            //{
            //    // creep does something, keep it rotated towards target
            //    Vector3 relativePos = _rotationRoot.position - (Vector3)_creep.ActionTarget;
            //    if (relativePos != Vector3.zero)
            //    {
            //        Quaternion tRotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //        _rotationRoot.rotation = tRotation;
            //    }
            //}
            //else
            //{
            // keep rotation towards move direction
            _rotationRoot.transform.rotation = Quaternion.Slerp(_rotationRoot.transform.rotation, this.Rotation,
                Time.deltaTime * 5);
            //}

            // TODO: CreepActionView with a say subcomponent or something.
            //if (Actions.ContainsKey("say") && !Actions["say"].IsNull)
            //    EffectsUtility.Speech(this, Actions["say"]["message"].str, Actions["say"]["isPublic"].b);

        }

        public void Init(RoomView roomView)
        {
            this._roomView = roomView;
        }

        public void Unload()
        {

        }


    }
}