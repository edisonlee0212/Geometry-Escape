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
        Blocked,
        Normal,
        NailTrap,
        MusicAccleratorTrap,
        FreezeTrap,
        InverseTrap,
        Bonus,
        Exit,
        Recover,
    }

    public enum EntityType
    {
        Tile,
        Monster
    }

    [Serializable]
    public struct TypeOfEntity : IComponentData
    {
        public EntityType Value;
    }

    [Serializable]
    public struct RenderContent : ISharedComponentData, IEquatable<RenderContent>
    {
        public MeshMaterial MeshMaterial;
        public bool Equals(RenderContent other)
        {
            return MeshMaterial == other.MeshMaterial;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    [Serializable]
    public class MeshMaterial
    {
        public Mesh Mesh;
        public Material Material;
    }

    public enum MonsterType
    {
        Green,
        Blue,
        Skeleton
    }

    [Serializable]
    public struct TypeOfMonster : IComponentData
    {
        public MonsterType Value;
    }


    [Serializable]
    public struct LeftTile : IComponentData
    {
        public Entity Value;
    }

    [Serializable]
    public struct RightTile : IComponentData
    {
        public Entity Value;
    }

    [Serializable]
    public struct MonsterHP : IComponentData
    {
        public int Value;
    }

    [Serializable]
    public struct UpTile : IComponentData
    {
        public Entity Value;
    }
    [Serializable]
    public struct DownTile : IComponentData
    {
        public Entity Value;
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
    public struct Coordinate : IComponentData, IEquatable<Coordinate>
    {
        public float X, Y, Z, Direction;

        public bool Equals(Coordinate other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return (int)(X * 100 + Y);
        }
    }

    [Serializable]
    public struct PreviousCoordinate : IComponentData
    {
        public float X, Y, Z, Direction;
    }
    [Serializable]
    public struct TargetCoordinate : IComponentData
    {
        public float X, Y, Z, Direction;
    }
    [Serializable]
    public struct Timer : IComponentData
    {
        public float T, maxT;
        public bool isOn;
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
    public struct TypeOfTile : IComponentData
    {
        public TileType Value;
    }

    [Serializable]
    public struct TileProperties : IComponentData
    {
        public int Index;
    }

    [Serializable]
    public struct MonsterProperties : IComponentData
    {
        public int Index;
    }

    [Serializable]
    public struct DisplayColor : IComponentData
    {
        public Vector4 Value;
    }

    [Serializable]
    public struct DefaultColor : IComponentData
    {
        public Vector4 Value;
    }
}
