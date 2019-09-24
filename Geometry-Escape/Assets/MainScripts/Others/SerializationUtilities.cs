using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace GeometryEscape
{
    public class SerializationUtilities
    {
        public static void Write(BinaryWriter writer, ref Coordinate coordinate)
        {
            writer.Write(coordinate.X);
            writer.Write(coordinate.Y);
            writer.Write(coordinate.Z);
            writer.Write(coordinate.Direction);
        }

        public static Coordinate ReadCoordinate(BinaryReader reader)
        {
            var coordinate = new Coordinate();
            coordinate.X = reader.ReadSingle();
            coordinate.Y = reader.ReadSingle();
            coordinate.Z = reader.ReadSingle();
            coordinate.Direction = reader.ReadSingle();
            return coordinate;
        }

        public static void Write(BinaryWriter writer, ref TileProperties tileProperties)
        {
            writer.Write(tileProperties.Index);
        }

        public static TileProperties ReadTileProperties(BinaryReader reader)
        {
            var tileProperties = new TileProperties();
            tileProperties.Index = reader.ReadInt32();
            return tileProperties;
        }
    }
}
