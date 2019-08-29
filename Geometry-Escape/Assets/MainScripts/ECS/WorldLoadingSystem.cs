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
        private static EntityArchetype _TileEntityArchetype;
        private int _MaterialAmount;
        private static NativeQueue<TileInfo> _TileCreationQueue;
        private static NativeQueue<Entity> _TileDestructionQueue;
        #endregion

        #region Public
        private int _TotalTileAmount;
        public int TotalTileAmount { get => _TotalTileAmount; }
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
            _TileCreationQueue = new NativeQueue<TileInfo>(Allocator.Persistent);
            _TileDestructionQueue = new NativeQueue<Entity>(Allocator.Persistent);
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
            if (_TileCreationQueue.IsCreated) _TileCreationQueue.Dispose();
            if (_TileDestructionQueue.IsCreated) _TileDestructionQueue.Dispose();
        }
        #endregion

        #region Methods

        public static void AddTile(int materialIndex, Coordinate initialCoordinate = default, TileType tileType = TileType.Normal)
        {
            _TileCreationQueue.Enqueue(new TileInfo
            {
                MaterialIndex = materialIndex,
                Coordinate = initialCoordinate,
                TileType = tileType
            });
        }

        public static void DeleteTile(Entity tileEntity)
        {
            _TileDestructionQueue.Enqueue(tileEntity);
        }

        #endregion

        #region Jobs
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_TileCreationQueue.Count != 0)
            {
                int count = _TileCreationQueue.Count;
                for(int i = 0; i < 100 && i < count; i++)
                {
                    var tileInfo = _TileCreationQueue.Dequeue();
                    CreateTile(tileInfo);
                }
            }
            if (_TileDestructionQueue.Count != 0)
            {
                int count = _TileDestructionQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var tileEntity = _TileDestructionQueue.Dequeue();
                    DestroyTile(tileEntity);
                }
            }
            return inputDeps;
        }

        private void DestroyTile(Entity tileEntity)
        {
            EntityManager.DestroyEntity(tileEntity);
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
