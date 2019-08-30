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
        private Transform m_Light = null;
        [SerializeField]
        private ControlMode _InitialControlMode = ControlMode.NoControl;

        private EntityManager m_EntityManager;
        private RenderSystem m_TileRenderSystem;
        private TileSystem m_TileSystem;
        private WorldSystem m_WorldSystem;
        private ControlSystem m_ControlSystem;
        public ControlSystem ControlSystem { get => m_ControlSystem; set => m_ControlSystem = value; }

        void Start()
        {
            RenderSystem.TileResources = m_TileResources;
            TileSystem.TileScale = 2;
            TileSystem.TimeStep = 0.1f;
            TileSystem.Light = m_Light;
            ControlSystem.ControlMode = _InitialControlMode;
            m_EntityManager = World.Active.EntityManager;
            m_TileRenderSystem = World.Active.GetOrCreateSystem<RenderSystem>();
            m_TileRenderSystem.Init();
            m_TileSystem = World.Active.GetOrCreateSystem<TileSystem>();
            m_TileSystem.Init();
            m_WorldSystem = World.Active.GetOrCreateSystem<WorldSystem>();
            m_WorldSystem.Init();
            m_ControlSystem = new ControlSystem();
            ControlSystem.ControlMode = _InitialControlMode;
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) ControlSystem.ControlMode = ControlMode.NoControl;
            if (Input.GetKeyDown(KeyCode.F2)) ControlSystem.ControlMode = ControlMode.Menu;
            if (Input.GetKeyDown(KeyCode.F3)) ControlSystem.ControlMode = ControlMode.InGame;
            if (Input.GetKeyDown(KeyCode.F4)) ControlSystem.ControlMode = ControlMode.MapEditor;
        }
    }
}
