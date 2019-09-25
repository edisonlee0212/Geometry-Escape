using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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
        
        private int _MonsterCount;
        private int _MonsterMaterAmount;
        private static float _beatTime;
        private static NativeQueue<MonsterInfo> _MonsterCreationQueue;

        //public float beatTime { get => _beatTime; set => _beatTime=value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {

        }

        public void Init()
        {
            ShutDown();
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



        public void RouteOnCall(Entity entity) {
            /** need an algorithm to calculate 
            https://medium.com/@jimmy0x52/making-smarter-monsters-adding-pathfinding-to-unitys-2d-roguelike-tutorial-5c004207a7a3
                       
            */


        }


        #endregion

        #region Jobs
        #endregion
        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {
            //Schedule your job for every beat here.
            //Coordinate startPoint = new Coordinate { X=c1.X, Y=c1.Y, Z=c1.Z };
            //Coordinate endPoint=new Coordinate {X=c1.X2,Y=c1.Y2,Z=c1.Z2 };
            _beatTime = Time.time;

            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            //Schedule your job for every time step here. Time step is defined in central system.
            return inputDeps;
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
          

            inputDeps = new MonsterPositionUpdate
            {
                //c0.X = lerp.X;
                //c0.Y = lerp.Y;
                //c0.Z = lerp.Z;

                
            }.Schedule(this, inputDeps);
            inputDeps.Complete();

            return inputDeps;
        }
        #region Jobs


        // check if any monsters are in the scope and run to the character
        struct CheckOnCall : IJobForEach<Coordinate> {
            public void Execute( ref Coordinate c0)
            {

            }
        }

        // check if any monsters got shot
        struct CheckDamage : IJobForEach<Coordinate> {
            public void Execute(ref Coordinate c0)
            {
            }
        }

        struct MonsterPositionUpdate : IJobForEach<Coordinate, MonsterMovingCoordinate, MonsterTypeIndex>
        {
            public void Execute(ref Coordinate c0, ref MonsterMovingCoordinate c1,ref MonsterTypeIndex c2)
            {
                if (c2.Value == MonsterType.Green)
                {
                    System.Numerics.Vector3 startPoint = new System.Numerics.Vector3(c1.X, c1.Y, c1.Z);
                    System.Numerics.Vector3 endPoint = new System.Numerics.Vector3(c1.X2, c1.Y2, c1.Z2);
                    float dev = Mathf.Abs((m_MusicAudioSource.time - m_Music.MusicInfo.MusicStartTime) % m_Music.MusicInfo.MusicBeatsTime);
                    float fractionOfJourney = (dev) / AudioSystem.Music.MusicInfo.MusicBeatsTime;
                    //float JourneyLength = System.Numerics.Vector3.Distance(startPoint, endPoint);
                    System.Numerics.Vector3 lerp = System.Numerics.Vector3.Lerp(startPoint, endPoint, fractionOfJourney);
                    c1.X = c0.X;
                    c1.Y = c0.Y;
                    c1.Z = c0.Z;
                    c1.X2 = c0.X;
                    c1.Y2 = c0.Y;
                    c1.Z2 = c0.Z;
                    c1.X2 += 1;
                    c1.Y2 += 1;
                    c1.Z2 += 1;
                }
            }

        }
        #endregion


    }
}