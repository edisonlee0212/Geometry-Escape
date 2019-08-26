using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace GeometryEscape {
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private TileMeshAndMaterials m_TileMaterials;

        private EntityManager m_EntityManager;
        private TileRenderSystem m_TileRenderSystem;
        void Start()
        {
            m_EntityManager = World.Active.EntityManager;
            m_TileRenderSystem = World.Active.GetOrCreateSystem<TileRenderSystem>();
            m_TileRenderSystem.TileMesh = m_TileMaterials.TileMesh;
            m_TileRenderSystem.Materials = m_TileMaterials.Materials;
            m_TileRenderSystem.Test();
        }

        void Update()
        {

        }
    }
}
