using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace GeometryEscape
{
    /// <summary>
    /// The system that control all tiles.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class TileSystem : JobComponentSystem
    {
        #region Private
        #endregion

        #region Public
        private static bool _Moving, _Zooming;
        private static float3 _CurrentCenterPosition;
        private static float _CurrentZoomFactor;
        private static float _Timer;
        private static int _Counter;
        private static float _TimeStep;
        private static float _TileScale;
        private static Transform m_Light;
        private static NativeArray<Entity> _CenterEntity;
        public static float TileScale { get => _TileScale; set => _TileScale = value; }
        public static float Timer { get => _Timer; set => _Timer = value; }
        public static int Counter { get => _Counter; set => _Counter = value; }
        public static float TimeStep { get => _TimeStep; set => _TimeStep = value; }
        public static float3 CurrentCenterPosition { get => _CurrentCenterPosition; set => _CurrentCenterPosition = value; }
        public static float CurrentZoomFactor { get => _CurrentZoomFactor; set => _CurrentZoomFactor = value; }
        public static Entity CenterEntity { get => _CenterEntity[0]; set => _CenterEntity[0] = value; }
        public static bool Moving { get => _Moving; set => _Moving = value; }
        public static bool Zooming { get => _Zooming; set => _Zooming = value; }
        public static Transform Light { get => m_Light; set => m_Light = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {

        }

        public void Init()
        {
            ShutDown();
            _CenterEntity = new NativeArray<Entity>(1, Allocator.Persistent);
            _CurrentZoomFactor = 1;
            Enabled = true;
        }

        public void ShutDown()
        {
            if (_CenterEntity.IsCreated) _CenterEntity.Dispose();
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        #region Methods



        #endregion

        #region Jobs

        [BurstCompile]
        struct PositionSelect : IJobForEachWithEntity<Translation, DefaultColor>
        {
            [ReadOnly] public float scale;
            [ReadOnly] public Vector3 position;
            [NativeDisableParallelForRestriction]
            [WriteOnly] public NativeArray<Entity> selectedEntity;
            public void Execute(Entity entity, int index, [ReadOnly] ref Translation c0, [WriteOnly] ref DefaultColor c1)
            {
                if (Mathf.Abs(c0.Value.x - position.x) < scale && Mathf.Abs(c0.Value.y - position.y) < scale / 4)
                {
                    selectedEntity[0] = entity;
                    c1.Color = new float4(1);
                }
            }
        }

        [BurstCompile]
        struct CalculateTileLocalToWorld : IJobForEach<Coordinate, Scale, Translation, Rotation>
        {
            [ReadOnly] public float scale;
            [ReadOnly] public float3 centerPosition;
            public void Execute([ReadOnly] ref Coordinate c0, [WriteOnly] ref Scale c1, [WriteOnly] ref Translation c2, [WriteOnly] ref Rotation c3)
            {
                var coordinate = c0;
                c1.Value = scale;
                c2.Value = new float3((coordinate.X + centerPosition.x) * scale, (coordinate.Y + centerPosition.y) * scale, (coordinate.Z + centerPosition.z) * scale);
                c3.Value = Quaternion.Euler(0, 0, coordinate.Direction);
            }
        }

        [BurstCompile]
        struct RotateTileTest1 : IJobForEach<Coordinate>
        {
            [ReadOnly] public int counter;
            public void Execute([WriteOnly] ref Coordinate c0)
            {
                if (c0.X % 2 == 0) c0.Direction = counter * 90;
            }
        }

        [BurstCompile]
        struct ChangeColorTest : IJobForEach<TileProperties, DefaultColor>
        {
            [ReadOnly] public int counter;
            public void Execute([ReadOnly] ref TileProperties c0, [WriteOnly] ref DefaultColor c1)
            {
                Vector4 color = default;
                int offset = c0.Index + counter;
                color.x = offset * 32 % 256 / 256f;
                color.y = (offset * 32 + 64) % 256 / 256f;
                color.z = (offset * 32 + 128) % 256 / 256f;
                color.w = 1;
                c1.Color = color;
            }
        }

        [BurstCompile]
        struct ChangeTextureInfoTest : IJobForEach<TextureIndex, TextureMaxIndex, TileProperties>
        {
            [ReadOnly] public int counter;
            public void Execute([WriteOnly] ref TextureIndex c0, [ReadOnly] ref TextureMaxIndex c1, [ReadOnly] ref TileProperties c2)
            {
                if (c2.MaterialIndex == 2) c0.Value = counter % c1.Value;
                else c0.Value = 0;
            }
        }
        #endregion

        protected JobHandle OnFixedUpdate(JobHandle inputDeps)
        {
            inputDeps = new RotateTileTest1
            {
                counter = Counter / 10,
            }.Schedule(this, inputDeps);
            inputDeps = new ChangeColorTest
            {
                counter = Counter / 10,
            }.Schedule(this, inputDeps);

            inputDeps = new ChangeTextureInfoTest
            {
                counter = Counter,
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            #region Time
            _Timer += Time.deltaTime;
            if (_Timer >= TimeStep)
            {
                _Counter += (int)(_Timer / _TimeStep);
                _Timer = 0;
                OnFixedUpdate(inputDeps);
            }
            #endregion

            #region InputSystem

            if (_Moving) OnMoving();
            if (_Zooming) OnZooming();

            #endregion

            inputDeps = new CalculateTileLocalToWorld
            {
                scale = _TileScale / _CurrentZoomFactor,
                centerPosition = _CurrentCenterPosition
            }.Schedule(this, inputDeps);

            inputDeps.Complete();

            _CenterEntity[0] = Entity.Null;

            inputDeps = new PositionSelect
            {
                scale = _TileScale / _CurrentZoomFactor,
                position = new Vector3(0, 0, 0),
                selectedEntity = _CenterEntity
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        #region Variables for InputSystem
        
        private static float _MovementTimer, _ZoomingTimer;
        private static float _PreviousZoomFactor, _TargetZoomFactor;
        private static Vector3 _PreviousCenterPosition, _TargetCenterPosition;
        #endregion

        private void OnMoving()
        {
            _MovementTimer += Time.deltaTime;
            if (_MovementTimer < 0.1f)
            {
                _CurrentCenterPosition = Vector3.Lerp(_PreviousCenterPosition, _TargetCenterPosition, _MovementTimer / 0.1f);
            }
            else
            {
                _Moving = false;
                _CurrentCenterPosition = _TargetCenterPosition;
            }

        }

        private void OnZooming()
        {
            _ZoomingTimer += Time.deltaTime;
            if (_ZoomingTimer <= 0.1f)
            {
                _CurrentZoomFactor = (_TargetZoomFactor * _ZoomingTimer + _PreviousZoomFactor * (0.1f - _ZoomingTimer)) / 0.1f;
            }
            else
            {
                _Zooming = false;
                _CurrentZoomFactor = _TargetZoomFactor;
            }
            Vector3 position = m_Light.position;
            position.z = -5 / _CurrentZoomFactor;
            m_Light.position = position;
        }

        public static void Zoom(float direction)
        {
            if (!_Zooming)
            {
                //Debug.Log(direction);
                if (direction != 0)
                {
                    _Zooming = true;
                    _ZoomingTimer = 0;
                    _PreviousZoomFactor = _CurrentZoomFactor;
                    if (direction > 0 && _CurrentZoomFactor < 2f)
                    {
                        _TargetZoomFactor = _PreviousZoomFactor;
                        _TargetZoomFactor += 0.5f;
                    }
                    else if (direction < 0 && _CurrentZoomFactor > 0.5f)
                    {
                        _TargetZoomFactor = _PreviousZoomFactor;
                        _TargetZoomFactor -= 0.5f;
                    }
                }
            }
        }

        public static void Move(Vector2 direction)
        {
            if (!_Moving)
            {
                if (direction != Vector2.zero && direction.x * direction.y == 0)
                {
                    _Moving = true;
                    _MovementTimer = 0;
                    _PreviousCenterPosition = _CurrentCenterPosition;
                    _TargetCenterPosition = _PreviousCenterPosition;
                    if (direction.x > 0)
                    {
                        //Debug.Log("Move right.");
                        _TargetCenterPosition.x -= 1;
                    }
                    else if (direction.x < 0)
                    {
                        //Debug.Log("Move left.");
                        _TargetCenterPosition.x += 1;
                    }
                    else if (direction.y > 0)
                    {
                        //Debug.Log("Move down.");
                        _TargetCenterPosition.y -= 1;
                    }
                    else if (direction.y < 0)
                    {
                        //Debug.Log("Move up.");
                        _TargetCenterPosition.y += 1;
                    }
                }
            }

        }
    }
}
