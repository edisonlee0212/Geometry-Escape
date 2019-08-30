using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace GeometryEscape {
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private TileResources m_TileResources = null;
        [SerializeField]
        private ControlMode _InitialControlMode;

        private EntityManager m_EntityManager;
        private TileRenderSystem m_TileRenderSystem;
        private TileSystem m_TileSystem;
        private WorldSystem m_WorldSystem;
        private ControlSystem m_ControlSystem;
        public ControlSystem ControlSystem { get => m_ControlSystem; set => m_ControlSystem = value; }

        void Start()
        {
            TileRenderSystem.TileResources = m_TileResources;
            TileSystem.TileScale = 2;
            TileSystem.TimeStep = 0.1f;
            ControlSystem.ControlMode = _InitialControlMode;
            m_EntityManager = World.Active.EntityManager;
            m_TileRenderSystem = World.Active.GetOrCreateSystem<TileRenderSystem>();
            m_TileRenderSystem.Init();
            m_TileSystem = World.Active.GetOrCreateSystem<TileSystem>();
            m_TileSystem.Init();
            m_WorldSystem = World.Active.GetOrCreateSystem<WorldSystem>();
            m_WorldSystem.Init();
            m_ControlSystem = new ControlSystem();
            ControlSystem.ControlMode = _InitialControlMode;
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    int index = i * count + j;
                    WorldSystem.AddTile(index % 3, new Coordinate { X = i, Y = j, Z = 0 });
                }
            }
        }

        void Update()
        {

        }
    }
}
