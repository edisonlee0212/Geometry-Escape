using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace GeometryEscape {
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private TileMeshAndMaterials m_TileMaterials = null;

        private EntityManager m_EntityManager;
        
        private TileRenderSystem m_TileRenderSystem;
        private TileSystem m_TileSystem;

        void Start()
        {
            TileRenderSystem.MaterialAmount = m_TileMaterials.GetMaterialAmount();
            TileSystem.TileScale = 2;
            TileSystem.TimeStep = 1f;
            m_EntityManager = World.Active.EntityManager;
            m_TileRenderSystem = World.Active.GetOrCreateSystem<TileRenderSystem>();
            m_TileRenderSystem.TileMesh = m_TileMaterials.TileMesh;
            m_TileRenderSystem.Materials = m_TileMaterials.Materials;
            m_TileRenderSystem.MaxSingleMaterialTileAmount = 1024;
            m_TileRenderSystem.Camera = Camera.main;
            m_TileRenderSystem.Init();

            m_TileSystem = World.Active.GetOrCreateSystem<TileSystem>();
            
            m_TileSystem.Init();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int index = i * 10 + j;
                    m_TileSystem.AddTile(0, new Coordinate { X = i, Y = j, Z = 0 });
                }
            }
        }

        void Update()
        {

        }
    }
}
