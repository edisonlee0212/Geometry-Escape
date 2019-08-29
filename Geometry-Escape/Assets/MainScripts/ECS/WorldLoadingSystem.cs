using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
namespace GeometryEscape
{
    public struct TileInfo
    {
        public int MaterialIndex;
        public Coordinate Coordinate;
        public TileType TileType;
    }
    /// <summary>
    /// The system manages the creation and destruction of all world entities.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class WorldSystem : JobComponentSystem
    {
        #region Private
        private EntityArchetype _TileEntityArchetype;
        private int _MaterialAmount;
        private NativeQueue<TileInfo> _TileQueue;
        #endregion

        #region Public
        private static int _TotalTileAmount;
        public static int TotalTileAmount { get => _TotalTileAmount; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            _TileEntityArchetype = EntityManager.CreateArchetype(
                typeof(RenderMaterialIndex),
                typeof(Coordinate),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(Unity.Transforms.LocalToWorld),
                typeof(TileProperties),
                typeof(DefaultColor),
                typeof(TextureIndex),
                typeof(TextureMaxIndex)
                );
            _TileQueue = new NativeQueue<TileInfo>(Allocator.Persistent);
        }

        public void Init()
        {
            ShutDown();
            _MaterialAmount = TileRenderSystem.MaterialAmount;
            _TotalTileAmount = 0;
        }

        public void ShutDown()
        {

        }

        protected override void OnDestroy()
        {
            if (_TileQueue.IsCreated) _TileQueue.Dispose();
        }
        #endregion

        #region Methods

        public void AddTile(int materialIndex, Coordinate initialCoordinate = default, TileType tileType = TileType.Normal)
        {
            _TileQueue.Enqueue(new TileInfo
            {
                MaterialIndex = materialIndex,
                Coordinate = initialCoordinate,
                TileType = tileType
            });
        }
        #endregion

        #region Jobs
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_TileQueue.Count != 0)
            {
                int count = _TileQueue.Count;
                for(int i = 0; i < 10 || i < count; i++)
                {
                    var tileInfo = _TileQueue.Dequeue();
                    CreateTile(tileInfo);
                }
            }
            return inputDeps;
        }

        private void CreateTile(TileInfo tileInfo)
        {
            var materialIndex = tileInfo.MaterialIndex;
            var initialCoordinate = tileInfo.Coordinate;
            var tileType = tileInfo.TileType;
            if (materialIndex < 0 || materialIndex >= _MaterialAmount)
            {
                Debug.LogError("AddTile: Wrong material index: " + materialIndex);
                return;
            }
            var color = new DefaultColor { };
            color.Color = Vector4.one;
            var textureInfo = new TextureIndex
            {
                Value = 1
            };
            var renderMaterialIndex = new RenderMaterialIndex
            {
                Value = materialIndex
            };
            var maxTextureIndex = new TextureMaxIndex
            {
                Value = 25
            };

            Entity instance = EntityManager.CreateEntity(_TileEntityArchetype);
            EntityManager.SetSharedComponentData(instance, renderMaterialIndex);
            EntityManager.SetComponentData(instance, initialCoordinate);
            EntityManager.SetComponentData(instance, color);
            EntityManager.SetComponentData(instance, textureInfo);
            EntityManager.SetComponentData(instance, maxTextureIndex);
            var properties = new TileProperties
            {
                Index = TotalTileAmount,
                TileType = tileType,
                MaterialIndex = materialIndex
            };
            _TotalTileAmount++;
            EntityManager.SetComponentData(instance, properties);
        }
    }
}
