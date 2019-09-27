using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Mathematics.math;
namespace GeometryEscape
{
    /// <summary>
    /// The system that control all other systems.
    /// </summary>
    [AlwaysUpdateSystem]
    public class CentralSystem : JobComponentSystem
    {
        #region Private
        private static EntityManager m_EntityManager;
        private static Transform m_Light;
        

        private static ControlMode _SavedControlMode;
        #endregion

        #region Public

        #region Sub-Systems
        /*
         * 下面是各种系统的引用，虽然系统内大部分成员变量都是static全局的变量，但是系统本身不是static的，因为unity允许多个相同系统的存在。所以在这里建立和各个子系统的链接。
         */
        private static MeshRenderSystem m_RenderSystem;
        private static TileSystem m_TileSystem;
        private static WorldSystem m_WorldSystem;
        private static ControlSystem m_ControlSystem;
        private static AudioSystem m_AudioSystem;
        private static MonsterSystem m_MonsterSystem;
        private static UISystem m_UISystem;
        private static CopyTextureIndexSystem m_CopyTextureIndexSystem;
        private static CopyDisplayColorSystem m_CopyDisplayColorSystem;
        #endregion

        #region Resources
        /* Resource作为ECS与非ECS内容的媒介，可以帮助ECS创建Mono GameObject或者引入美术及音乐资源以及导入数据。
         */
        /// <summary>
        /// 下面三个都是scriptobject，用来导入特殊数据，比如说音乐，图像材质等等，具体关于scriptable object可以Google一下。一句话概括，因为unity原来只有在场景里面存在的gameobject才有能力通过“Instanciate”
        /// 方法生成新的gameobject，但是我们常常需要a生成b，b生成c但是b不需要在场景里有实体，所以有了scriptable object，不需要在场景里存在即可生成物体。
        /// 但是这里我使用scriptable object是用来存储数据。虽然unity直接支持Resources方法直接读取asset，但是利用一个resource作为媒介使用更加方便，这样你就可以直接往在Resource文件夹内的ScriptableObjects里面的三个object拖拽素材
        /// Light resources 存储各类光照，我们的视野系统就是一个只照亮中心区域的锥形光。
        /// </summary>
        private static LightResources m_LightResources;
        /// <summary>
        /// tile resources 存储各类砖块材质，我们以后设计不同的砖块最后就放到这个里面。
        /// </summary>
        private static TileResources m_TileResources;
        /// <summary>
        /// music resources 存储各种音乐素材和对应beats
        /// </summary>
        private static AudioResources m_AudioResources;
        /// <summary>
        /// tile resources 存储各类怪物材质，我们以后设计不同的砖块最后就放到这个里面。
        /// </summary>
        private static MonsterResources m_MonsterResources;
        /// <summary>
        /// 这里面存储所有主角相关的GameObject。
        /// </summary>
        private static MainCharacterResources m_MainCharacterResources;

        #endregion

        #region Timers and Counters and Others
        /* 这部分保存了所有与计数有关或者与时间相关的参数
         * 
         */
        private static float _TimeStep;
        private static int _Counter;
        private static int _BeatCounter;
        private static CharacterController m_MainCharacterController;
        private static float _Timer;

        private static bool _Running;

        #endregion

        #region Variables for Moving and Zooming
        private static bool _Moving, _Zooming;
        private static bool _CheckTile;
        private static bool _InverseDirection;
        private static int _FreezeCount;
        private static float3 _CurrentCenterPosition;
        private static float _CurrentZoomFactor;
        private static float _Scale;

        private static float _MovementTimer, _ZoomingTimer;
        private static float _PreviousZoomFactor, _TargetZoomFactor;
        private static Vector3 _PreviousOriginPosition, _TargetOriginPosition;

        #endregion

        #region Getters and Setters
        public static MeshRenderSystem RenderSystem { get => m_RenderSystem; set => m_RenderSystem = value; }
        public static TileSystem TileSystem { get => m_TileSystem; set => m_TileSystem = value; }
        public static WorldSystem WorldSystem { get => m_WorldSystem; set => m_WorldSystem = value; }
        public static ControlSystem ControlSystem { get => m_ControlSystem; set => m_ControlSystem = value; }
        public static AudioSystem AudioSystem { get => m_AudioSystem; set => m_AudioSystem = value; }
        public static MonsterSystem MonsterSystem { get => m_MonsterSystem; set => m_MonsterSystem = value; }
        public static UISystem UISystem { get => m_UISystem; set => m_UISystem = value; }
        public static CopyTextureIndexSystem CopyTextureIndexSystem { get => m_CopyTextureIndexSystem; set => m_CopyTextureIndexSystem = value; }
        public static LightResources LightResources { get => m_LightResources; set => m_LightResources = value; }
        public static TileResources TileResources { get => m_TileResources; set => m_TileResources = value; }
        public static AudioResources AudioResources { get => m_AudioResources; set => m_AudioResources = value; }
        public static MonsterResources MonsterResources { get => m_MonsterResources; set => m_MonsterResources = value; }
        public static MainCharacterResources MainCharacterResources { get => m_MainCharacterResources; set => m_MainCharacterResources = value; }
        public static float Scale { get => _Scale; set => _Scale = value; }
        public static float3 CurrentCenterPosition { get => _CurrentCenterPosition; set => _CurrentCenterPosition = value; }
        public static float CurrentZoomFactor { get => _CurrentZoomFactor; set => _CurrentZoomFactor = value; }
        public static bool Moving { get => _Moving; set => _Moving = value; }
        public static bool Zooming { get => _Zooming; set => _Zooming = value; }
        public static bool CheckTile { get => _CheckTile; set => _CheckTile = value; }
        public static int FreezeCount { get => _FreezeCount; set => _FreezeCount = value; }
        public static bool InverseDirection { get => _InverseDirection; set => _InverseDirection = value; }
        public static CopyDisplayColorSystem CopyDisplayColorSystem { get => m_CopyDisplayColorSystem; set => m_CopyDisplayColorSystem = value; }
        public static CharacterController MainCharacterController { get => m_MainCharacterController; set => m_MainCharacterController = value; }
        public static int BeatCounter { get => _BeatCounter; set => _BeatCounter = value; }
        public static int Counter { get => _Counter; set => _Counter = value; }
        public static float Timer { get => _Timer; set => _Timer = value; }
        public static float TimeStep { get => _TimeStep; set => _TimeStep = value; }

        public static bool Running { get => _Running; set => _Running = value; }

        #endregion

        #endregion

        #region Managers
        /// <summary>
        /// 每个继承JobComponentSystem的系统都拥有OnCreate，OnDestroy方法，这两个方法在系统建立时和系统被删除时会被调用。ecs的所有系统默认随程序启动，所以在程序运行时所有系统都会被调用OnCreate方法。
        /// 在程序结束的时候所有系统也会被调用ondestroy方法。
        /// </summary>
        protected override void OnCreate()
        {
            m_EntityManager = EntityManager;
            //如果你观察所有其他系统的OnCreate函数，
            //你会发现它们都只包含Enabled = false，即所有子系统虽然在程序开始时都被自动建立，但是所有子系统在开始时都会中止运行。
            //直到centralsystem结束所有必要设置后由centralsystem开启各个子系统，这些初始设置及启动子系统的步骤包含在Init内。
            
            #region Connect other systems
            UISystem = Object.FindObjectOfType<UISystem>();
            RenderSystem = World.Active.GetOrCreateSystem<MeshRenderSystem>();//从unity获取当前已经存在的系统
            TileSystem = World.Active.GetOrCreateSystem<TileSystem>();
            MonsterSystem = World.Active.GetOrCreateSystem<MonsterSystem>();
            WorldSystem = World.Active.GetOrCreateSystem<WorldSystem>();
            AudioSystem = World.Active.GetOrCreateSystem<AudioSystem>();
            CopyTextureIndexSystem = World.Active.GetOrCreateSystem<CopyTextureIndexSystem>();
            CopyDisplayColorSystem = World.Active.GetOrCreateSystem<CopyDisplayColorSystem>();

            #endregion

            SceneManager.LoadScene("MainMenu");
            Enabled = false;
            //Init();
        }
        public void Init()
        {
            
            #region Load Resources
            //首先我们载入resources，准备好要分配给各个子系统的资源
            m_LightResources = Resources.Load<LightResources>("ScriptableObjects/LightResources");
            m_TileResources = Resources.Load<TileResources>("ScriptableObjects/TileResources");
            m_AudioResources = Resources.Load<AudioResources>("ScriptableObjects/AudioResources");
            m_MonsterResources = Resources.Load<MonsterResources>("ScriptableObjects/MonsterResources");
            m_MainCharacterResources = Resources.Load<MainCharacterResources>("ScriptableObjects/MainCharacterResources");
            #endregion
            #region Initial Settings
            /* 设置灯光，因为地图具有缩放功能，在地图缩放的时候灯光范围也应该随之更改，所以在这里加入引用。
             */
            m_Light = LightResources.ViewLight.transform;

            /* 设置主角，主角动画及大小随地图缩放和玩家操作改变，这里加入引用。
             */
            MainCharacterController = m_MainCharacterResources.MainCharacterController;
            /*
             * 地图放大倍数
             */
            _CurrentZoomFactor = 1;
            _CurrentCenterPosition = Unity.Mathematics.float3.zero;
            //设置砖块系统的一些初始参数，tilescale值砖块的大小，你可以尝试改变这个值，看看有什么效果。
            Scale = 2;
            //设置砖块系统的单位时间，砖块系统内有对应的OnUpdate和OnFixedUpdate，对应unity原本的Update和FixedUpdate
            //其中OnUpdate由系统自动调用，OnFixedUpdate为我写的方法，由OnUpdate调用，这里就是调用时间间隔，0.1f代表每秒执行10次OnFixedUpdate
            TimeStep = 0.1f;
            #endregion
            #region Initialize sub-systems
            //确认所有必须设置的初始参数设置完毕，我们可以启动各个子系统。
            RenderSystem.Init();//启动这个系统
            TileSystem.Init();

            MonsterSystem.Init();
            WorldSystem.TileResources = m_TileResources;
            WorldSystem.MonsterResources = m_MonsterResources;
            WorldSystem.Init();
            AudioSystem.Init();

            ControlSystem = new ControlSystem();//control system并不是一个真正的ECS的系统，所以我们通过这种方式建立。
            CopyTextureIndexSystem.Init();
            CopyDisplayColorSystem.Init();
            #endregion
            //这个地方设置操作模式，不同操作模式对应不同场景。

            ControlSystem.ControlMode = ControlMode.MapEditor;
            Running = true;

            Enabled = true;
        }

        /// <summary>
        /// This method stops all ECS systems from running and disable all control in control system.
        /// </summary>
        public static void Pause()
        {

            if (!Running) return;
            _SavedControlMode = ControlSystem.ControlMode;
            ControlSystem.ControlMode = ControlMode.NoControl;
            Running = false;

            MainCharacterController.Pause();
            TileSystem.Pause();
            MonsterSystem.Pause();
            AudioSystem.Pause();

            WorldSystem.Pause();

        }

        /// <summary>
        /// This method resume all ECS systems and control system.
        /// </summary>
        public static void Resume()
        {

            if (Running) return;
            ControlSystem.ControlMode = _SavedControlMode;
            Running = true;

            MainCharacterController.Resume();
            TileSystem.Resume();
            MonsterSystem.Resume();
            AudioSystem.Resume();

            WorldSystem.Resume();

        }

        /// <summary>
        /// 所有系统包含shutdown函数，这个函数包括Init是我的习惯，这两个函数对应”不希望删除整个系统只是中止运行或者恢复运行“这种需求。
        /// 对于中央系统，在关闭中央系统时中央系统会先关闭所有其他子系统，保证子系统正确关闭，没有内存泄露。
        /// （没错，由于ecs为了高效，拥有自己的各类container，例如NativeArray，NativeQueue，NatveList, etc. ，这些container是在底层用C++写的，他们不受C#垃圾回收机制管理，需要手动清除。）
        /// </summary>
        public void ShutDown()
        {
            Enabled = false;
            RenderSystem.ShutDown();
            TileSystem.ShutDown();
            WorldSystem.ShutDown();
            MonsterSystem.ShutDown();
            AudioSystem.ShutDown();
            CopyDisplayColorSystem.ShutDown();
            CopyTextureIndexSystem.ShutDown();
            ControlSystem.ControlMode = ControlMode.NoControl;
            
        }
        /// <summary>
        /// 当中央系统被删除时，它需要先中止其他系统运行。
        /// </summary>
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods
        
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

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public static void Move(Vector2 moveVec)
        {
            if (_FreezeCount > 0)
            {
                _FreezeCount--;
                Debug.Log("Freezed! Need " + _FreezeCount + " more try to make another move.");
                return;
            }
            if (!_Moving && (ControlSystem.ControlMode == ControlMode.MapEditor || TileSystem.CenterEntity != Entity.Null))
            {
                Debug.Log(AudioSystem.OnBeats());
                if ((AudioSystem.OnBeats() || ControlSystem.ControlMode == ControlMode.MapEditor) && moveVec != Vector2.zero && moveVec.x * moveVec.y == 0)
                {
                    Direction characterMovingDirection = default;
                    _Moving = true;
                    _MovementTimer = 0;
                    _PreviousOriginPosition = _CurrentCenterPosition;
                    _TargetOriginPosition = _PreviousOriginPosition;

                    #region Decide Direction
                    if (moveVec.x > 0)
                    {
                        if (!(ControlSystem.ControlMode == ControlMode.MapEditor))
                        {

                            if (_InverseDirection)
                            {
                                if (m_EntityManager.GetComponentData<LeftTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                            else
                            {
                                if (m_EntityManager.GetComponentData<RightTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }

                        }
                        characterMovingDirection = _InverseDirection ? Direction.Left : Direction.Right;
                    }
                    else if (moveVec.x < 0)
                    {
                        if (!(ControlSystem.ControlMode == ControlMode.MapEditor))
                        {
                            if (_InverseDirection)
                            {
                                if (m_EntityManager.GetComponentData<RightTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                            else
                            {
                                if (m_EntityManager.GetComponentData<LeftTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                        }
                        characterMovingDirection = _InverseDirection ? Direction.Right : Direction.Left;
                    }
                    else if (moveVec.y > 0)
                    {
                        if (!(ControlSystem.ControlMode == ControlMode.MapEditor))
                        {
                            if (_InverseDirection)
                            {
                                if (m_EntityManager.GetComponentData<DownTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                            else
                            {
                                if (m_EntityManager.GetComponentData<UpTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                        }
                        characterMovingDirection = _InverseDirection ? Direction.Down : Direction.Up;
                    }
                    else if (moveVec.y < 0)
                    {
                        if (!(ControlSystem.ControlMode == ControlMode.MapEditor))
                        {
                            if (_InverseDirection)
                            {
                                if (m_EntityManager.GetComponentData<UpTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                            else
                            {
                                if (m_EntityManager.GetComponentData<DownTile>(TileSystem.CenterEntity).Value == Entity.Null)
                                {
                                    Debug.Log("Blocked in player mode! Use map editor mode if you want to move to empty space.");
                                    return;
                                }
                            }
                        }
                        characterMovingDirection = _InverseDirection ? Direction.Up : Direction.Down;
                    }
                    #endregion

                    switch (characterMovingDirection)
                    {
                        case Direction.Up:
                            _TargetOriginPosition.y -= 1;
                            MainCharacterController.MoveUp();
                            Debug.Log("Move up, target position: " + (-_TargetOriginPosition));
                            break;
                        case Direction.Down:
                            _TargetOriginPosition.y += 1;
                            MainCharacterController.MoveDown();
                            Debug.Log("Move down, target position: " + (-_TargetOriginPosition));
                            break;
                        case Direction.Left:
                            _TargetOriginPosition.x += 1;
                            MainCharacterController.MoveLeft();
                            Debug.Log("Move left, target position: " + (-_TargetOriginPosition));
                            break;
                        case Direction.Right:
                            _TargetOriginPosition.x -= 1;
                            MainCharacterController.MoveRight();
                            Debug.Log("Move right, target position: " + (-_TargetOriginPosition));
                            break;
                    }
                    UISystem.ShowHit_300();
                }
                else
                {
                    UISystem.ShowMiss();
                }
            }
        }
        #endregion

        #region Jobs
        [BurstCompile]
        protected struct TimerJob : IJobForEach<Timer>
        {
            [ReadOnly] public float deltaTime;
            public void Execute(ref Timer c0)
            {
                if (!c0.isOn || c0.T == c0.maxT) return;
                c0.T += deltaTime;
                if (c0.T > c0.maxT) c0.T = c0.maxT;
            }
        }
        #endregion

        protected JobHandle OnFixedUpdate(ref JobHandle inputDeps)
        {
            #region Update Systems
            m_AudioSystem.OnFixedUpdate(ref inputDeps, _BeatCounter);
            m_TileSystem.OnFixedUpdate(ref inputDeps, _Counter);
            m_MonsterSystem.OnFixedUpdate(ref inputDeps, _Counter);
            #endregion

            return inputDeps;
        }

        protected JobHandle OnBeatUpdate(ref JobHandle inputDeps)
        {
            #region Update Systems
            m_AudioSystem.OnBeatUpdate(ref inputDeps, _BeatCounter);
            m_TileSystem.OnBeatUpdate(ref inputDeps, _BeatCounter);
            m_MonsterSystem.OnBeatUpdate(ref inputDeps, _BeatCounter);
            #endregion

            if (TileSystem.CenterEntity != Entity.Null)
            {
                switch (EntityManager.GetComponentData<TileTypeIndex>(TileSystem.CenterEntity).Value)
                {
                    case TileType.NailTrap:
                        if (EntityManager.GetComponentData<TextureIndex>(TileSystem.CenterEntity).Value == 1)
                        {
                            m_MainCharacterController.ChangeHealth(-1);
                            m_AudioSystem.PlayTrapSound();

                        }
                        break;
                }
            }
            return inputDeps;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Running)
            {
                inputDeps = new TimerJob
                {
                    deltaTime = Time.deltaTime
                }.Schedule(this, inputDeps);

                #region Special System Update
                #region Fixed Time Step
                _Timer += Time.deltaTime;
                if (_Timer >= TimeStep)
                {
                    _Counter += (int)(_Timer / _TimeStep);
                    _Timer = 0;
                    OnFixedUpdate(ref inputDeps);
                }
                #endregion

                #region Beat
                int count = AudioSystem.CurrentBeatCounter();
                if (count != _BeatCounter)
                {
                    _BeatCounter = count;
                    OnBeatUpdate(ref inputDeps);
                }
                #endregion
                #endregion

                #region Movement
                if (_Moving) OnMoving();
                else if (_CheckTile && TileSystem.CenterEntity != Entity.Null)
                {
                    //如果上次移动之后没有检测是什么砖块，我们在这里进行操作。
                    _CheckTile = false;
                    switch (EntityManager.GetComponentData<TileTypeIndex>(TileSystem.CenterEntity).Value)
                    {
                        case TileType.Normal:
                            break;
                        case TileType.FreezeTrap:
                            _FreezeCount = 5;
                            break;
                        case TileType.InverseTrap:
                            _InverseDirection = !_InverseDirection;
                            break;
                        case TileType.MusicAccleratorTrap:

                            break;
                        default:
                            break;
                    }
                }
                if (_Zooming) OnZooming();

                #endregion
            }

            return inputDeps;
        }
        #region Movement


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
                // TODO
                MainCharacterController.Idle();
            }
            //设置以在移动完成之后进行操作。
            _CheckTile = true;
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
        #endregion
    }


    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class FloatingOriginSystem : JobComponentSystem
    {
        #region Private
        private float3 _PreviousCenterPosition;
        #endregion

        #region Public
        #endregion

        #region Managers

        #endregion

        #region Methods
        #endregion

        #region Jobs

        [BurstCompile]
        struct CalculateLocalToWorld : IJobForEach<Coordinate, Scale, Translation, Rotation>
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

        #endregion
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps = new CalculateLocalToWorld
            {
                scale = CentralSystem.Scale / CentralSystem.CurrentZoomFactor,
                centerPosition = CentralSystem.CurrentCenterPosition
            }.Schedule(this, inputDeps);
            inputDeps.Complete();
            return inputDeps;
        }
    }
}