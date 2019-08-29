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
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class TileSystem : JobComponentSystem
    {
        #region Private
        #endregion

        #region Public
        private static Controls _InputSystem;
        private static float3 _CurrentCenterPosition;
        private static float _CurrentZoomFactor;
        private static float _Timer;
        private static int _Counter;
        private static float _TimeStep;
        private static float _TileScale;
        private static NativeArray<Entity> _PositionSelectedEntity;
        public static float TileScale { get => _TileScale; set => _TileScale = value; }
        public static float Timer { get => _Timer; set => _Timer = value; }
        public static int Counter { get => _Counter; set => _Counter = value; }
        public static float TimeStep { get => _TimeStep; set => _TimeStep = value; }
        public static float3 CurrentCenterPosition { get => _CurrentCenterPosition; set => _CurrentCenterPosition = value; }
        public static Controls InputSystem { get => _InputSystem; set => _InputSystem = value; }
        public static float CurrentZoomFactor { get => _CurrentZoomFactor; set => _CurrentZoomFactor = value; }
        public static NativeArray<Entity> PositionSelectedEntity { get => _PositionSelectedEntity; set => _PositionSelectedEntity = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {

        }

        public void Init()
        {
            ShutDown();
            _PositionSelectedEntity = new NativeArray<Entity>(1, Allocator.Persistent);
            _CurrentZoomFactor = 1;
            Enabled = true;
        }

        public void ShutDown()
        {
            if (_PositionSelectedEntity.IsCreated) _PositionSelectedEntity.Dispose();
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
                if (Mathf.Abs(c0.Value.x - position.x) < scale && Mathf.Abs(c0.Value.y - position.y) < scale)
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
            else InputSystem.Player.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
            if (_Zooming) OnZooming();
            else InputSystem.Player.Zoom.performed += ctx => Zoom(ctx.ReadValue<float>());

            #endregion

            inputDeps = new CalculateTileLocalToWorld
            {
                scale = _TileScale / _CurrentZoomFactor,
                centerPosition = _CurrentCenterPosition
            }.Schedule(this, inputDeps);

            inputDeps.Complete();



            inputDeps = new PositionSelect
            {
                scale = _TileScale / _CurrentZoomFactor,
                position = new Vector3(0, 0, 0),
                selectedEntity = _PositionSelectedEntity
            }.Schedule(this, inputDeps);
            inputDeps.Complete();

            if (Input.GetKeyDown(KeyCode.Delete)) WorldSystem.DeleteTile(_PositionSelectedEntity[0]);
            if (!_Moving && _PositionSelectedEntity[0] != null && Input.GetKeyDown(KeyCode.Insert)) WorldSystem.AddTile(0, new Coordinate
            {
                X = (int)-_CurrentCenterPosition.x,
                Y = (int)-_CurrentCenterPosition.y,
                Z = (int)-_CurrentCenterPosition.z,
            });
            return inputDeps;
        }

        #region Variables for InputSystem
        private bool _Moving, _Zooming;
        private float _MovementTimer, _ZoomingTimer;
        private float _PreviousZoomFactor, _TargetZoomFactor;
        private Vector3 _PreviousCenterPosition, _TargetCenterPosition;
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
            if (_ZoomingTimer < 0.1f)
            {
                _CurrentZoomFactor = (_TargetZoomFactor * _ZoomingTimer + _PreviousZoomFactor * (0.1f - _ZoomingTimer)) / 0.1f;
            }
            else
            {
                _Zooming = false;
                _CurrentZoomFactor = _TargetZoomFactor;
            }

        }

        private void Zoom(float direction)
        {
            if (!_Zooming)
            {
                Debug.Log(direction);
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
                    //Debug.Log("TargetZoomFactor: " + _TargetZoomFactor);
                }
            }
        }

        private void Move(Vector2 direction)
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
