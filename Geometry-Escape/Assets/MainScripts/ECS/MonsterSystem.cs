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

        #region Methods
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
        public void RouteOnCall(Entity entity) {
            /** need an algorithm to calculate 
            https://medium.com/@jimmy0x52/making-smarter-monsters-adding-pathfinding-to-unitys-2d-roguelike-tutorial-5c004207a7a3
                       
            */

        }


        private void CreateMonster(JobHandle inputDeps, MonsterInfo monsterInfo)
        {
            var materialIndex = monsterInfo.MaterialIndex;
            var initialCoordinate = monsterInfo.Coordinate;
            var monsterType = monsterInfo.MonsterType;
            if (materialIndex < 0 || materialIndex >= _MonsterMaterAmount)
            {
                Debug.LogError("AddTile: Wrong material index: " + materialIndex);
                return;
            }
            var color = new DefaultColor { };
            //color.Color = Vector4.one;
            var textureInfo = new TextureIndex
            {
                Value = 1
            };
            /*var renderMaterialIndex = new RenderMaterialIndex
            {
               Value = materialIndex
            };*/
            //int maxIndex = 0;
            //switch (monsterInfo.MaterialIndex)
            //{
            //    case 2:
            //        maxIndex = 25;
            //        break;
            //    case 3:
            //        maxIndex = 23;
            //        break;
            //    default:
            //        maxIndex = 1;
            //        break;
            //}
            var maxTextureIndex = new TextureMaxIndex
            {
                Value = 1
            };
            Entity instance = EntityManager.CreateEntity(_MonsterEntityArchetype);
            //NativeArray<LeftTile> left = new NativeArray<LeftTile>(1, Allocator.TempJob);
            //NativeArray<RightTile> right = new NativeArray<RightTile>(1, Allocator.TempJob);
            //NativeArray<UpTile> up = new NativeArray<UpTile>(1, Allocator.TempJob);
            //NativeArray<DownTile> down = new NativeArray<DownTile>(1, Allocator.TempJob);
            //inputDeps = new LocateLeftTilesJob
            //{
            //    leftTile = left,
            //    coordinate = initialCoordinate,
            //    originEntity = instance,
            //    mode = 1
            //}.Schedule(this, inputDeps);
            //inputDeps = new LocateRightTilesJob
            //{
            //    rightTile = right,
            //    coordinate = initialCoordinate,
            //    originEntity = instance,
            //    mode = 1
            //}.Schedule(this, inputDeps);
            //inputDeps = new LocateUpTilesJob
            //{
            //    upTile = up,
            //    coordinate = initialCoordinate,
            //    originEntity = instance,
            //    mode = 1
            //}.Schedule(this, inputDeps);
            //inputDeps = new LocateDownTilesJob
            //{
            //    downTile = down,
            //    coordinate = initialCoordinate,
            //    originEntity = instance,
            //    mode = 1
            //}.Schedule(this, inputDeps);
            //inputDeps.Complete();


            //EntityManager.SetComponentData(instance, left[0]);
            //EntityManager.SetComponentData(instance, right[0]);
            //EntityManager.SetComponentData(instance, up[0]);
            //EntityManager.SetComponentData(instance, down[0]);
            //left.Dispose();
            //right.Dispose();
            //up.Dispose();
            //down.Dispose();

            //EntityManager.SetSharedComponentData(instance, renderMaterialIndex);
            EntityManager.SetComponentData(instance, initialCoordinate);
            EntityManager.SetComponentData(instance, color);
            EntityManager.SetComponentData(instance, textureInfo);
            EntityManager.SetComponentData(instance, maxTextureIndex);
            var properties = new MonsterProperties
            {
                MonsterType = monsterType,
                //Coordinate = 
                MaterialIndex=materialIndex 
            };
            _MonsterCount++;
            EntityManager.SetComponentData(instance, properties);
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