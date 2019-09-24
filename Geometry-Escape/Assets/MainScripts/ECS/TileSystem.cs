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
    /// The system that control all tiles.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class TileSystem : JobComponentSystem
    {
        /* #region 是C#的一项功能，可以将代码划分区块，方便代码架构设计。
        *根据我的经验，我在每个系统里面都加入了：
        *Private（承载所有不对外开放的成员变量），
        * Public（对外开放的成员变量），
        * Managers（OnCreate, Init, ShutDown,OnDestroy四个涉及对系统状态更改的方法）
        * Methods（所有可以被外界调用的方法归在这里）
        * Jobs（所有本系统的功能）
        * 所有系统内的private方法都放在OnUpdate之后，这些方法的类似Helper functions，不可被外界调用，只是能减少其他部分的代码量。
        */
        #region Private
        /// <summary>
        /// ECS讲究的是高效的利用多线程对一个符合条件的一整个集合中的每一个entity进行数据运算和修改，但是涉及到对于单个entity的操作，我们多利用entitymanager来完成，例如创建一个entity，消灭一个entity，修改某单个entity的某项数据等等。
        /// entitymanager也同时管理所有和entity相关的其他事务。
        /// </summary>
        private static EntityManager m_EntityManager;

        #endregion

        #region Public
        private static NativeArray<Entity> _CenterEntity;
        public static Entity CenterEntity { get => _CenterEntity[0]; set => _CenterEntity[0] = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            //在这里我们设置EnittyManager，因为EntityManager全局只有一个并且在程序运行时自动创建，在这里我们是存储一下它的reference。
            m_EntityManager = EntityManager;
        }

        public void Init()
        {
            /*所有的Init第一步都是shutdown，这是因为我过去的经验
             * 如果我们要重启某个系统，但是我们忘记了先shutdown在init，那么所有的nativecontainer就会被重新分配，原先的没有被删除，就造成了内存泄漏。
             * 例如这里的NativeArray。
             * 所以在Init之前首先调用ShutDown，保证所有之前可能的遗留内存确认清理。
             */
            ShutDown();
            
            /*这个centerentity一直存储着位于视野中心的砖块，这个由PositionSelect这个Job更新
             */
            _CenterEntity = new NativeArray<Entity>(1, Allocator.Persistent);
            

            /*当我们初始化完毕，就可以开始运行这个系统了。
             */
            Enabled = true;
        }

        public void ShutDown()
        {
            /*内存清理，”如果_CenterEntity这上面被分配了内存，我们就清理它。“
             * IsCreated是所有NativeContainer都拥有的属性，代表这个container是否由内容。
             * 具体关于NativeContainer，可以查看如下网址：
             * https://docs.unity3d.com/Manual/JobSystemNativeContainer.html
             * https://jacksondunstan.com/articles/4713
             */
            if (_CenterEntity.IsCreated) _CenterEntity.Dispose();
            /*保证所有清理工作完成后，我们中断系统运行。
             */
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        #region Methods
        #endregion

        #region Jobs
        /*以下的BurstCompile是一个标记，代表这个Jobs由BurstCompiler来直接编译成高效的汇编代码，跳过C#的编译器
         * BurstCompiler专门为游戏设计，其效率在大多数情况下甚至高于C。
         * 以下是关于burst compiler的介绍：
         * https://docs.unity3d.com/Packages/com.unity.burst@0.2/manual/index.html
         * https://xoofx.com/blog/2019/03/28/behind-the-burst-compiler/
         * 【只有ECS的Jobs的代码支持Burst。】
         */
        [BurstCompile]
        /*
         * 关于Jobs（IjobForEach IJobForEachWithEntity）：
         * https://docs.unity3d.com/Packages/com.unity.entities@0.0/manual/entity_iteration_job.html
         */
        struct PositionSelect : IJobForEachWithEntity<Translation, DefaultColor, TileProperties>
        {
            [ReadOnly] public float scale;
            [ReadOnly] public Vector3 position;
            /// <summary>
            /// 这个地方我解释一下为什么要用NativeArray去存储中心entity，以及为什么要有[NativeDisableParallelForRestriction]
            /// 因为这里的job是原生多线程运行，所以为了效率和安全，unity强制要求jobs内只能包含primitive type或者由primitive type组成的struct还有nativecontainer
            /// 在job内，所有参数传入时都是pass by value，只有nativecontainer是pass by reference，如果你在job内修改了例如这里的scale值，在这个job之外的scale的值是不会被改变的。
            /// 如果没有nativecontainer，我们就不能从job向外界发送信息，只能修改entity的信息。
            /// 而nativecontainer是传入的nativecontainer的reference，所以在这里修改nativecontainer的内容，在jobs外面这个结果也会被保留。
            /// 但我们知道，IJobforeachwithentity是多线程的，这就可能会导致io conflict，比如多个线程同时写入某个nativecontainer的某一项的值，这样就会导致问题
            /// 所以unity默认不允许用户在jobs内向nativecontainer写入数据，只允许读取。
            /// 但是在这里，我们清楚只有一个tile位于中心，无论有几个线程，只会有一个线程在对这个砖块操作时会往这个container写入数据，我们自己确认这是安全的，所以我们打上[NativeDisableParallelForRestriction]标记
            /// 告诉unity，我们明白我们在做什么，出了问题我们自己负责。这样unity才会允许我们在多线程的job内向container写入数据。
            /// </summary>
            [NativeDisableParallelForRestriction]
            [WriteOnly] public NativeArray<Entity> selectedEntity;
            /*每个Job都必须含有Execute，这个是job的具体工作，execute格式不需要自己写，对着上面的”IJobForEachwithentity”按alt+enter会有选项自动帮你创建。
             * 注意这个地方的【ReadOnly】和WriteOnly，这两个标记告诉Unity我们只会读取这个数据或者我们只会写入这个数据，这个是optional的，但是会提高运行效率。
             */


            public void Execute(Entity entity, int index, [ReadOnly] ref Translation c0, [WriteOnly] ref DefaultColor c1, [ReadOnly] ref TileProperties c2)
            {
                if (Mathf.Abs(c0.Value.x - position.x) < scale && Mathf.Abs(c0.Value.y - position.y) < scale / 4)
                {
                    //如果这个砖块处于中心，我们把它存到container里面
                    selectedEntity[0] = entity;
                    //并且高亮这个砖块
                    c1.Value = new float4(1);
                }
            }
        }

        [BurstCompile]
        struct RotateTileTest1 : IJobForEach<Coordinate, TileProperties>
        {
            [ReadOnly] public int counter;
            public void Execute([WriteOnly] ref Coordinate c0, [ReadOnly] ref TileProperties c1)
            {
                if (c0.X % 2 == 0) c0.Direction = counter * 90;
            }
        }

        [BurstCompile]
        struct ChangeColorTest : IJobForEach<TileProperties, DefaultColor>
        {
            [ReadOnly] public int counter;
            public void Execute([ReadOnly] ref TileProperties c0, [WriteOnly] ref DefaultColor c1)
            {
                Vector4 color = default;
                int offset = c0.Index + counter;
                color.x = offset * 32 % 256 / 256f;
                color.y = (offset * 32 + 64) % 256 / 256f;
                color.z = (offset * 32 + 128) % 256 / 256f;
                color.w = 1;
                c1.Value = color;
            }
        }

        [BurstCompile]
        struct ChangeTextureInfoTest : IJobForEach<TextureIndex, TextureMaxIndex, TileProperties>
        {
            [ReadOnly] public int counter;
            public void Execute([WriteOnly] ref TextureIndex c0, [ReadOnly] ref TextureMaxIndex c1, [ReadOnly] ref TileProperties c2)
            {
                c0.Value = counter % c1.Value;
            }
        }

        [BurstCompile]
        struct CopyDisplayColor : IJobForEach<TileProperties, DefaultColor, DisplayColor>
        {
            public void Execute(ref TileProperties c0, ref DefaultColor c1, ref DisplayColor c2)
            {
                c2.Value = c1.Value;
            }
        }
        [BurstCompile]
        struct SetAllNailTrap : IJobForEach<TileTypeIndex, TextureIndex>
        {
            public int mode;
            public void Execute(ref TileTypeIndex c0, ref TextureIndex c1)
            {
                if (c0.Value == TileType.NailTrap) c1.Value = mode;
            }
        }

        #endregion

        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {
            /*inputDeps = new RotateTileTest1
            {
                counter = beatCounter,
            }.Schedule(this, inputDeps);
            inputDeps = new ChangeColorTest
            {
                counter = beatCounter,
            }.Schedule(this, inputDeps);
            */
            inputDeps = new SetAllNailTrap
            {
                mode = beatCounter % 2
            }.Schedule(this, inputDeps);

            inputDeps.Complete();
            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            /*inputDeps = new ChangeTextureInfoTest
            {
                counter = counter,
            }.Schedule(this, inputDeps);*/
            inputDeps.Complete();
            return inputDeps;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _CenterEntity[0] = Entity.Null;

            inputDeps = new PositionSelect
            {
                scale = CentralSystem.Scale / CentralSystem.CurrentZoomFactor,
                position = new Vector3(0, 0, 0),
                selectedEntity = _CenterEntity
            }.Schedule(this, inputDeps);
            inputDeps.Complete();

            inputDeps = new CopyDisplayColor { }.Schedule(this, inputDeps);
            return inputDeps;
        }
    }
}
