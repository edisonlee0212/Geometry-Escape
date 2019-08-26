using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace GeometryEscape
{
    /* 关于Unity本身系统执行顺序：
     * https://docs.unity3d.com/Manual/ExecutionOrder.html
     * 关于Unity ECS各项系统更新顺序，以及设定方式。
     * https://docs.unity3d.com/Packages/com.unity.entities@0.0/manual/system_update_order.html
     * ECS系统内OnUpdate在Monobehaviour之前执行。
     */
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class TileRenderSystem : ComponentSystem
    {
        #region Private
        private EndSimulationEntityCommandBufferSystem m_CommandBufferSystem;
        private EntityQuery _TileQuery;
        private NativeArray<CustomLocalToWorld> _TilesLocalToWorlds;
        private NativeArray<DefaultColor> _TileColors;
        private float4x4[] _Matrices;
        private float4[] _Colors;
        private ComputeBuffer _LocalToWorldBuffer, _ColorBuffer, _ArgsBuffer;
        private Camera m_Camera;
        private uint[] args;
        private float _Timer;
        #endregion

        #region Public
        private int _TileAmount;
        private UnityEngine.Material[] m_Materials;
        public UnityEngine.Material[] Materials { get => m_Materials; set => m_Materials = value; }
        private UnityEngine.Mesh m_TileMesh;
        public UnityEngine.Mesh TileMesh { get => m_TileMesh; set => m_TileMesh = value; }
        public int TileAmount { get => _TileAmount; set => _TileAmount = value; }
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
        }

        public void Init()
        {
            ShutDown();
            m_Camera = Camera.main;
            _TilesLocalToWorlds = new NativeArray<CustomLocalToWorld>(TileAmount, Allocator.Persistent);
            _TileColors = new NativeArray<DefaultColor>(TileAmount, Allocator.Persistent);
            _Matrices = new float4x4[TileAmount];
            _Colors = new float4[TileAmount];
            _LocalToWorldBuffer = new ComputeBuffer(TileAmount, 64);
            _ColorBuffer = new ComputeBuffer(TileAmount, 16);
            args = new uint[5] { m_TileMesh.GetIndexCount(0), (uint)_TileAmount, 0, 0, 0 };
            _ArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _ArgsBuffer.SetData(args);
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_TilesLocalToWorlds.IsCreated) _TilesLocalToWorlds.Dispose();
            if (_TileColors.IsCreated) _TileColors.Dispose();
            if (_LocalToWorldBuffer != null) _LocalToWorldBuffer.Release();
            if (_ColorBuffer != null) _ColorBuffer.Release();
            if (_ArgsBuffer != null) _ArgsBuffer.Release();
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods

        public void Test()
        {
            _TileAmount = 100;
            Init();
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int index = i * 10 + j;
                    var localToWorld = _TilesLocalToWorlds[index];
                    localToWorld.Value.c3.w = 1;
                    localToWorld.Position = new float3(i, j, 0);
                    localToWorld.Scale = new float3(1, 1, 1);
                    _TilesLocalToWorlds[i * 10 + j] = localToWorld;
                }
            }
            Shuffle();
        }
        private uint state = 1;
        public void Shuffle()
        {
            Unity.Mathematics.Random random = new Unity.Mathematics.Random();
            random.state = state++;
            for(int i = 0; i < _TileAmount; i++)
            {
                var color = _TileColors[i];
                color.Color = new Vector4(random.NextFloat(), random.NextFloat(), random.NextFloat(), 1);
                _TileColors[i] = color;
            }
        }

        #region ToArray()
        /// <summary>
        /// The low-level method to convert NativeArray<DefaultColor> to float4[].
        /// </summary>
        /// <param name="colors">
        /// Array of colors
        /// </param>
        /// <param name="count">
        /// The amount of element to copy.
        /// </param>
        /// <param name="outMatrices">
        /// Destination array.
        /// </param>
        /// <param name="offset">
        /// The start index for copy.
        /// </param>
        public static unsafe void ToArray(NativeArray<DefaultColor> colors, int count, float4[] outMatrices, int offset)
        {
            fixed (float4* resultMatrices = outMatrices)
            {
                DefaultColor* sourceMatrices = (DefaultColor*)colors.GetUnsafeReadOnlyPtr();
                UnsafeUtility.MemCpy(resultMatrices + offset, sourceMatrices, UnsafeUtility.SizeOf<float4>() * count);
            }
        }

        private static unsafe void ToArray(NativeArray<CustomLocalToWorld> transforms, int count, float4x4[] outMatrices, int offset)
        {
            fixed (float4x4* resultMatrices = outMatrices)
            {
                CustomLocalToWorld* sourceMatrices = (CustomLocalToWorld*)transforms.GetUnsafeReadOnlyPtr();
                UnsafeUtility.MemCpy(resultMatrices + offset, sourceMatrices, UnsafeUtility.SizeOf<float4x4>() * count);
            }
        }
        #endregion
        
        #endregion

        #region Jobs
        #endregion


        protected override void OnUpdate()
        {
            _Timer += Time.deltaTime;
            if(_Timer >= 0.2)
            {
                _Timer = 0;
                Shuffle();
            }
            ToArray(_TilesLocalToWorlds, _TileAmount, _Matrices, 0);
            ToArray(_TileColors, _TileAmount, _Colors, 0);
            _LocalToWorldBuffer.SetData(_Matrices);
            _ColorBuffer.SetData(_Colors);
            m_Materials[0].SetBuffer("localToWorldBuffer", _LocalToWorldBuffer);
            m_Materials[0].SetBuffer("colorBuffer", _ColorBuffer);
            Graphics.DrawMeshInstancedIndirect(m_TileMesh, 0, m_Materials[0], new Bounds(Vector3.zero, Vector3.one * 60000), _ArgsBuffer, 0, null, 0, false, 0);
        }
    }
}
