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
        private EntityArchetype _TileEntityArchetype;
        private RenderMaterial[] _RenderMaterials;
        private int _MaterialAmount;
        #endregion

        #region Public
        private static float _Timer;
        private static int _Counter;
        private static float _TimeStep;
        private static float _TileScale;
        private static int _TotalTileAmount;
        public static int TotalTileAmount { get => _TotalTileAmount; }
        public static float TileScale { get => _TileScale; set => _TileScale = value; }
        public static float Timer { get => _Timer; set => _Timer = value; }
        public static int Counter { get => _Counter; set => _Counter = value; }
        public static float TimeStep { get => _TimeStep; set => _TimeStep = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            _TileEntityArchetype = EntityManager.CreateArchetype(
                typeof(RenderMaterial),
                typeof(Coordinate),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(Unity.Transforms.LocalToWorld),
                typeof(TileProperties),
                typeof(DefaultColor)
                );
        }

        public void Init()
        {
            ShutDown();
            _MaterialAmount = TileRenderSystem.MaterialAmount;
            _RenderMaterials = new RenderMaterial[_MaterialAmount];
            _TotalTileAmount = 0;
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        #region Methods

        public void AddTile(int materialIndex, Coordinate initialCoordinate = default, TileType tileType = TileType.Normal)
        {
            if(materialIndex < 0 || materialIndex >= _MaterialAmount)
            {
                Debug.LogError("AddTile: Wrong material index: " + materialIndex);
                return;
            }
            var color = new DefaultColor { };
            color.Color = Vector4.one;
            Entity instance = EntityManager.CreateEntity(_TileEntityArchetype);
            EntityManager.SetSharedComponentData(instance, _RenderMaterials[materialIndex]);
            EntityManager.SetComponentData(instance, initialCoordinate);
            EntityManager.SetComponentData(instance, color);
            var properties = new TileProperties
            {
                Index = TotalTileAmount,
                TileType = tileType
            };
            EntityManager.SetComponentData(instance, properties);
        }

        #endregion

        #region Jobs
        [BurstCompile]
        struct CalculateTileLocalToWorld : IJobForEach<Coordinate, Scale, Translation, Rotation>
        {
            [ReadOnly] public float scale;
            public void Execute([ReadOnly] ref Coordinate c0, [WriteOnly] ref Scale c1, [WriteOnly] ref Translation c2, [WriteOnly] ref Rotation c3)
            {
                c1.Value = scale;
                c2.Value = new float3(c0.X * scale, c0.Y * scale, c0.Z * scale);
                c3.Value = Quaternion.Euler(0, 0, c0.Direction);
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
        struct RotateTileTest2 : IJobForEach<Coordinate>
        {
            [ReadOnly] public float timer;
            [ReadOnly] public float timeStep;
            public void Execute([WriteOnly] ref Coordinate c0)
            {
                if (c0.X % 2 == 1) c0.Direction = (int)(timer / timeStep * 360);
            }
        }

        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Timer += Time.deltaTime;
            if (Timer >= TimeStep)
            {
                Timer = 0;
                ++Counter;
                inputDeps = new RotateTileTest1
                {
                    counter = Counter,
                }.Schedule(this, inputDeps);
                inputDeps.Complete();
            }

            inputDeps = new RotateTileTest2
            {
                timer = Timer,
                timeStep = TimeStep
            }.Schedule(this, inputDeps);

            inputDeps = new CalculateTileLocalToWorld
            {
                scale = _TileScale
            }.Schedule(this, inputDeps);
            return inputDeps;
        }
    }
}
