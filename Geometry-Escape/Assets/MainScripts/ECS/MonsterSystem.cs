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
        private static CentralSystem m_CentralSystem;

        private static EntityArchetype _MonsterEntityArchetype;

        private int _MonsterCount;
        private int _MonsterMaterAmount;

        private static NativeQueue<MonsterInfo> _MonsterCreationQueue;
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
            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            //Schedule your job for every time step here. Time step is defined in central system.
            return inputDeps;
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
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
        #endregion


    }
}