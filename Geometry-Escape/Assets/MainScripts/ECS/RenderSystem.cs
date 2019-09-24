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
    [UpdateBefore(typeof(MeshRenderSystem))]
    public class CopyTextureIndexSystem : JobComponentSystem
    {
        #region Private
        private static EntityQuery _EntityQuery;
        private static NativeArray<TextureIndex> _TextureIndices;
        private static ComputeBuffer[] _ComputeBuffers;
        private static List<RenderContent> _RenderContents;
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _RenderContents = new List<RenderContent>();
            _EntityQuery = EntityManager.CreateEntityQuery(typeof(RenderContent), typeof(TextureIndex));
        }

        public void Init()
        {
            ShutDown();
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_ComputeBuffers != null)
            {
                foreach (var i in _ComputeBuffers)
                {
                    if (i != null) i.Release();
                }
            }
            if (_TextureIndices.IsCreated) _TextureIndices.Dispose();
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityManager.GetAllUniqueSharedComponentData(_RenderContents);
            int count = _RenderContents.Count;
            if (_ComputeBuffers == null)
            {
                _ComputeBuffers = new ComputeBuffer[count - 1];
            }else if(_ComputeBuffers.Length < count)
            {
                foreach (var i in _ComputeBuffers)
                {
                    if (i != null) i.Release();
                }
                _ComputeBuffers = new ComputeBuffer[count - 1];
            }
            for (int i = 1; i < count; i++)
            {
                _EntityQuery.SetFilter(_RenderContents[i]);
                _TextureIndices = _EntityQuery.ToComponentDataArray<TextureIndex>(Allocator.TempJob);
                Debug.Log(_TextureIndices.Length);
                if (_TextureIndices.Length != 0)
                {
                    if (_ComputeBuffers[i - 1] != null) _ComputeBuffers[i - 1].Release();
                    _ComputeBuffers[i - 1] = new ComputeBuffer(_TextureIndices.Length, 4);
                    _ComputeBuffers[i - 1].SetData(_TextureIndices);
                    _RenderContents[i].MeshMaterial.Material.SetBuffer("_TilingAndOffsetBuffer", _ComputeBuffers[i - 1]);
                }
                _TextureIndices.Dispose();
            }
            _RenderContents.Clear();
            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(MeshRenderSystem))]
    public class CopyDisplayColorSystem : JobComponentSystem
    {
        #region Private
        private static EntityQuery _EntityQuery;
        private static NativeArray<DefaultColor> _Colors;
        private static ComputeBuffer[] _ComputeBuffers;
        private static List<RenderContent> _RenderContents;
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _RenderContents = new List<RenderContent>();
            _EntityQuery = EntityManager.CreateEntityQuery(typeof(RenderContent), typeof(DefaultColor));
        }

        public void Init()
        {
            ShutDown();
            Enabled = true;
        }

        public void ShutDown()
        {
            Enabled = false;
            if (_ComputeBuffers != null)
            {
                foreach (var i in _ComputeBuffers)
                {
                    if (i != null) i.Release();
                }
            }
            if (_Colors.IsCreated) _Colors.Dispose();
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityManager.GetAllUniqueSharedComponentData(_RenderContents);
            int count = _RenderContents.Count;
            if (_ComputeBuffers == null)
            {
                _ComputeBuffers = new ComputeBuffer[count - 1];
            }
            else if (_ComputeBuffers.Length < count)
            {
                foreach (var i in _ComputeBuffers)
                {
                    if (i != null) i.Release();
                }
                _ComputeBuffers = new ComputeBuffer[count - 1];
            }
            for (int i = 1; i < count; i++)
            {
                _EntityQuery.SetFilter(_RenderContents[i]);
                _Colors = _EntityQuery.ToComponentDataArray<DefaultColor>(Allocator.TempJob);
                Debug.Log(_Colors.Length);
                if (_Colors.Length != 0)
                {
                    if (_ComputeBuffers[i - 1] != null) _ComputeBuffers[i - 1].Release();
                    _ComputeBuffers[i - 1] = new ComputeBuffer(_Colors.Length, 16);
                    _ComputeBuffers[i - 1].SetData(_Colors);
                    _RenderContents[i].MeshMaterial.Material.SetBuffer("_ColorBuffer", _ComputeBuffers[i - 1]);
                }
                _Colors.Dispose();
            }
            _RenderContents.Clear();
            return inputDeps;
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class MeshRenderSystem : JobComponentSystem
    {
        #region Private
        private EntityQuery _MeshRenderEntityQuery;
        private NativeArray<LocalToWorld> _LocalToWorlds;
        private List<RenderContent> _RenderMeshes;
        private ComputeBuffer[] _ArgsBuffers, _LocalToWorldBuffers;
        private uint[] args;
        #endregion

        #region Public
        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
            _ArgsBuffers = new ComputeBuffer[32];
            _LocalToWorldBuffers = new ComputeBuffer[32];
            _RenderMeshes = new List<RenderContent>();
            _MeshRenderEntityQuery = EntityManager.CreateEntityQuery(typeof(RenderContent), typeof(LocalToWorld));
        }
        public void Init()
        {
            ShutDown();
            args = new uint[5] { 0, 0, 0, 0, 0 };
            Enabled = true;
        }

        public void ShutDown()
        {
            if (_LocalToWorlds.IsCreated) _LocalToWorlds.Dispose();
            if (_LocalToWorldBuffers != null)
                foreach (var i in _LocalToWorldBuffers)
                {
                    if (i != null) i.Release();
                }
            if (_ArgsBuffers != null)
                foreach (var i in _ArgsBuffers)
                {
                    if (i != null) i.Release();
                }
            Enabled = false;
        }
        protected override void OnDestroy()
        {
            ShutDown();
        }

        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityManager.GetAllUniqueSharedComponentData(_RenderMeshes);
            int count = _RenderMeshes.Count;
            for (int i = 1; i < count; i++)
            {
                _MeshRenderEntityQuery.SetFilter(_RenderMeshes[i]);
                _LocalToWorlds = _MeshRenderEntityQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
                int amount = _LocalToWorlds.Length;
                if (amount != 0)
                {
                    if (_ArgsBuffers[i - 1] != null) _ArgsBuffers[i - 1].Release();
                    if (_LocalToWorldBuffers[i - 1] != null) _LocalToWorldBuffers[i - 1].Release();
                    args[0] = _RenderMeshes[i].MeshMaterial.Mesh.GetIndexCount(0);
                    args[1] = (uint)amount;
                    _ArgsBuffers[i - 1] = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                    _ArgsBuffers[i - 1].SetData(args);

                    _LocalToWorldBuffers[i - 1] = new ComputeBuffer(amount, 64);
                    _LocalToWorldBuffers[i - 1].SetData(_LocalToWorlds);
                    _RenderMeshes[i].MeshMaterial.Material.SetBuffer("_LocalToWorldBuffer", _LocalToWorldBuffers[i - 1]);

                    Graphics.DrawMeshInstancedIndirect(_RenderMeshes[i].MeshMaterial.Mesh, 0, _RenderMeshes[i].MeshMaterial.Material, new Bounds(Vector3.zero, Vector3.one * 60000), _ArgsBuffers[i - 1], 0, null, 0, false, 0);
                }
                _LocalToWorlds.Dispose();
            }
            _RenderMeshes.Clear();
            return inputDeps;
        }
    }

}
