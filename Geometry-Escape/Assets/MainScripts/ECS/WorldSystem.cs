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
        //public MonsterType MonsterType;
        //public Coordinate Coordinate;       // position
        //public int MaterialIndex;
        // default route
        // mechanic trigger
        // view scope check 
        // monster hp
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
        private static bool _AddingTiles;
        private static bool _RemovingTiles;
        private static bool _AddingMonsters;
        private static bool _RemovingMonsts;
        private static int _TotalTileAmount;
        private static int _TotalMonsterAmmount;
        private static TileResources m_TileResources;
        private static MonsterResources m_MonsterResources;

        public static int TotalTileAmount { get => _TotalTileAmount; }
        public static bool AddingTiles { get => _AddingTiles; set => _AddingTiles = value; }
        public static bool RemovingTiles { get => _RemovingTiles; set => _RemovingTiles = value; }
        public static TileResources TileResources { get => m_TileResources; set => m_TileResources = value; }
        public static MonsterResources MonsterResources { get => m_MonsterResources; set => m_MonsterResources = value; }
        public static int TotalMonsterAmmount { get => _TotalMonsterAmmount; set => _TotalMonsterAmmount = value; }
        public static EntityArchetype TileEntityArchetype { get => _TileEntityArchetype; set => _TileEntityArchetype = value; }
        public static EntityArchetype MonsterEntityArchetype { get => _MonsterEntityArchetype; set => _MonsterEntityArchetype = value; }
        public static NativeHashMap<Coordinate, TileType> TileHashMap { get => _TileHashMap; set => _TileHashMap = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            MonsterEntityArchetype = EntityManager.CreateArchetype(
                typeof(MonsterTypeIndex),
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
                typeof(TileTypeIndex),
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
            _TotalTileAmount = 0;
            _TotalMonsterAmmount = 0;

            int _TileCount = 50;
            for (int i = 0; i < _TileCount; i++)
            {
                for (int j = 0; j < _TileCount; j++)
                {
                    int index = i * _TileCount + j;

                    AddTileCreationInfo(new TileCreationInfo { TileProperties = new TileProperties { Index = index % 2 }, Coordinate = new Coordinate { X = i, Y = j, Z = 0 } });

                }
            }
            /*
            int mapDimension = _TileCount;
            int monsterNumber = MonsterNumber(mapDimension);
            Vector3[] MonsterPosiArray = MonsterPosiGenerator(mapDimension, monsterNumber);
            for (int i = 0; i < monsterNumber; i++)
            {
                // generate random coordinate

                Coordinate thisPosi = new Coordinate { };
                thisPosi.X = 1;//(int)MonsterPosiArray[i].x;
                thisPosi.Y = 1; //(int)MonsterPosiArray[i].y;
                thisPosi.Z = 1; //(int)MonsterPosiArray[i].z;
                Debug.Log("Check monster init");
                WorldSystem.AddMonster(0, thisPosi);
            }
            */
            
            CentralSystem.MonsterSystem.MonsterCurrentPosition = new Coordinate[2];     // hardcode number of monsters

            for (int i = 0; i < 2; i++)      // hardcode number of monsters
            {
                AddMonster(i, new Coordinate { X = 0, Y = 0, Z = -1 });

                CentralSystem.MonsterSystem.MonsterCurrentPosition[i] = new Coordinate { X=0,Y=0,Z=-1};
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
            if (!_AddingTiles && !CentralSystem.Moving && TileSystem.CenterEntity == Entity.Null)
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
            if (!_RemovingTiles && !CentralSystem.Moving && TileSystem.CenterEntity != Entity.Null)
            {
                DeleteTile(TileSystem.CenterEntity);
                TileSystem.CenterEntity = Entity.Null;
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

        /*public static void AddMonster(int materialIndex, Coordinate initialCoordinate = default, MonsterType monsterType = MonsterType.Green)
        {
            _AddingMonsts = true;
            _MonsterCreationQueue.Enqueue(new MonsterInfo
            {
                MaterialIndex = materialIndex,
                Coordinate = initialCoordinate,
                MonsterType = monsterType
            });
        }*/

        public static void AddMonster(int monsterIndex, Coordinate initialCoordinate = default)
        {
           CentralSystem.MonsterSystem.MonsterCount++;
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
                    CreateMonster(inputDeps, monsterInfo,i);
                }
                if (_MonsterCreationQueue.Count == 0) _AddingMonsters = false;
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
            var tileType = new TileTypeIndex
            {
                Value = tile.TileType
            };
            EntityManager.SetComponentData(instance, tileType);
            _TotalTileAmount++;
        }
        private void CreateMonster(JobHandle inputDeps, MonsterInfo monsterInfo,int i)
        {

            /*
            // TODO ->
            var materialIndex = monsterInfo.MaterialIndex;
            var initialCoordinate = monsterInfo.Coordinate;
            var monsterType = monsterInfo.MonsterType;

            var color = new DefaultColor { };
            //color.Color = Vector4.one;
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
                Value = 1
            };
            Entity instance = EntityManager.CreateEntity(_MonsterEntityArchetype);

            EntityManager.SetSharedComponentData(instance, renderMaterialIndex);
            EntityManager.SetComponentData(instance, initialCoordinate);
            EntityManager.SetComponentData(instance, color);
            EntityManager.SetComponentData(instance, textureInfo);
            EntityManager.SetComponentData(instance, maxTextureIndex);
            var properties = new MonsterProperties
            {
                MonsterType = monsterType,
                //Coordinate = 
                MaterialIndex = materialIndex
            };
            _TotalMonsterAmmount++;
            EntityManager.SetComponentData(instance, properties);*/
            var monster = m_MonsterResources.GetMonster(monsterInfo.MonsterIndex);
            /*typeof(MonsterTypeIndex),
                typeof(RenderContent),
                typeof(Coordinate),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(LocalToWorld),
                typeof(MonsterProperties),
                typeof(DefaultColor),
                typeof(TextureIndex),
                typeof(TextureMaxIndex),
                typeof(MonsterHP)*/
            var instance = EntityManager.CreateEntity(MonsterEntityArchetype);
            EntityManager.SetComponentData(instance, new MonsterTypeIndex
            {
                Value = monster.MonsterType
            });
            EntityManager.SetSharedComponentData(instance, monster.RenderContent);
            EntityManager.SetComponentData(instance, monsterInfo.Coordinate);
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
            }) ;
        }
    }
}
