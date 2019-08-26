using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace GeometryEscape
{
    public class TileSystem : JobComponentSystem
    {
        #region Private
        private EntityArchetype _TileEntityArchetype;
        private RenderMaterial[] _RenderMaterials;
        private int _MaterialAmount;
        #endregion

        #region Public
        private static int _TotalTileAmount;
        public static int TotalTileAmount { get => _TotalTileAmount; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            _TileEntityArchetype = EntityManager.CreateArchetype(
                typeof(RenderMaterial),
                typeof(Coordinate),
                typeof(CustomLocalToWorld),
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
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps; 
        }
    }
}
