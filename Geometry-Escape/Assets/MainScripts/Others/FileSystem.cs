using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
namespace GeometryEscape
{
    public class FileSystem
    {
        public static string _SavePath;
        public static EntityManager _EntityManager;
        public static EntityQuery _TileEntityQuery;
        private static SerializationUtilities m_SerializationUtilities;
        public FileSystem()
        {
            _SavePath = Application.persistentDataPath;
            m_SerializationUtilities = new SerializationUtilities();
            _EntityManager = World.Active.EntityManager;
            _TileEntityQuery = _EntityManager.CreateEntityQuery(typeof(TileProperties));
        }

        public static void SaveNewMap(string mapName)
        {
            //If the central system is currently running, we return because other systems may alter the map.
            if (CentralSystem.Running) CentralSystem.Pause();
            var tileEntityArray = _TileEntityQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            int tileAmount = tileEntityArray.Length;
            Debug.Log("Saving " + tileAmount + " tiles.");
            var stream = File.Open(_SavePath + "/" + mapName, FileMode.OpenOrCreate);
            var formatter = new BinaryWriter(stream);
            formatter.Write(tileAmount);
            foreach (var i in tileEntityArray)
            {
                SaveTileEntity(stream, i, formatter);
            }
            tileEntityArray.Dispose();
            CentralSystem.Resume();
        }

        public static void LoadMap(string mapName)
        {
            if (CentralSystem.Running) CentralSystem.Pause();
            var stream = File.Open(_SavePath + "/" + mapName, FileMode.Open);
            var formatter = new BinaryReader(stream);
            int tileAmount = formatter.ReadInt32();
            Debug.Log("Loading " + tileAmount + " tiles.");
            for (int i = 0; i < tileAmount; i++)
            {
                LoadTileEntity(stream, formatter);
            }
            CentralSystem.Resume();
        }

        public static void SaveTileEntity(FileStream stream, Entity entity, BinaryWriter writer)
        {
            var coordinate = _EntityManager.GetComponentData<Coordinate>(entity);
            SerializationUtilities.Write(writer, ref coordinate);
            var tileProperties = _EntityManager.GetComponentData<TileProperties>(entity);
            SerializationUtilities.Write(writer, ref tileProperties);
        }

        public static void LoadTileEntity(FileStream stream, BinaryReader reader)
        {

            var tileLoadingInfo = new TileCreationInfo
            {
                Coordinate = SerializationUtilities.ReadCoordinate(reader),
                TileProperties = SerializationUtilities.ReadTileProperties(reader)
            };
            WorldSystem.AddTileCreationInfo(tileLoadingInfo);
        }
    }
}