using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
namespace GeometryEscape
{
    /// <summary>
    /// The system that control all monsters.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class MonsterSystem : JobComponentSystem
    {
        #region Private
        #endregion

        #region Public
        #endregion
        
        #region Managers
        protected override void OnCreate()
        {
            
        }

        public void Init()
        {
            ShutDown();
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
    }
}