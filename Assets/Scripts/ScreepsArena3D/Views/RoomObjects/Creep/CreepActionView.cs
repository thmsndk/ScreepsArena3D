using Assets.Scripts.Common;
using Assets.Scripts.ScreepsArena3D.Effects;
using Assets.Scripts.ScreepsArenaApi.Responses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ScreepsArena3D.Views.RoomObjects.Creep
{
    public class CreepActionView : MonoBehaviour, IObjectViewComponent
    {

        [SerializeField] private Transform _creepRoot = default;
        //private ReplayChunkRoomObjectCreep _creep;
        private Vector3 _bumpRef;
        private bool _shouldBump;
        private bool _bumping;
        private bool _actionEffect;
        private bool _animating;
        private RoomView _roomView;
        private RoomObjectView _view;

        // possible actions from Delta JSONdata: 
        // attack               bump + sparks
        // attacked             n/a
        // heal                 n/a
        // rangedHeal           beam 
        // healed               aura (green)
        // rangedAttack         beam
        // rangedMassAttack     aura
        // harvest              bump + sparks
        // repair               beam
        // build                beam
        // upgradeController    beam
        // reserveController    bump + aura(violet)
        // say                  text

        public void Init(RoomView roomView, RoomObjectView view)
        {
            _roomView = roomView;
            _view = view;
        }

        public void Load(ReplayChunkRoomObject roomObject)
        {
            //_creep = roomObject as ReplayChunkRoomObjectCreep;
        }

        public void Tick(ReplayChunkRoomObject data)
        {
            if (data.actionLog == null || !data.actionLog.Any())
            {
                return;
            }

            var tickCreep = data as ReplayChunkRoomObjectCreep;

            _shouldBump = false;
            _bumping = true;
            _animating = true;
            _actionEffect = false;
            Color beamColor = Color.black;

            List<string> aKeys = tickCreep.actionLog.Keys.ToList();
            for (int i = 0; i < aKeys.Count; i++)
            {
                string k = aKeys[i];
                //if (_creep.actionLog[k].IsNull)
                //{
                //    continue;
                //}
                var action = tickCreep.actionLog[k];
                var target = PosUtility.Convert(action, _roomView.size);

                switch (k)
                {
                    case "rangedMassAttack":
                        // TODO:
                        //EffectsUtility.ElectricExplosion(_creep as RoomObject);
                        break;
                    // Bump Stuff
                    case "attack":
                        _shouldBump = true;
                        doParticles(target, k, new Color32(255, 45, 0, 255));
                        break;
                    case "harvest":
                        _shouldBump = true;
                        doParticles(target, k, new Color32(255, 111, 111, 255));
                        break;
                    case "reserveController":
                        _shouldBump = true;
                        doParticles(target, k, new Color32(255, 111, 111, 255));
                        break;
                    case "attackController":
                        _shouldBump = true;
                        doParticles(target, k, new Color32(255, 111, 111, 255));
                        break;
                    // Healed - just aura
                    case "healed":
                        doParticles(target, k, new Color32(65, 140, 65, 255));
                        break;
                    // Beam Stuff
                    case "rangedAttack":
                        doArcBeam(target, Color.blue);
                        // doBeam(_creep.actionLog[k] , new BeamConfig(Color.blue, 0.3f, 0.3f));
                        doParticles(target, k, new Color32(0, 45, 255, 255));
                        break;
                    case "rangedHeal":
                        // doBeam(_creep.actionLog[k], new BeamConfig(Color.green, 0.3f, 0.3f)); 
                        doArcBeam(target, Color.green);
                        break;
                    case "repair":
                        // doBeam(_creep.actionLog[k], new BeamConfig(Color.yellow, 0.3f, 0.3f));
                        doArcBeam(target, Color.yellow);
                        break;
                    case "build":
                        // doBeam(_creep.actionLog[k], new BeamConfig(Color.yellow, 0.3f, 0.3f));
                        doArcBeam(target, Color.yellow);
                        break;
                    case "upgradeController":
                        // doBeam(_creep.actionLog[k], new BeamConfig(Color.yellow, 0.3f, 1f));
                        doArcBeam(target, Color.yellow);
                        break;
                }
            }
        }

        public void Unload()
        {
            //_creep = null;
        }

        private void doBump()
        {

            //var bumpCreep = _creep as IBump;

            var localBase = Vector3.zero;
            var targetLocalPos = localBase;
            var speed = .2f;
            if (_bumping)
            {
                // either half forward (creep is having -z as forward - reasons...)
                targetLocalPos = Vector3.back * .5f;
                speed = .1f;
            }
            // creep IS rotated towards source/action so we just need to go forward via Z, and do not care about X axis
            targetLocalPos.x = 0f;
            targetLocalPos.y = 0.3f;
            _creepRoot.transform.localPosition =
                Vector3.SmoothDamp(_creepRoot.transform.localPosition, targetLocalPos, ref _bumpRef, speed);
            var sqrMag = (_creepRoot.transform.localPosition - targetLocalPos).sqrMagnitude;

            // if(_bumping && sqrMag < .005f && !_actionEffect) {
            //     EffectsUtility.Attack(_creep as RoomObject, bumpCreep.BumpPosition);
            //     _actionEffect = true;
            // }

            if (sqrMag < .0001f)
            {
                if (_bumping)
                {
                    _bumping = false;
                    // _actionEffect = false;
                }
                else
                {
                    _animating = false;
                }
            }
        }

        //private void doBeam(JSONObject target, BeamConfig beamCfg)
        //{
        //    EffectsUtility.Beam(_creep as RoomObject, target, beamCfg);
        //}

        private void doArcBeam(Vector3 target, Color beamColor)
        {
            EffectsUtility.ArcBeam(_roomView.transform, _creepRoot.transform, target, new BeamConfig(beamColor, 0f, 0f));
        }

        private void doParticles(Vector3 target, string particleType, Color32 auraColor)
        {
            switch (particleType)
            {
                case "rangedAttack":
                    EffectsUtility.RangedAttackHit(_roomView.transform, target);
                    break;
                case "attack":
                    EffectsUtility.Attack(_roomView.transform, (_creepRoot.transform.position + target) / 2);
                    break;
                case "heal":
                    // no effect on creep that heals, effect applied to healed creep
                    break;
                case "healed":
                    EffectsUtility.Heal(_view.transform);
                    break;
                case "reserveController":
                    //EffectsUtility.Reserve(target);
                    break;
                case "harvest":
                    //EffectsUtility.Harvest(target);
                    break;
            }
        }


        private void Update()
        {
            //if (_creep == null || !_animating)
            //    return;

            if (_shouldBump)
            {
                doBump();
            }
        }
    }
}