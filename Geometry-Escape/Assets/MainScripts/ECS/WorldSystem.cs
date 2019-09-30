using System;
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
    [Serializable]
    public struct TileCreationInfo
    {
        public TileProperties TileProperties;
        public Coordinate Coordinate;
    }
    [Serializable]
    public struct MonsterInfo
    {
        public int MonsterIndex;
        public Coordinate Coordinate;
    }


    /// <summary>
    /// The system manages the creation and destruction of all world entities.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [AlwaysUpdateSystem]
    public class WorldSystem : JobComponentSystem
    {
        #region Private
        private FileSystem m_FileSystem;
        private static EntityManager m_EntityManager;
        private static NativeQueue<TileCreationInfo> _TileCreationQueue;
        private static NativeQueue<Entity> _TileDestructionQueue;


        private static NativeQueue<MonsterInfo> _MonsterCreationQueue;
        private static NativeQueue<Entity> _MonsterDestructionQueue;
        #endregion

        #region Public
        private static EntityArchetype _TileEntityArchetype;
        private static EntityArchetype _MonsterEntityArchetype;
        private static NativeHashMap<Coordinate, TileType> _TileHashMap;
        private static NativeHashMap<Coordinate, TypeOfMonster> _MonsterHashMap;


        private static bool _AddingTiles;
        private static bool _RemovingTiles;
        private static bool _AddingMonsters;
        private static bool _RemovingMonsters;
        private static int _TotalTileAmount;
        private int _TotalMonsterAmount;
        private static TileResources m_TileResources;
        private static MonsterResources m_MonsterResources;

        public static int TotalTileAmount { get => _TotalTileAmount; }
        public static bool AddingTiles { get => _AddingTiles; set => _AddingTiles = value; }
        public static bool RemovingTiles { get => _RemovingTiles; set => _RemovingTiles = value; }
        public static TileResources TileResources { get => m_TileResources; set => m_TileResources = value; }
        public static MonsterResources MonsterResources { get => m_MonsterResources; set => m_MonsterResources = value; }
        public int TotalMonsterAmount { get => _TotalMonsterAmount; set => _TotalMonsterAmount = value; }
        public static EntityArchetype TileEntityArchetype { get => _TileEntityArchetype; set => _TileEntityArchetype = value; }
        public static EntityArchetype MonsterEntityArchetype { get => _MonsterEntityArchetype; set => _MonsterEntityArchetype = value; }
        public static NativeHashMap<Coordinate, TileType> TileHashMap { get => _TileHashMap; set => _TileHashMap = value; }
        public static NativeHashMap<Coordinate, TypeOfMonster> MonsterHashMap { get => _MonsterHashMap; set => _MonsterHashMap = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            MonsterEntityArchetype = EntityManager.CreateArchetype(
                typeof(TypeOfEntity),
                typeof(TypeOfMonster),
                typeof(RenderContent),
                typeof(Coordinate),
                typeof(Timer),
                typeof(PreviousCoordinate),
                typeof(TargetCoordinate),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(LocalToWorld),
                typeof(MonsterProperties),
                typeof(DisplayColor),
                typeof(TextureIndex),
                typeof(TextureMaxIndex),
                typeof(MonsterHP)
                );

            TileEntityArchetype = EntityManager.CreateArchetype(
                typeof(Timer),
                typeof(TypeOfEntity),
                typeof(TypeOfTile),
                typeof(LeftTile),
                typeof(RightTile),
                typeof(UpTile),
                typeof(DownTile),
                typeof(RenderContent),
                typeof(Coordinate),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(LocalToWorld),
                typeof(TileProperties),
                typeof(DefaultColor),
                typeof(DisplayColor),
                typeof(TextureIndex),
                typeof(TextureMaxIndex)
                );
            m_EntityManager = World.Active.EntityManager;
            m_FileSystem = new FileSystem();
            Enabled = false;
        }

        public void Init()
        {
            ShutDown();
            _TileCreationQueue = new NativeQueue<TileCreationInfo>(Allocator.Persistent);
            _TileDestructionQueue = new NativeQueue<Entity>(Allocator.Persistent);
            _MonsterCreationQueue = new NativeQueue<MonsterInfo>(Allocator.Persistent);
            _MonsterDestructionQueue = new NativeQueue<Entity>(Allocator.Persistent);
            _TileHashMap = new NativeHashMap<Coordinate, TileType>(10000, Allocator.Persistent);
            _MonsterHashMap = new NativeHashMap<Coordinate, TypeOfMonster>(10000, Allocator.Persistent);

            _TotalTileAmount = 0;
            _TotalMonsterAmount = 0;

            int _TileCount = 50;
            for (int i = 0; i < _TileCount; i++)
            {
                for (int j = 0; j < _TileCount; j++)
                {
                    int index = i * _TileCount + j;

                    AddTileCreationInfo(new TileCreationInfo { TileProperties = new TileProperties { Index = index % 5 }, Coordinate = new Coordinate { X = i, Y = j, Z = 0 } });

                }
            }

            for (int i = 1; i < 50; i++)
            {
                AddMonster(i % 2, new Coordinate { X = i, Y = i, Z = -1 });

            }

            Enabled = true;
        }

        public void Pause()
        {
            Enabled = false;
        }

        public void Resume()
        {

            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_TileCreationQueue.IsCreated) _TileCreationQueue.Dispose();
            if (_TileDestructionQueue.IsCreated) _TileDestructionQueue.Dispose();
            if (_MonsterCreationQueue.IsCreated) _MonsterCreationQueue.Dispose();
            if (_MonsterDestructionQueue.IsCreated) _MonsterDestructionQueue.Dispose();
            if (_TileHashMap.IsCreated) _TileHashMap.Dispose();
            if (_MonsterHashMap.IsCreated) _MonsterHashMap.Dispose();

        }

        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods

        public int MonsterNumber(int mapDimension)
        {
            return mapDimension / 10;
        }
        public Vector3[] MonsterPosiGenerator(int mapDimension, int MonsterNumber)
        {
            Vector3[] MonstPosiArray = new Vector3[MonsterNumber];
            float z = 1.0f;
            float randomx = 0.0f;
            float randomy = 0.0f;
            // need to revise to check viability
            for (int i = 0; i < MonsterNumber; i++)
            {
                Vector3 thisVec = new Vector3 { };
                randomx = UnityEngine.Random.Range(1, mapDimension);
                randomy = UnityEngine.Random.Range(1, mapDimension);
                thisVec.x = randomx;
                thisVec.y = randomy;
                thisVec.z = z;
                MonstPosiArray[i] = thisVec;
            }
            return MonstPosiArray;



        }
        public static void AddCenterTile()
        {
            if (!_AddingTiles && !CentralSystem.Moving && FloatingOriginSystem.CenterTileEntity == Entity.Null)
            {
                AddTileCreationInfo(new TileCreationInfo
                {
                    TileProperties = new TileProperties { Index = 0 },
                    Coordinate = new Coordinate
                    {
                        X = (int)-CentralSystem.CurrentCenterPosition.x,
                        Y = (int)-CentralSystem.CurrentCenterPosition.y,
                        Z = (int)-CentralSystem.CurrentCenterPosition.z,
                    }
                });
                Debug.Log("Inserted a new tile at " + (-CentralSystem.CurrentCenterPosition));
            }
        }

        public static void RemoveCenterTile()
        {
            if (!_RemovingTiles && !CentralSystem.Moving && FloatingOriginSystem.CenterTileEntity != Entity.Null)
            {
                DeleteTile(FloatingOriginSystem.CenterTileEntity);
                FloatingOriginSystem.CenterTileEntity = Entity.Null;
                Debug.Log("Deleted a new tile at " + (-CentralSystem.CurrentCenterPosition));
            }
        }

        public static void AddTileCreationInfo(TileCreationInfo tileCreationInfo)
        {
            _AddingTiles = true;
            _TileCreationQueue.Enqueue(tileCreationInfo);
        }



        public static void DeleteTile(Entity tileEntity)
        {
            _RemovingTiles = true;
            _TileDestructionQueue.Enqueue(tileEntity);
        }

        public static void AddMonster(int monsterIndex, Coordinate initialCoordinate = default)
        {
            CentralSystem.WorldSystem.TotalMonsterAmount++;
            _AddingMonsters = true;
            _MonsterCreationQueue.Enqueue(new MonsterInfo
            {
                Coordinate = initialCoordinate,
                MonsterIndex = monsterIndex
            });
        }

        public static void DestroyAllTiles()
        {
            var query = m_EntityManager.CreateEntityQuery(typeof(TileProperties));
            var list = query.ToEntityArray(Allocator.TempJob);
            foreach (var i in list)
            {
                _TileDestructionQueue.Enqueue(i);
            }
            _RemovingTiles = true;
            list.Dispose();
        }

        public static void SaveMap(string mapName)
        {
            FileSystem.SaveNewMap(mapName);
        }

        public static void LoadMap(string mapName)
        {
            FileSystem.LoadMap(mapName);
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
                if (c1.X == coordinate.X - 1 && c1.Y == coordinate.Y)
                {
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
            if (_AddingTiles && _TileCreationQueue.Count != 0)
            {
                int count = _TileCreationQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var tileInfo = _TileCreationQueue.Dequeue();
                    CreateTile(inputDeps, tileInfo);
                }
                if (_TileCreationQueue.Count == 0) _AddingTiles = false;
            }
            if (_RemovingTiles && _TileDestructionQueue.Count != 0)
            {
                int count = _TileDestructionQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var tileEntity = _TileDestructionQueue.Dequeue();
                    DestroyTile(inputDeps, tileEntity);
                }
                if (_TileDestructionQueue.Count == 0) _RemovingTiles = false;
            }


            if (_AddingMonsters && _MonsterCreationQueue.Count != 0)
            {
                int count = _MonsterCreationQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var monsterInfo = _MonsterCreationQueue.Dequeue();
                    CreateMonster(inputDeps, monsterInfo, i);
                }
                if (_MonsterCreationQueue.Count == 0) _AddingMonsters = false;
            }
            if (_RemovingMonsters && _MonsterDestructionQueue.Count != 0)
            {
                int count = _MonsterDestructionQueue.Count;
                for (int i = 0; i < 100 && i < count; i++)
                {
                    var monsterEntity = _MonsterDestructionQueue.Dequeue();
                    DestroyMonster(inputDeps, monsterEntity);
                }
                if (_MonsterDestructionQueue.Count == 0) _RemovingMonsters = false;
            }
            if (!CentralSystem.MonsterSystem.MonsterKilled.Equals(Entity.Null))
            {
                DestroyMonster(inputDeps, CentralSystem.MonsterSystem.MonsterKilled);
                CentralSystem.MonsterSystem.MonsterKilled = Entity.Null;
            }
            return inputDeps;
        }

        private void DestroyTile(JobHandle inputDeps, Entity tileEntity)
        {
            Coordinate coordinate = EntityManager.GetComponentData<Coordinate>(tileEntity);
            _TileHashMap.Remove(coordinate);
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
        private void CreateTile(JobHandle inputDeps, TileCreationInfo tileInfo)
        {
            var initialCoordinate = tileInfo.Coordinate;

            var color = new DefaultColor { };
            color.Value = Vector4.one;
            var textureInfo = new TextureIndex
            {
                Value = 1
            };

            var tile = m_TileResources.GetTile(tileInfo.TileProperties.Index);
            _TileHashMap.TryAdd(initialCoordinate, tile.TileType);
            var maxTextureIndex = new TextureMaxIndex
            {
                Value = tile.MaxIndex
            };
            Entity instance = EntityManager.CreateEntity(TileEntityArchetype);
            
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

            EntityManager.SetComponentData(instance, new TypeOfEntity { Value = EntityType.Tile });
            EntityManager.SetComponentData(instance, left[0]);
            EntityManager.SetComponentData(instance, right[0]);
            EntityManager.SetComponentData(instance, up[0]);
            EntityManager.SetComponentData(instance, down[0]);
            left.Dispose();
            right.Dispose();
            up.Dispose();
            down.Dispose();

            EntityManager.SetSharedComponentData(instance, tile.RenderContent);
            EntityManager.SetComponentData(instance, initialCoordinate);
            EntityManager.SetComponentData(instance, color);
            EntityManager.SetComponentData(instance, textureInfo);
            EntityManager.SetComponentData(instance, maxTextureIndex);

            EntityManager.SetComponentData(instance, tileInfo.TileProperties);
            var tileType = new TypeOfTile
            {
                Value = tile.TileType
            };
            EntityManager.SetComponentData(instance, tileType);
            _TotalTileAmount++;
        }
        public void DestroyMonster(JobHandle inputDeps, Entity monsterEntity)
        {
            var coordinate = EntityManager.GetComponentData<Coordinate>(monsterEntity);
            MonsterHashMap.Remove(coordinate);
            EntityManager.DestroyEntity(monsterEntity);
            TotalMonsterAmount--;
        }
        private void CreateMonster(JobHandle inputDeps, MonsterInfo monsterInfo, int i)
        {
            var monster = m_MonsterResources.GetMonster(monsterInfo.MonsterIndex);

            if (!MonsterHashMap.TryAdd(monsterInfo.Coordinate, new TypeOfMonster
            {
                Value = monster.MonsterType
            }))
            {
                Debug.Log("Warning! Trying to create a new monster with the same position as another monster!");
            }
            var instance = EntityManager.CreateEntity(MonsterEntityArchetype);
            //Debug.Log("test drawmesh"+monster.HealthBarMeshMaterial.Mesh.Equals(null));
            //Graphics.DrawMesh(monster.HealthBarMeshMaterial.Mesh, new Vector3 { x=0,y=0,z=-3}, Quaternion.identity, monster.HealthBarMeshMaterial.Material,0);
            EntityManager.SetComponentData(instance, new TypeOfEntity { Value = EntityType.Monster });
            EntityManager.SetComponentData(instance, new TypeOfMonster
            {
                Value = monster.MonsterType
            });
            EntityManager.SetSharedComponentData(instance, monster.RenderContent);


            EntityManager.SetComponentData(instance, monsterInfo.Coordinate); //
            EntityManager.SetComponentData(instance, new DisplayColor
            {
                Value = Vector4.one
            });
            EntityManager.SetComponentData(instance, monster.TextureMaxIndex);
            EntityManager.SetComponentData(instance, new Scale
            {
                Value = 1
            });
            EntityManager.SetComponentData(instance, new MonsterProperties
            {
                Index = i
            });
            EntityManager.SetComponentData(instance, new MonsterHP
            {
                Value = 100
            });
        }
    }
}
