using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace GeometryEscape
{
    [Serializable]
    public struct HealthBarMeshMaterial
    {
        public Mesh Mesh;
        public Material Material;
    }
    /// <summary>
    /// The system that control all monsters.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class MonsterSystem : JobComponentSystem
    {
        #region Private
        private static MeshRenderSystem m_RenderSystem;
        private static TileSystem m_TileSystem;
        private static WorldSystem m_WorldSystem;
        private static ControlSystem m_ControlSystem;
        private static AudioSystem m_AudioSystem;
        private static AudioSource m_MusicAudioSource;
        private static AudioResources.Music m_Music;
        private static CentralSystem m_CentralSystem;
        private static EntityQuery _MonsterEntityQuery;
        private static MonsterMovePattern m_MonsterMovePattern;
        private static EntityArchetype _MonsterEntityArchetype;
        private static Entity _MonsterKilled;

        private static float _Timer;
        private static int _Counter;
        private static float _TimeStep;
        private int _MonsterMaterAmount;
        private static float _beatTime;
        private float _currentTime;
        private static NativeQueue<MonsterInfo> _MonsterCreationQueue;
        public Entity MonsterKilled { get => _MonsterKilled; set => _MonsterKilled = value; }
        private static Vector3 startPoint;
        private static Vector3 endPoint;


        //public float beatTime { get => _beatTime; set => _beatTime=value; }
        #endregion

        #region Public
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            _MonsterEntityQuery = EntityManager.CreateEntityQuery(typeof(MonsterProperties));
        }

        public void ChangeHealth()
        {

        }
        public void Init()
        {
            ShutDown();
            m_MonsterMovePattern = default;
            _TimeStep = 0.5f;
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
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods


        /*public void MonsterRoute() {

            float distCovered = (_currentTime - _beatTime) * 2.0f;
            float fractionOfJourney=distCovered/ Vector3.Distance(startPoint, endPoint);
            lerp = Vector3.Lerp(startPoint, endPoint, fractionOfJourney);
            Debug.Log("in monsterRoute "+startPoint);
        }*/

        public void RouteOnCall(Entity entity)
        {
            /** need an algorithm to calculate 
            https://medium.com/@jimmy0x52/making-smarter-monsters-adding-pathfinding-to-unitys-2d-roguelike-tutorial-5c004207a7a3
                       
            */


        }
        public void ChangeMonsterHealth()
        {
            UISystem.ChangeMonsterHealth();
        }

        #endregion


        #region Jobs

        protected struct MoveMonster : IJobForEachWithEntity<Coordinate, PreviousCoordinate, TargetCoordinate, Timer, MonsterProperties, MonsterHP>
        {
            [ReadOnly] public Vector3 position;
            public void Execute(Entity entity,int index,ref Coordinate c0, ref PreviousCoordinate c1, ref TargetCoordinate c2, ref Timer c3, ref MonsterProperties c4, ref MonsterHP c5)
            {
                if (!c3.isOn) return;
                var proportion = c3.T / c3.maxT;
                c0.X = math.lerp(c1.X, c2.X, proportion);
                c0.Y = math.lerp(c1.Y, c2.Y, proportion);
                c0.Z = math.lerp(c1.Z, c2.Z, proportion);
                c0.Direction = math.lerp(c1.Direction, c2.Direction, proportion);
                if (c3.T == c3.maxT)
                {
                    c3.isOn = false;
                }
                if (c5.Value <= 0)
                {
                    _MonsterKilled = entity;
                }
            }

        }

        // check if any monsters are in the scope and run to the character
        struct CheckOnCall : IJobForEach<Coordinate>
        {
            public void Execute(ref Coordinate c0)
            {

            }
        }

        // check if any monsters got shot
        struct CheckDamage : IJobForEach<Coordinate>
        {
            public void Execute(ref Coordinate c0)
            {
            }
        }

        //看完这段请把这段代码挪到合适的位置。
        public struct MonsterMovePattern
        {
            //在这里你可以记录你需要设置的公用参数，比如你之前的lerp（虽然我不知道那是干啥的
            //这里的参数值要在schedule一个新的job时设定，具体看OnBeatsUpdate
            public int Counter;

            //这个struct会被传到SetRoute里面，你可以在这里设计怪物行走路线，下面是两个示例，你可以设置更多传入参数，比如说怪物的生命值之类的来丰富你的pattern

            //左右横移
            public Coordinate LeftAndRight(PreviousCoordinate previousCoordinate)
            {
                Coordinate targetCoordinate = default;
                switch (Counter % 2)
                {
                    case 0:
                        targetCoordinate.X = previousCoordinate.X - 1;
                        targetCoordinate.Y = previousCoordinate.Y;
                        targetCoordinate.Z = previousCoordinate.Z;
                        targetCoordinate.Direction = previousCoordinate.Direction;
                        break;
                    case 1:
                        targetCoordinate.X = previousCoordinate.X + 1;
                        targetCoordinate.Y = previousCoordinate.Y;
                        targetCoordinate.Z = previousCoordinate.Z;
                        targetCoordinate.Direction = previousCoordinate.Direction;
                        break;
                }
                return targetCoordinate;
            }
            //原地转圈
            public Coordinate SmallCircle(PreviousCoordinate previousCoordinate)
            {
                Coordinate targetCoordinate = default;
                switch (Counter % 4)
                {
                    case 0:
                        targetCoordinate.X = previousCoordinate.X + 1;
                        targetCoordinate.Y = previousCoordinate.Y;
                        targetCoordinate.Z = previousCoordinate.Z;
                        targetCoordinate.Direction = previousCoordinate.Direction;
                        break;
                    case 1:
                        targetCoordinate.X = previousCoordinate.X;
                        targetCoordinate.Y = previousCoordinate.Y + 1;
                        targetCoordinate.Z = previousCoordinate.Z;
                        targetCoordinate.Direction = previousCoordinate.Direction;
                        break;
                    case 2:
                        targetCoordinate.X = previousCoordinate.X - 1;
                        targetCoordinate.Y = previousCoordinate.Y;
                        targetCoordinate.Z = previousCoordinate.Z;
                        targetCoordinate.Direction = previousCoordinate.Direction;
                        break;
                    case 3:
                        targetCoordinate.X = previousCoordinate.X;
                        targetCoordinate.Y = previousCoordinate.Y - 1;
                        targetCoordinate.Z = previousCoordinate.Z;
                        targetCoordinate.Direction = previousCoordinate.Direction;
                        break;
                }
                return targetCoordinate;
            }
        }

        [BurstCompile]
        struct SetRoutePosition : IJobForEach<MonsterProperties, TypeOfMonster, Coordinate, PreviousCoordinate, TargetCoordinate, Timer>
        {
            [ReadOnly] public MonsterMovePattern MonsterMovePattern;
            [ReadOnly] public NativeHashMap<Coordinate, TileType> TileHashMap;
            public void Execute(ref MonsterProperties c0, ref TypeOfMonster c1, ref Coordinate c2, ref PreviousCoordinate c3, ref TargetCoordinate c4, ref Timer c5)
            {
                c3.X = c2.X;
                c3.Y = c2.Y;
                c3.Z = c2.Z;
                c3.Direction = c2.Direction;
                Coordinate nextMove = default;
                switch (c1.Value)
                {
                    case MonsterType.Blue:
                        nextMove = MonsterMovePattern.SmallCircle(c3);
                        break;
                    case MonsterType.Green:
                        nextMove = MonsterMovePattern.LeftAndRight(c3);
                        break;
                    case MonsterType.Skeleton:
                        nextMove = MonsterMovePattern.SmallCircle(c3);
                        break;
                    default:
                        nextMove = c2;
                        break;
                }
                TileType tileType;
                if (!TileHashMap.TryGetValue(nextMove, out tileType))
                {
                    return;
                }


                switch (tileType)
                {
                    case TileType.Blocked:
                        return;
                    default:
                        break;
                }
                c5.T = 0;
                c5.maxT = 0.2f;
                c5.isOn = true;
                c4.X = nextMove.X;
                c4.Y = nextMove.Y;
                c4.Z = nextMove.Z;
                c4.Direction = nextMove.Direction;
            }
        }
        #endregion

        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {
            //Schedule your job for every beat here.
            var monsterEntityList = _MonsterEntityQuery.ToEntityArray(Allocator.TempJob);
            m_MonsterMovePattern.Counter = beatCounter;
            for (int i = 0; i < monsterEntityList.Length; i++)
            {
                Entity monster = monsterEntityList[i];
                PreviousCoordinate previousCoordinate = default;
                var coordinate = EntityManager.GetComponentData<Coordinate>(monster);
                previousCoordinate.X = coordinate.X;
                previousCoordinate.Y = coordinate.Y;
                previousCoordinate.Z = coordinate.Z;
                previousCoordinate.Direction = coordinate.Direction;
                Coordinate nextMove = default;
                var type = EntityManager.GetComponentData<TypeOfMonster>(monster);
                switch (type.Value)
                {
                    case MonsterType.Blue:
                        nextMove = m_MonsterMovePattern.SmallCircle(previousCoordinate);
                        break;
                    case MonsterType.Green:
                        nextMove = m_MonsterMovePattern.LeftAndRight(previousCoordinate);
                        break;
                    case MonsterType.Skeleton:
                        nextMove = m_MonsterMovePattern.SmallCircle(previousCoordinate);
                        break;
                    default:
                        nextMove = coordinate;
                        break;
                }
                TileType tileType;
                //If the next position is out of map we cancel.
                if (!WorldSystem.TileHashMap.TryGetValue(nextMove, out tileType))
                {
                    continue;
                }
                //If the next position already has a monster, we cancel.
                TypeOfMonster monsterType = default;
                if (WorldSystem.MonsterHashMap.TryGetValue(nextMove, out monsterType))
                {
                    continue;
                }
                WorldSystem.MonsterHashMap.Remove(coordinate);
                if (!WorldSystem.MonsterHashMap.TryAdd(nextMove, type))
                {
                    Debug.Log("Something wrong with setting monster position hash map!");
                    continue;
                }
                EntityManager.SetComponentData(monster, previousCoordinate);
                EntityManager.SetComponentData(monster, new TargetCoordinate { X = nextMove.X, Y = nextMove.Y, Z = nextMove.Z, Direction = nextMove.Direction });
                EntityManager.SetComponentData(monster, new Timer { isOn = true, T = 0, maxT = 0.2f });
            }

            monsterEntityList.Dispose();

            inputDeps.Complete();
            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            //Schedule your job for every time step here. Time step is defined in central system.

            return inputDeps;
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            //MonsterRoute();
            inputDeps = new MoveMonster { }.Schedule(this, inputDeps);
            

            return inputDeps;


        }


    }
}