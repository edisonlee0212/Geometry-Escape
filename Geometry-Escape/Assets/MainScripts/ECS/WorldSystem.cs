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
        private static bool _AddingTiles;
        private static bool _RemovingTiles;
        private static int _TotalTileAmount;
        public static int TotalTileAmount { get => _TotalTileAmount; }
        public static bool AddingTiles { get => _AddingTiles; set => _AddingTiles = value; }
        public static bool RemovingTiles { get => _RemovingTiles; set => _RemovingTiles = value; }
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
            
            Enabled = false;
        }

        public void Init()
        {
            ShutDown();
            _TileCreationQueue = new NativeQueue<TileInfo>(Allocator.Persistent);
            _TileDestructionQueue = new NativeQueue<Entity>(Allocator.Persistent);
            _MaterialAmount = TileRenderSystem.MaterialAmount;
            _TotalTileAmount = 0;
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_TileCreationQueue.IsCreated) _TileCreationQueue.Dispose();
            if (_TileDestructionQueue.IsCreated) _TileDestructionQueue.Dispose();
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods

        public static void AddCenterTile()
        {
            if (!_AddingTiles && !TileSystem.Moving && TileSystem.CenterEntity == Entity.Null)
            {
                AddTile(0, new Coordinate
                {
                    X = (int)-TileSystem.CurrentCenterPosition.x,
                    Y = (int)-TileSystem.CurrentCenterPosition.y,
                    Z = (int)-TileSystem.CurrentCenterPosition.z,
                });
                Debug.Log("Inserted a new tile at " + (-TileSystem.CurrentCenterPosition));
            }
        }

        public static void RemoveCenterTile()
        {
            if (!_RemovingTiles && !TileSystem.Moving && TileSystem.CenterEntity != Entity.Null)
            {
                DeleteTile(TileSystem.CenterEntity);
                TileSystem.CenterEntity = Entity.Null;
                Debug.Log("Deleted a new tile at " + (-TileSystem.CurrentCenterPosition));
            }
        }

        public static void AddTile(int materialIndex, Coordinate initialCoordinate = default, TileType tileType = TileType.Normal)
        {
            _AddingTiles = true;
            _TileCreationQueue.Enqueue(new TileInfo
            {
                MaterialIndex = materialIndex,
                Coordinate = initialCoordinate,
                TileType = tileType
            });
        }

        public static void DeleteTile(Entity tileEntity)
        {
            _RemovingTiles = true;
            _TileDestructionQueue.Enqueue(tileEntity);
        }

        #endregion


        #region Jobs
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_AddingTiles && _TileCreationQueue.Count != 0)
            {
                int count = _TileCreationQueue.Count;
                for(int i = 0; i < 100 && i < count; i++)
                {
                    var tileInfo = _TileCreationQueue.Dequeue();
                    CreateTile(tileInfo);
                }
                if (_TileCreationQueue.Count == 0) _AddingTiles = false;
            }
            if (_RemovingTiles && _TileDestructionQueue.Count != 0)
            {
                int count = _TileDestructionQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var tileEntity = _TileDestructionQueue.Dequeue();
                    DestroyTile(tileEntity);
                }
                if (_TileDestructionQueue.Count == 0) _RemovingTiles = false;
            }
            return inputDeps;
        }

        private void DestroyTile(Entity tileEntity)
        {
            EntityManager.DestroyEntity(tileEntity);
            _TotalTileAmount--;
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
