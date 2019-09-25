using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace GeometryEscape
{
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

        private static EntityArchetype _MonsterEntityArchetype;

        private static float _Timer; 
        private static int _Counter;
        private static float _TimeStep;
        private int _MonsterCount;
        private int _MonsterMaterAmount;
        private static float _beatTime;
        private float _currentTime;
        private static NativeQueue<MonsterInfo> _MonsterCreationQueue;
        private static Vector3 startPoint;
        private static Vector3 endPoint;
        private static Vector3 lerp;


        //public float beatTime { get => _beatTime; set => _beatTime=value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {

        }

        public void Init()
        {
            ShutDown();
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


        public void MonsterRoute() {

            float distCovered = (_currentTime - _beatTime) * 2.0f;
            float fractionOfJourney=distCovered/ Vector3.Distance(startPoint, endPoint);
            lerp = Vector3.Lerp(startPoint, endPoint, fractionOfJourney);
            Debug.Log("in monsterRoute "+startPoint);
        }

        public void RouteOnCall(Entity entity) {
            /** need an algorithm to calculate 
            https://medium.com/@jimmy0x52/making-smarter-monsters-adding-pathfinding-to-unitys-2d-roguelike-tutorial-5c004207a7a3
                       
            */


        }


        #endregion


        #region Jobs

        [BurstCompile]
        protected struct MoveMonster : IJobForEach<Coordinate, PreviousCoordinate, TargetCoordinate, Timer>
        {
            public void Execute(ref Coordinate c0, ref PreviousCoordinate c1, ref TargetCoordinate c2, ref Timer c3)
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
        [BurstCompile]
        struct MonsterPositionUpdate : IJobForEach<Coordinate, MonsterTypeIndex>
        {
            public void Execute(ref Coordinate c0, ref MonsterTypeIndex c2)
            {
                if (c2.Value == MonsterType.Green)
                {
                    startPoint.x = c0.X;
                    startPoint.y = c0.Y;
                    startPoint.z = c0.Z;

                    endPoint.x = c0.X + 1;
                    endPoint.y = c0.Y;
                    endPoint.z = c0.Z;
                }

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
            public TargetCoordinate LeftAndRight(PreviousCoordinate previousCoordinate)
            {
                TargetCoordinate targetCoordinate = default;
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
            public TargetCoordinate SmallCircle(PreviousCoordinate previousCoordinate)
            {
                TargetCoordinate targetCoordinate = default;
                switch(Counter % 4)
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
        struct SetRoutePosition : IJobForEach<MonsterProperties, MonsterTypeIndex, Coordinate, PreviousCoordinate, TargetCoordinate, Timer>
        {
            [ReadOnly] public MonsterMovePattern MonsterMovePattern;
            public void Execute(ref MonsterProperties c0, ref MonsterTypeIndex c1, ref Coordinate c2, ref PreviousCoordinate c3, ref TargetCoordinate c4, ref Timer c5)
            {
                c3.X = c2.X;
                c3.Y = c2.Y;
                c3.Z = c2.Z;
                c3.Direction = c2.Direction;
                switch (c1.Value)
                {
                    case MonsterType.Blue:
                        c4 = MonsterMovePattern.SmallCircle(c3);
                        break;
                    case MonsterType.Green:
                        c4 = MonsterMovePattern.LeftAndRight(c3);
                        break;
                    case MonsterType.Skeleton:
                        c4 = MonsterMovePattern.SmallCircle(c3);
                        break;
                    default:
                        c4.X = c3.X;
                        c4.Y = c3.Y;
                        c4.Z = c3.Z;
                        c4.Direction = c3.Direction;
                        break;
                }
                c5.T = 0;
                c5.maxT = 0.2f;
                c5.isOn = true;
            }
        }
        #endregion

        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {
            //Schedule your job for every beat here.
            inputDeps = new SetRoutePosition
            {
                MonsterMovePattern = new MonsterMovePattern
                {
                    Counter = beatCounter
                }
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            //Schedule your job for every time step here. Time step is defined in central system.
            //Coordinate startPoint = new Coordinate { X=c1.X, Y=c1.Y, Z=c1.Z };
            //Coordinate endPoint=new Coordinate {X=c1.X2,Y=c1.Y2,Z=c1.Z2 };
            
            return inputDeps;
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            //MonsterRoute();
            inputDeps = new MoveMonster{ }.Schedule(this, inputDeps);
            
            return inputDeps;


        }
       

    }
}