using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
namespace GeometryEscape
{
    /// <summary>
    /// The system that control all other systems.
    /// </summary>
    public class CentralSystem : JobComponentSystem
    {
        #region Public
        private static RenderSystem m_RenderSystem;
        private static TileSystem m_TileSystem;
        private static WorldSystem m_WorldSystem;
        private static ControlSystem m_ControlSystem;
        private static AudioSystem m_AudioSystem;
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
        private static MusicResources m_MusicResources;

        /*
         * 下面是各种系统的引用，虽然系统内大部分成员变量都是static全局的变量，但是系统本身不是static的，因为unity允许多个相同系统的存在。所以在这里建立和各个子系统的链接。
         */
        public static RenderSystem RenderSystem { get => m_RenderSystem; set => m_RenderSystem = value; }
        public static TileSystem TileSystem { get => m_TileSystem; set => m_TileSystem = value; }
        public static WorldSystem WorldSystem { get => m_WorldSystem; set => m_WorldSystem = value; }
        public static ControlSystem ControlSystem { get => m_ControlSystem; set => m_ControlSystem = value; }
        public static LightResources LightResources { get => m_LightResources; set => m_LightResources = value; }
        public static TileResources TileResources { get => m_TileResources; set => m_TileResources = value; }
        public static MusicResources MusicResources { get => m_MusicResources; set => m_MusicResources = value; }
        public static AudioSystem AudioSystem { get => m_AudioSystem; set => m_AudioSystem = value; }

        #endregion
        /// <summary>
        /// 每个继承JobComponentSystem的系统都拥有OnCreate，OnDestroy方法，这两个方法在系统建立时和系统被删除时会被调用。ecs的所有系统默认随程序启动，所以在程序运行时所有系统都会被调用OnCreate方法。
        /// 在程序结束的时候所有系统也会被调用ondestroy方法。
        /// </summary>
        protected override void OnCreate()
        {
            //如果你观察所有其他系统的OnCreate函数，
            //你会发现它们都只包含Enabled = false，即所有子系统虽然在程序开始时都被自动建立，但是所有子系统在开始时都会中止运行。
            //直到centralsystem结束所有必要设置后由centralsystem开启各个子系统，这些初始设置及启动子系统的步骤包含在Init内。
            Init();
        }

        public void Init()
        {
            //首先我们载入resources，准备好要分配给各个子系统的资源
            m_LightResources = Resources.Load<LightResources>("ScriptableObjects/LightResources");
            m_TileResources = Resources.Load<TileResources>("ScriptableObjects/TileResources");
            m_MusicResources = Resources.Load<MusicResources>("ScriptableObjects/MusicResources");
            //设置砖块系统的一些初始参数，tilescale值砖块的大小，你可以尝试改变这个值，看看有什么效果。
            TileSystem.TileScale = 2;
            //设置砖块系统的单位时间，砖块系统内有对应的OnUpdate和OnFixedUpdate，对应unity原本的Update和FixedUpdate
            //其中OnUpdate由系统自动调用，OnFixedUpdate为我写的方法，由OnUpdate调用，这里就是调用时间间隔，0.1f代表每秒执行10次OnFixedUpdate
            TileSystem.TimeStep = 0.1f;

            //确认所有必须设置的初始参数设置完毕，我们可以启动各个子系统。
            RenderSystem = World.Active.GetOrCreateSystem<RenderSystem>();//从unity获取当前已经存在的系统
            RenderSystem.Init();//启动这个系统
            TileSystem = World.Active.GetOrCreateSystem<TileSystem>();
            TileSystem.Init();
            WorldSystem = World.Active.GetOrCreateSystem<WorldSystem>();
            WorldSystem.Init();
            AudioSystem = World.Active.GetOrCreateSystem<AudioSystem>();
            AudioSystem.Init();

            ControlSystem = new ControlSystem();//control system并不是一个真正的ECS的系统，所以我们通过这种方式建立。
            ControlSystem.ControlMode = ControlMode.InGame;

            //所有系统建立完毕之后，我们调用worldsystem来构建游戏世界，当前只有砖块，所以我们生成100块砖。
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    int index = i * count + j;
                    WorldSystem.AddTile(index % 4, new Coordinate { X = i, Y = j, Z = 0 });
                }
            }
        }
        /// <summary>
        /// 所有系统包含shutdown函数，这个函数包括Init是我的习惯，这两个函数对应”不希望删除整个系统只是中止运行或者恢复运行“这种需求。
        /// 对于中央系统，在关闭中央系统时中央系统会先关闭所有其他子系统，保证子系统正确关闭，没有内存泄露。
        /// （没错，由于ecs为了高效，拥有自己的各类container，例如NativeArray，NativeQueue，NatveList, etc. ，这些container是在底层用C++写的，他们不受C#垃圾回收机制管理，需要手动清除。）
        /// </summary>
        public void ShutDown()
        {
            m_RenderSystem.ShutDown();
            m_TileSystem.ShutDown();
            m_WorldSystem.ShutDown();
        }
        /// <summary>
        /// 当中央系统被删除时，它需要先中止其他系统运行。
        /// </summary>
        protected override void OnDestroy()
        {
            ShutDown();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}