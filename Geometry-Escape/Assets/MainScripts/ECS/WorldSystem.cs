using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
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
                typeof(LeftTile),
                typeof(RightTile),
                typeof(UpTile),
                typeof(DownTile),
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
            _MaterialAmount = RenderSystem.MaterialAmount;
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

        [BurstCompile]
        public struct LocateLeftTilesJob : IJobForEachWithEntity<TileProperties, Coordinate, RightTile>
        {
            [ReadOnly] public Coordinate coordinate;
            [NativeDisableParallelForRestriction]
            public NativeArray<LeftTile> leftTile;
            public Entity originEntity;
            //Case 1: Connect, Case 2: Disconnect.
            public byte mode;
            public void Execute(Entity entity, int index, ref TileProperties c0, ref Coordinate c1, ref RightTile c2)
            {
                LeftTile left = default;
                if (c1.X == coordinate.X - 1 && c1.Y == coordinate.Y) {
                    switch (mode)
                    {
                        case 1:
                            left.Value = entity;
                            c2.Value = originEntity;
                            break;
                        case 2:
                            left.Value = Entity.Null;
                            c2.Value = Entity.Null;
                            break;

                    }
                    leftTile[0] = left;
                }
            }
        }
        [BurstCompile]
        public struct LocateRightTilesJob : IJobForEachWithEntity<TileProperties, Coordinate, LeftTile>
        {
            [ReadOnly] public Coordinate coordinate;
            [NativeDisableParallelForRestriction]
            public NativeArray<RightTile> rightTile;
            public Entity originEntity;
            public byte mode;
            public void Execute(Entity entity, int index, ref TileProperties c0, ref Coordinate c1, ref LeftTile c2)
            {
                RightTile right = default;
                if (c1.X == coordinate.X + 1 && c1.Y == coordinate.Y)
                {
                    switch (mode)
                    {
                        case 1:
                            right.Value = entity;
                            c2.Value = originEntity;
                            break;
                        case 2:
                            right.Value = Entity.Null;
                            c2.Value = Entity.Null;
                            break;
                    }
                    rightTile[0] = right;
                }
            }
        }
        [BurstCompile]
        public struct LocateUpTilesJob : IJobForEachWithEntity<TileProperties, Coordinate, DownTile>
        {
            [ReadOnly] public Coordinate coordinate;
            [NativeDisableParallelForRestriction]
            public NativeArray<UpTile> upTile;
            public Entity originEntity;
            public byte mode;
            public void Execute(Entity entity, int index, ref TileProperties c0, ref Coordinate c1, ref DownTile c2)
            {
                UpTile up = default;
                if (c1.X == coordinate.X && c1.Y == coordinate.Y + 1)
                {
                    switch (mode)
                    {
                        case 1:
                            up.Value = entity;
                            c2.Value = originEntity;
                            break;
                        case 2:
                            up.Value = Entity.Null;
                            c2.Value = Entity.Null;
                            break;
                    }
                    upTile[0] = up;
                }
            }
        }
        [BurstCompile]
        public struct LocateDownTilesJob : IJobForEachWithEntity<TileProperties, Coordinate, UpTile>
        {
            [ReadOnly] public Coordinate coordinate;
            [NativeDisableParallelForRestriction]
            public NativeArray<DownTile> downTile;
            public Entity originEntity;
            public byte mode;
            public void Execute(Entity entity, int index, ref TileProperties c0, ref Coordinate c1, ref UpTile c2)
            {
                DownTile down = default;
                if (c1.X == coordinate.X && c1.Y == coordinate.Y - 1)
                {
                    switch (mode)
                    {
                        case 1:
                            down.Value = entity;
                            c2.Value = originEntity;
                            break;
                        case 2:
                            down.Value = Entity.Null;
                            c2.Value = Entity.Null;
                            break;
                    }
                    downTile[0] = down;
                }
            }
        }

        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_AddingTiles && _TileCreationQueue.Count != 0)
            {
                int count = _TileCreationQueue.Count;
                for(int i = 0; i < 10 && i < count; i++)
                {
                    var tileInfo = _TileCreationQueue.Dequeue();
                    CreateTile(inputDeps, tileInfo);
                }
                if (_TileCreationQueue.Count == 0) _AddingTiles = false;
            }
            if (_RemovingTiles && _TileDestructionQueue.Count != 0)
            {
                int count = _TileDestructionQueue.Count;
                for (int i = 0; i < 10 && i < count; i++)
                {
                    var tileEntity = _TileDestructionQueue.Dequeue();
                    DestroyTile(inputDeps, tileEntity);
                }
                if (_TileDestructionQueue.Count == 0) _RemovingTiles = false;
            }
            return inputDeps;
        }

        private void DestroyTile(JobHandle inputDeps, Entity tileEntity)
        {
            Coordinate coordinate = EntityManager.GetComponentData<Coordinate>(tileEntity);

            NativeArray<LeftTile> left = new NativeArray<LeftTile>(1, Allocator.TempJob);
            NativeArray<RightTile> right = new NativeArray<RightTile>(1, Allocator.TempJob);
            NativeArray<UpTile> up = new NativeArray<UpTile>(1, Allocator.TempJob);
            NativeArray<DownTile> down = new NativeArray<DownTile>(1, Allocator.TempJob);
            inputDeps = new LocateLeftTilesJob
            {
                leftTile = left,
                coordinate = coordinate,
                originEntity = tileEntity,
                mode = 2
            }.Schedule(this, inputDeps);
            inputDeps = new LocateRightTilesJob
            {
                rightTile = right,
                coordinate = coordinate,
                originEntity = tileEntity,
                mode = 2
            }.Schedule(this, inputDeps);
            inputDeps = new LocateUpTilesJob
            {
                upTile = up,
                coordinate = coordinate,
                originEntity = tileEntity,
                mode = 2
            }.Schedule(this, inputDeps);
            inputDeps = new LocateDownTilesJob
            {
                downTile = down,
                coordinate = coordinate,
                originEntity = tileEntity,
                mode = 2
            }.Schedule(this, inputDeps);
            inputDeps.Complete();

            left.Dispose();
            right.Dispose();
            up.Dispose();
            down.Dispose();

            EntityManager.DestroyEntity(tileEntity);
            _TotalTileAmount--;
        }

        private void CreateTile(JobHandle inputDeps, TileInfo tileInfo)
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
            NativeArray<LeftTile> left = new NativeArray<LeftTile>(1, Allocator.TempJob);
            NativeArray<RightTile> right = new NativeArray<RightTile>(1, Allocator.TempJob);
            NativeArray<UpTile> up = new NativeArray<UpTile>(1, Allocator.TempJob);
            NativeArray<DownTile> down = new NativeArray<DownTile>(1, Allocator.TempJob);
            inputDeps = new LocateLeftTilesJob
            {
                leftTile = left,
                coordinate = initialCoordinate,
                originEntity = instance,
                mode = 1
            }.Schedule(this, inputDeps);
            inputDeps = new LocateRightTilesJob
            {
                rightTile = right,
                coordinate = initialCoordinate,
                originEntity = instance,
                mode = 1
            }.Schedule(this, inputDeps);
            inputDeps = new LocateUpTilesJob
            {
                upTile = up,
                coordinate = initialCoordinate,
                originEntity = instance,
                mode = 1
            }.Schedule(this, inputDeps);
            inputDeps = new LocateDownTilesJob
            {
                downTile = down,
                coordinate = initialCoordinate,
                originEntity = instance,
                mode = 1
            }.Schedule(this, inputDeps);
            inputDeps.Complete();

            
            EntityManager.SetComponentData(instance, left[0]);
            EntityManager.SetComponentData(instance, right[0]);
            EntityManager.SetComponentData(instance, up[0]);
            EntityManager.SetComponentData(instance, down[0]);
            left.Dispose();
            right.Dispose();
            up.Dispose();
            down.Dispose();

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
