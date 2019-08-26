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
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class TileTransformationSystem : JobComponentSystem
    {
        #region Private
        #endregion

        #region Public
        #endregion

        #region Managers
        #endregion

        #region Methods
        #endregion

        #region Jobs
        [BurstCompile]
        struct CalculateTileLocalToWorld : IJobForEach<CustomLocalToWorld, Coordinate>
        {
            [ReadOnly] public float scale;
            public void Execute([WriteOnly]ref CustomLocalToWorld c0, [ReadOnly] ref Coordinate c1)
            {
                var localToWorld = new CustomLocalToWorld
                {
                    Position = new float3(c1.X, c1.Y, c1.Z),
                    Scale = new float3(scale),
                };
                localToWorld.Value.c3.w = 1;
                c0 = localToWorld;
            }
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps = new CalculateTileLocalToWorld
            {
                scale = 1
            }.Schedule(this, inputDeps);
            return inputDeps;
        }
    }
}
