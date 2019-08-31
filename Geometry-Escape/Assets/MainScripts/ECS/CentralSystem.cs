using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
namespace GeometryEscape
{
    /// <summary>
    /// The system that control all other systems.
    /// </summary>
    public class CentralSystem : JobComponentSystem
    {
        #region Public
        private static RenderSystem m_RenderSystem;
        private static TileSystem m_TileSystem;
        private static WorldSystem m_WorldSystem;
        private static ControlSystem m_ControlSystem;
        private static LightResources m_LightResources;
        private static TileResources m_TileResources;
        public static RenderSystem RenderSystem { get => m_RenderSystem; set => m_RenderSystem = value; }
        public static TileSystem TileSystem { get => m_TileSystem; set => m_TileSystem = value; }
        public static WorldSystem WorldSystem { get => m_WorldSystem; set => m_WorldSystem = value; }
        public static ControlSystem ControlSystem { get => m_ControlSystem; set => m_ControlSystem = value; }
        public static LightResources LightResources { get => m_LightResources; set => m_LightResources = value; }
        public static TileResources TileResources { get => m_TileResources; set => m_TileResources = value; }

        #endregion
        protected override void OnCreate()
        {
            Init();
        }

        public void Init()
        {
            m_LightResources = Resources.Load<LightResources>("ScriptableObjects/LightResources");
            m_TileResources = Resources.Load<TileResources>("ScriptableObjects/TileResources");
            TileSystem.TileScale = 2;
            TileSystem.TimeStep = 0.1f;
            RenderSystem = World.Active.GetOrCreateSystem<RenderSystem>();
            RenderSystem.Init();
            TileSystem = World.Active.GetOrCreateSystem<TileSystem>();
            TileSystem.Init();
            WorldSystem = World.Active.GetOrCreateSystem<WorldSystem>();
            WorldSystem.Init();
            ControlSystem = new ControlSystem();
            ControlSystem.ControlMode = ControlMode.InGame;

            int count = 100;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    int index = i * count + j;
                    WorldSystem.AddTile(index % 3, new Coordinate { X = i, Y = j, Z = 0 });
                }
            }
        }

        public void ShutDown()
        {
            m_RenderSystem.ShutDown();
            m_TileSystem.ShutDown();
            m_WorldSystem.ShutDown();
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}