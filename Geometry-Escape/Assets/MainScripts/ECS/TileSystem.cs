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
        private static bool _Moving, _Zooming;
        private static float3 _CurrentCenterPosition;
        private static float _CurrentZoomFactor;
        private static float _Timer;
        private static int _BeatCounter;
        private static int _Counter;
        private static float _TimeStep;
        private static float _TileScale;
        private static Transform m_Light;
        private static NativeArray<Entity> _CenterEntity;
        public static float TileScale { get => _TileScale; set => _TileScale = value; }
        public static float Timer { get => _Timer; set => _Timer = value; }
        public static int BeatCounter { get => _BeatCounter; set => _BeatCounter = value; }
        public static float TimeStep { get => _TimeStep; set => _TimeStep = value; }
        public static float3 CurrentCenterPosition { get => _CurrentCenterPosition; set => _CurrentCenterPosition = value; }
        public static float CurrentZoomFactor { get => _CurrentZoomFactor; set => _CurrentZoomFactor = value; }
        public static Entity CenterEntity { get => _CenterEntity[0]; set => _CenterEntity[0] = value; }
        public static bool Moving { get => _Moving; set => _Moving = value; }
        public static bool Zooming { get => _Zooming; set => _Zooming = value; }
        public static Transform Light { get => m_Light; set => m_Light = value; }
        public static int Counter { get => _Counter; set => _Counter = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            //在这里我们设置EnittyManager，因为EntityManager全局只有一个并且在程序运行时自动创建，在这里我们是存储一下它的reference。
            m_EntityManager = World.Active.EntityManager;
        }

        public void Init()
        {
            /*所有的Init第一步都是shutdown，这是因为我过去的经验
             * 如果我们要重启某个系统，但是我们忘记了先shutdown在init，那么所有的nativecontainer就会被重新分配，原先的没有被删除，就造成了内存泄漏。
             * 例如这里的NativeArray。
             * 所以在Init之前首先调用ShutDown，保证所有之前可能的遗留内存确认清理。
             */
            ShutDown();
            /* 设置灯光，因为地图具有缩放功能，在地图缩放的时候灯光范围也应该随之更改，所以在这里加入引用。
             */
            m_Light = CentralSystem.LightResources.ViewLight.transform;
            /*这个centerentity一直存储着位于视野中心的砖块，这个由PositionSelect这个Job更新
             */
            _CenterEntity = new NativeArray<Entity>(1, Allocator.Persistent);
            /*
             * 地图放大倍数
             */
            _CurrentZoomFactor = 1;

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
        struct PositionSelect : IJobForEachWithEntity<Translation, DefaultColor>
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
            public void Execute(Entity entity, int index, [ReadOnly] ref Translation c0, [WriteOnly] ref DefaultColor c1)
            {

                if (Mathf.Abs(c0.Value.x - position.x) < scale && Mathf.Abs(c0.Value.y - position.y) < scale / 4)
                {
                    //如果这个砖块处于中心，我们把它存到container里面
                    selectedEntity[0] = entity;
                    //并且高亮这个砖块
                    c1.Color = new float4(1);
                }
            }
        }

        [BurstCompile]
        struct CalculateTileLocalToWorld : IJobForEach<Coordinate, Scale, Translation, Rotation>
        {
            [ReadOnly] public float scale;
            [ReadOnly] public float3 centerPosition;
            public void Execute([ReadOnly] ref Coordinate c0, [WriteOnly] ref Scale c1, [WriteOnly] ref Translation c2, [WriteOnly] ref Rotation c3)
            {
                var coordinate = c0;
                c1.Value = scale;
                c2.Value = new float3((coordinate.X + centerPosition.x) * scale, (coordinate.Y + centerPosition.y) * scale, (coordinate.Z + centerPosition.z) * scale);
                c3.Value = Quaternion.Euler(0, 0, coordinate.Direction);
            }
        }

        [BurstCompile]
        struct RotateTileTest1 : IJobForEach<Coordinate>
        {
            [ReadOnly] public int counter;
            public void Execute([WriteOnly] ref Coordinate c0)
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
                c1.Color = color;
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
        #endregion

        protected JobHandle OnBeatUpdate(JobHandle inputDeps)
        {
            inputDeps = new RotateTileTest1
            {
                counter = BeatCounter,
            }.Schedule(this, inputDeps);
            inputDeps = new ChangeColorTest
            {
                counter = BeatCounter,
            }.Schedule(this, inputDeps);

            inputDeps.Complete();
            return inputDeps;
        }

        protected JobHandle OnFixedUpdate(JobHandle inputDeps)
        {
            inputDeps = new ChangeTextureInfoTest
            {
                counter = Counter,
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            #region Time
            _Timer += Time.deltaTime;
            if (_Timer >= TimeStep)
            {
                _Counter += (int)(_Timer / _TimeStep);
                _Timer = 0;
                OnFixedUpdate(inputDeps);
            }
            #endregion

            #region Beat
            int count = AudioSystem.CurrentBeatCounter();
            if (count != _BeatCounter)
            {
                AudioSystem.m_BeatsAudioSource.Play();
                _BeatCounter = count;
                OnBeatUpdate(inputDeps);
            }
            #endregion

            #region InputSystem

            if (_Moving) OnMoving();
            if (_Zooming) OnZooming();

            #endregion

            inputDeps = new CalculateTileLocalToWorld
            {
                scale = _TileScale / _CurrentZoomFactor,
                centerPosition = _CurrentCenterPosition
            }.Schedule(this, inputDeps);

            inputDeps.Complete();

            _CenterEntity[0] = Entity.Null;

            inputDeps = new PositionSelect
            {
                scale = _TileScale / _CurrentZoomFactor,
                position = new Vector3(0, 0, 0),
                selectedEntity = _CenterEntity
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }

        #region Variables for InputSystem

        private static float _MovementTimer, _ZoomingTimer;
        private static float _PreviousZoomFactor, _TargetZoomFactor;
        private static Vector3 _PreviousOriginPosition, _TargetOriginPosition;
        #endregion

        private void OnMoving()
        {
            _MovementTimer += Time.deltaTime;
            if (_MovementTimer < 0.2f)
            {
                _CurrentCenterPosition = Vector3.Lerp(_PreviousOriginPosition, _TargetOriginPosition, _MovementTimer / 0.2f);
            }
            else
            {
                _Moving = false;
                _CurrentCenterPosition = _TargetOriginPosition;
            }

        }

        private void OnZooming()
        {
            _ZoomingTimer += Time.deltaTime;
            if (_ZoomingTimer <= 0.1f)
            {
                _CurrentZoomFactor = (_TargetZoomFactor * _ZoomingTimer + _PreviousZoomFactor * (0.1f - _ZoomingTimer)) / 0.1f;
            }
            else
            {
                _Zooming = false;
                _CurrentZoomFactor = _TargetZoomFactor;
            }
            Vector3 position = m_Light.position;
            position.z = -5 / _CurrentZoomFactor;
            m_Light.position = position;
        }

        public static void Zoom(float direction)
        {
            if (!_Zooming)
            {
                //Debug.Log(direction);
                if (direction != 0)
                {
                    _Zooming = true;
                    _ZoomingTimer = 0;
                    _PreviousZoomFactor = _CurrentZoomFactor;
                    if (direction > 0 && _CurrentZoomFactor < 16f)
                    {
                        _TargetZoomFactor = _PreviousZoomFactor;
                        _TargetZoomFactor *= 2;
                    }
                    else if (direction < 0 && _CurrentZoomFactor > 0.5f)
                    {
                        _TargetZoomFactor = _PreviousZoomFactor;
                        _TargetZoomFactor /= 2;
                    }
                }
            }
        }

        public static void Move(Vector2 direction)
        {
            if (!_Moving && (ControlSystem.ControlMode == ControlMode.MapEditor || _CenterEntity[0] != Entity.Null))
            {
                Debug.Log(AudioSystem.OnBeats());
                if ((AudioSystem.OnBeats() || ControlSystem.ControlMode == ControlMode.MapEditor) && direction != Vector2.zero && direction.x * direction.y == 0)
                {
                    _Moving = true;
                    _MovementTimer = 0;
                    _PreviousOriginPosition = _CurrentCenterPosition;
                    _TargetOriginPosition = _PreviousOriginPosition;
                    if (direction.x > 0 && (ControlSystem.ControlMode == ControlMode.MapEditor
                        || m_EntityManager.GetComponentData<RightTile>(_CenterEntity[0]).Value != Entity.Null))
                    {
                        _TargetOriginPosition.x -= 1;
                        Debug.Log("Move right, target position: " + (-_TargetOriginPosition));
                    }
                    else if (direction.x < 0 && (ControlSystem.ControlMode == ControlMode.MapEditor
                        || m_EntityManager.GetComponentData<LeftTile>(_CenterEntity[0]).Value != Entity.Null))
                    {
                        _TargetOriginPosition.x += 1;
                        Debug.Log("Move left, target position: " + (-_TargetOriginPosition));
                    }
                    else if (direction.y > 0 && (ControlSystem.ControlMode == ControlMode.MapEditor
                        || m_EntityManager.GetComponentData<UpTile>(_CenterEntity[0]).Value != Entity.Null))
                    {
                        _TargetOriginPosition.y -= 1;
                        Debug.Log("Move up, target position: " + (-_TargetOriginPosition));
                    }
                    else if (direction.y < 0 && (ControlSystem.ControlMode == ControlMode.MapEditor
                        || m_EntityManager.GetComponentData<DownTile>(_CenterEntity[0]).Value != Entity.Null))
                    {
                        _TargetOriginPosition.y += 1;
                        Debug.Log("Move down, target position: " + (-_TargetOriginPosition));
                    }
                    else
                    {
                        Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                    }
                }
            }

        }
    }
}
