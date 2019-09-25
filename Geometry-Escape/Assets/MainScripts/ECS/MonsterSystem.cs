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
        #endregion
        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {
            //Schedule your job for every beat here.

            return inputDeps;
        }

        public JobHandle MonsterFixedUpdate(ref JobHandle inputDeps) {
            
            _beatTime = Time.time;
            inputDeps = new MonsterPositionUpdate
            {
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

            _currentTime = Time.time;
            _Timer += Time.deltaTime;
            if (_Timer >= _TimeStep)
            {
                Debug.Log("**********");
                _Counter += (int)(_Timer / _TimeStep);
                _Timer = 0;
                MonsterFixedUpdate(ref inputDeps);
            }
            MonsterRoute();

            inputDeps = new SetRoutePosition
            {
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

        struct MonsterPositionUpdate : IJobForEach<Coordinate,  MonsterTypeIndex>
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
        struct SetRoutePosition : IJobForEach<Coordinate>
        {
            public void Execute(ref Coordinate c0)
            {
                c0.X = lerp.x;
                c0.Y = lerp.y;
                c0.Z = lerp.z;
                Debug.Log("monster current coordinate:" + (c0.X));
            }

        }
        #endregion


    }
}