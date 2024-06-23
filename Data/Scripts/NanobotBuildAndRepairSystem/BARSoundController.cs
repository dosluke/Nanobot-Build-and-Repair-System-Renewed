using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SpaceEquipmentLtd.NanobotBuildAndRepairSystem;
using System;
using VRageMath;

namespace Nanobot_Build_and_Repair_System_Renewed.Data.Scripts.NanobotBuildAndRepairSystem
{
    internal class BARSoundController
    {
        private float _SoundVolume = -1;
        private MyEntity3DSoundEmitter _SoundEmitter;
        private MyEntity3DSoundEmitter _SoundEmitterWorking;
        private Vector3D? _SoundEmitterWorkingPosition;
        private float? _SoundEmitterCustomMaxDistance = 30f;
        private static MySoundPair[] _Sounds = new[] { null, null, null, new MySoundPair("ToolLrgWeldMetal"), new MySoundPair("BlockModuleProductivity"), new MySoundPair("BaRUnable"), new MySoundPair("ToolLrgGrindMetal"), new MySoundPair("BlockModuleProductivity"), new MySoundPair("BaRUnable"), new MySoundPair("BaRUnable") };
        private static float[] _SoundLevels = new[] { 0f, 0f, 0f, 1f, 0.5f, 0.4f, 1f, 0.5f, 0.4f, 0.4f };

        internal void StopSFX()
        {
            if (_SoundEmitter != null)
            {
                _SoundEmitter.StopSound(false);
            }

            if (_SoundEmitterWorking != null)
            {
                _SoundEmitterWorking.StopSound(false);
                _SoundEmitterWorking.SetPosition(null); //Reset
                _SoundEmitterWorkingPosition = null;
            }
        }

        internal void SetWorkingSFX(WorkingState workingState, IMyShipWelder welder, float SoundVolume, SyncBlockState State)
        {
            var sound = _Sounds[(int)workingState];
            if (sound != null)
            {
                if (_SoundEmitter == null)
                {
                    _SoundEmitter = new MyEntity3DSoundEmitter((VRage.Game.Entity.MyEntity)welder);
                    _SoundEmitter.CustomMaxDistance = _SoundEmitterCustomMaxDistance;
                    _SoundEmitter.CustomVolume = _SoundLevels[(int)workingState] * SoundVolume;
                }
                if (_SoundEmitterWorking == null)
                {
                    _SoundEmitterWorking = new MyEntity3DSoundEmitter((VRage.Game.Entity.MyEntity)welder, true, 1f);
                    _SoundEmitterWorking.CustomMaxDistance = _SoundEmitterCustomMaxDistance;
                    _SoundEmitterWorking.CustomVolume = _SoundLevels[(int)workingState] * SoundVolume;
                    _SoundEmitterWorkingPosition = null;
                }

                if (_SoundEmitter != null)
                {
                    _SoundEmitter.StopSound(true);
                    _SoundEmitter.CustomVolume = _SoundLevels[(int)workingState] * SoundVolume;
                    _SoundEmitter.PlaySound(sound, true);
                }

                if (_SoundEmitterWorking != null)
                {
                    _SoundEmitterWorking.StopSound(true);
                    _SoundEmitterWorking.CustomVolume = _SoundLevels[(int)workingState] * SoundVolume;
                    _SoundEmitterWorking.SetPosition(null); //Reset
                    _SoundEmitterWorkingPosition = null;
                }
            }
            else
            {
                if (_SoundEmitter != null)
                {
                    _SoundEmitter.StopSound(true);
                }

                if (_SoundEmitterWorking != null)
                {
                    _SoundEmitterWorking.StopSound(true);
                    _SoundEmitterWorking.SetPosition(null); //Reset
                    _SoundEmitterWorkingPosition = null;
                }
            }

            UpdateWorkingSFXPosition(workingState, State, welder);
        }

        /// <summary>
        /// Set the position of the sound effects
        /// </summary>
        internal void UpdateWorkingSFXPosition(WorkingState workingState, SyncBlockState State, IMyShipWelder welder)
        {
            if (_SoundEmitterWorking == null) return;

            Vector3D position;

            if (State.CurrentWeldingBlock != null)
            {
                BoundingBoxD box;
                State.CurrentWeldingBlock.GetWorldBoundingBox(out box, false);
                position = box.Matrix.Translation;
            }
            else if (State.CurrentGrindingBlock != null)
            {
                BoundingBoxD box;
                State.CurrentGrindingBlock.GetWorldBoundingBox(out box, false);
                position = box.Matrix.Translation;
            }
            else
                position = welder.WorldMatrix.Translation;

            var sound = _Sounds[(int)workingState];

            if (sound == null)
                return;

            if (!_SoundEmitterWorking.IsPlaying || _SoundEmitterWorkingPosition == null || Math.Abs((_SoundEmitterWorkingPosition.Value - position).Length()) > 2)
            {
                _SoundEmitterWorking.SetPosition(position);
                _SoundEmitterWorkingPosition = position;
                _SoundEmitterWorking.PlaySound(sound, true);
            }
        }

        internal void UpdateSoundVolume(float soundVolume)
        {
            _SoundVolume = soundVolume;
        }

        internal void UpdateSettings(SyncModSettingsWelder welder)
        {
            if ((welder.AllowedEffects & VisualAndSoundEffects.WeldingSoundEffect) == 0)
                _Sounds[(int)WorkingState.Welding] = null;

            if ((welder.AllowedEffects & VisualAndSoundEffects.GrindingSoundEffect) == 0)
                _Sounds[(int)WorkingState.Grinding] = null;
        }
    }
}
