using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GeometryEscape
{
    public enum TileType
    {
        Normal, 
        Trap,
        Bonus,
    }

    [Serializable]
    public struct RenderMaterialIndex : ISharedComponentData, IEquatable<RenderMaterialIndex>
    {
        public int Value;

        public bool Equals(RenderMaterialIndex other)
        {
            return other.Value == Value;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }



    [Serializable]
    public struct TextureMaxIndex : IComponentData
    {
        public ushort Value;
    }

    [Serializable]
    public struct TextureIndex : IComponentData
    {
        public int Value;
    }

    [Serializable]
    public struct Coordinate : IComponentData
    {
        public int X, Y, Z, Direction;
    }

    [Serializable]
    public struct CustomLocalToWorld : IComponentData
    {
        public float4x4 Value;
        public float3 Position
        {
            get
            {
                return new float3(Value.c3.x, Value.c3.y, Value.c3.z);
            }
            set
            {
                Value.c3.x = value.x;
                Value.c3.y = value.y;
                Value.c3.z = value.z;
            }
        }
        public float3 Scale
        {
            get
            {
                return new float3(Value.c0.x, Value.c1.y, Value.c2.z);
            }
            set
            {
                Value.c0.x = value.x;
                Value.c1.y = value.y;
                Value.c2.z = value.z;
            }
        }
    }

    [Serializable]
    public struct TileProperties : IComponentData
    {
        public int Index;
        public int MaterialIndex;
        public TileType TileType;
    }

    [Serializable]
    public struct DefaultColor : IComponentData
    {
        public Vector4 Color;
    }
}
