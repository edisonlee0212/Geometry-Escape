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

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}