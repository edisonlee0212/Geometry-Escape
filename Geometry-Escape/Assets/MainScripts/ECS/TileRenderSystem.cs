using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
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
    public class TileRenderSystem : JobComponentSystem
    {
        #region Private
        /// <summary>
        /// The query to select array of tiles for rendering.
        /// </summary>
        private EntityQuery _TileQuery;
        /// <summary>
        /// Array to hold the result for query.
        /// </summary>
        private NativeArray<LocalToWorld> _TilesLocalToWorlds;
        /// <summary>
        /// Array to hold the texture info (tiling and offset).
        /// </summary>
        private NativeArray<TextureIndex> _TilesTextureInfos;
        /// <summary>
        /// Array to hold the result for query.
        /// </summary>
        private NativeArray<DefaultColor> _TilesColors;
        /// <summary>
        /// The buffer send to GPU for rendering.
        /// </summary>
        private ComputeBuffer[] _LocalToWorldBuffers, _ColorBuffers, _TextureInfoBuffers;
        private ComputeBuffer[] _ArgsBuffers;
        private uint[] args;
        #endregion

        #region Public
        private int _MaxSingleMaterialTileAmount;
        private UnityEngine.Material[] m_Materials;
        private static int _MaterialAmount;
        private Camera m_Camera;
        private UnityEngine.Mesh m_TileMesh;
        /// <summary>
        /// The list of material for drawing a tile.
        /// </summary>
        public UnityEngine.Material[] Materials { get => m_Materials; set => m_Materials = value; }
        /// <summary>
        /// The mesh for a tile, which is just a quad.
        /// </summary>
        public UnityEngine.Mesh TileMesh { get => m_TileMesh; set => m_TileMesh = value; }
        /// <summary>
        /// The amount of different material types.
        /// </summary>
        public static int MaterialAmount { get => _MaterialAmount; set => _MaterialAmount = value; }
        /// <summary>
        /// For each type of material, the maximum tile amount the system supports.
        /// There's no upper limit for this but a greater value will produce more overhead.
        /// </summary>
        public int MaxSingleMaterialTileAmount { get => _MaxSingleMaterialTileAmount; set => _MaxSingleMaterialTileAmount = value; }
        /// <summary>
        /// The target camera for rendering.
        /// </summary>
        public Camera Camera { get => m_Camera; set => m_Camera = value; }

        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _TileQuery = GetEntityQuery(typeof(TextureIndex), typeof(TileProperties), typeof(LocalToWorld), typeof(DefaultColor), typeof(RenderMaterialIndex));
        }

        public void Init()
        {
            ShutDown();
            _LocalToWorldBuffers = new ComputeBuffer[_MaterialAmount];
            _TextureInfoBuffers = new ComputeBuffer[_MaterialAmount];
            _ColorBuffers = new ComputeBuffer[_MaterialAmount];

            args = new uint[5] { m_TileMesh.GetIndexCount(0), 0, 0, 0, 0 };
            _ArgsBuffers = new ComputeBuffer[_MaterialAmount];
            //_ArgsBuffers = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            //_ArgsBuffers.SetData(args);
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_TilesLocalToWorlds.IsCreated) _TilesLocalToWorlds.Dispose();
            if (_TilesColors.IsCreated) _TilesColors.Dispose();
            if (_TilesTextureInfos.IsCreated) _TilesTextureInfos.Dispose();
            if (_LocalToWorldBuffers != null) foreach (var i in _LocalToWorldBuffers)
                {
                    if (i != null) i.Release();
                }
            if (_ColorBuffers != null) foreach (var i in _ColorBuffers)
                {
                    if (i != null) i.Release();
                }
            if (_TextureInfoBuffers != null) foreach (var i in _TextureInfoBuffers)
                {
                    if (i != null) i.Release();
                }
            if (_ArgsBuffers != null) foreach (var i in _ArgsBuffers)
                {
                    if (i != null) i.Release();
                }
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods

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

        public static unsafe void ToArray(NativeArray<TextureIndex> colors, int count, int[] outMatrices, int offset)
        {
            fixed (int* resultMatrices = outMatrices)
            {
                TextureIndex* sourceMatrices = (TextureIndex*)colors.GetUnsafeReadOnlyPtr();
                UnsafeUtility.MemCpy(resultMatrices + offset, sourceMatrices, UnsafeUtility.SizeOf<int>() * count);
            }
        }

        private static unsafe void ToArray(NativeArray<LocalToWorld> transforms, int count, float4x4[] outMatrices, int offset)
        {
            fixed (float4x4* resultMatrices = outMatrices)
            {
                LocalToWorld* sourceMatrices = (LocalToWorld*)transforms.GetUnsafeReadOnlyPtr();
                UnsafeUtility.MemCpy(resultMatrices + offset, sourceMatrices, UnsafeUtility.SizeOf<float4x4>() * count);
            }
        }
        #endregion

        #endregion

        #region Jobs
        #endregion


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            for (int i = 0; i < _MaterialAmount; i++)
            {
                //Set up query filter by material index. We render tiles in same material in batch.
                _TileQuery.SetFilter(new RenderMaterialIndex { Value = i });

                //Query matrix and colors.
                _TilesLocalToWorlds = _TileQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
                _TilesColors = _TileQuery.ToComponentDataArray<DefaultColor>(Allocator.TempJob);
                _TilesTextureInfos = _TileQuery.ToComponentDataArray<TextureIndex>(Allocator.TempJob);
                int amount = _TilesLocalToWorlds.Length;
                if (amount != 0)
                {
                    //Reallocate ComputeBuffer.
                    if (_LocalToWorldBuffers[i] != null) _LocalToWorldBuffers[i].Release();
                    if (_ColorBuffers[i] != null) _ColorBuffers[i].Release();
                    if (_TextureInfoBuffers[i] != null) _TextureInfoBuffers[i].Release();
                    if (_ArgsBuffers[i] != null) _ArgsBuffers[i].Release();
                    _LocalToWorldBuffers[i] = new ComputeBuffer(amount, 64);
                    _ColorBuffers[i] = new ComputeBuffer(amount, 16);
                    _TextureInfoBuffers[i] = new ComputeBuffer(amount, 4);
                    _ArgsBuffers[i] = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

                    //Set up data for ComputeBuffer.
                    _LocalToWorldBuffers[i].SetData(_TilesLocalToWorlds);
                    _ColorBuffers[i].SetData(_TilesColors);
                    _TextureInfoBuffers[i].SetData(_TilesTextureInfos);

                    //Set up material.
                    m_Materials[i].SetBuffer("_LocalToWorldBuffer", _LocalToWorldBuffers[i]);
                    m_Materials[i].SetBuffer("_ColorBuffer", _ColorBuffers[i]);
                    m_Materials[i].SetBuffer("_TilingAndOffsetBuffer", _TextureInfoBuffers[i]);

                    //Set up args buffer, so the GPU knows how many meshes(tiles) we want to render in this draw call.
                    args[1] = (uint)amount;
                    _ArgsBuffers[i].SetData(args);

                    //Draw tiles.
                    Graphics.DrawMeshInstancedIndirect(m_TileMesh, 0, m_Materials[i], new Bounds(Vector3.zero, Vector3.one * 60000), _ArgsBuffers[i], 0, null, 0, false, 0);
                }
                //Dispose resources.
                _TilesLocalToWorlds.Dispose();
                _TilesColors.Dispose();
                _TilesTextureInfos.Dispose();
            }

            return inputDeps;
        }
    }
}
