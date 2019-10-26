using System.Collections;
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
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            //在这里我们设置EnittyManager，因为EntityManager全局只有一个并且在程序运行时自动创建，在这里我们是存储一下它的reference。
            m_EntityManager = EntityManager;
            Enabled = false;
        }

        public void Init()
        {
            ShutDown();
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
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        #region Methods
        #endregion

        #region Jobs

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
        struct SetTrapMotion : IJobForEach<TypeOfTile, TextureIndex, Coordinate, Timer>
        {
            public int mode;
            public int counter;
            public void Execute(ref TypeOfTile c0, ref TextureIndex c1, ref Coordinate c2, ref Timer c3)
            {
                if (counter == -1)
                {
                    c1.Value = mode;
                    return;
                }
                else if ((c2.X + c2.Y + counter) % 3 == 0 && c0.Value == TileType.NailTrap)
                {
                    c1.Value = mode;
                    if (c1.Value == 1)
                    {
                        c3.isOn = true;
                        c3.T = 0;
                        c3.maxT = 0.2f;
                    }
                }else if(counter%2==0 && c0.Value == TileType.MusicAccleratorTrap)
                {
                    c1.Value = mode;
                    if (c1.Value == 1)
                    {
                        c3.isOn = true;
                        c3.T = 0;
                        c3.maxT = 0.2f;
                    }
                }
            }
        }

        #endregion

        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {

            inputDeps = new SetTrapMotion
            {
                mode = 1,
                counter = beatCounter
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            _ResetNails = true;
            _ResetNailsTimer = 0.2f;

            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            inputDeps.Complete();
            return inputDeps;
        }
        private static bool _ResetNails;
        private static float _ResetNailsTimer;
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_ResetNails)
            {
                if (_ResetNailsTimer > 0)
                {
                    _ResetNailsTimer -= Time.deltaTime;
                }
                else
                {
                    _ResetNails = false;
                    inputDeps = new SetTrapMotion
                    {
                        counter = -1,
                        mode = 0
                    }.Schedule(this, inputDeps);
                    inputDeps.Complete();
                }
            }

            inputDeps = new CopyDisplayColor { }.Schedule(this, inputDeps);
            return inputDeps;
        }

    }
}
