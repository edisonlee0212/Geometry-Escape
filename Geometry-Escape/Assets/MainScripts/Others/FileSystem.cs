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
            Debug.Log("Saving Path: " + _SavePath);
            m_SerializationUtilities = new SerializationUtilities();
            _EntityManager = World.Active.EntityManager;
            _TileEntityQuery = _EntityManager.CreateEntityQuery(typeof(TileProperties));
        }

        public static void SaveMapByPath(string path)
        {
            //If the central system is currently running, we return because other systems may alter the map.
            if (CentralSystem.Running) CentralSystem.Pause();
            var stream = File.Open(path, FileMode.OpenOrCreate);
            SaveMap(stream);
            CentralSystem.WorldSystem.Resume();
        }

        public static void SaveMapByName(string mapName)
        {
            //If the central system is currently running, we return because other systems may alter the map.
            if (CentralSystem.Running) CentralSystem.Pause();
            var stream = File.Open(_SavePath + "/" + mapName, FileMode.OpenOrCreate);
            SaveMap(stream);
        }

        private static void SaveMap(FileStream stream)
        {
            //First we use the query to get the list of the tile entities.
            var tileEntityArray = _TileEntityQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            //We calculate the tile count.
            int tileAmount = tileEntityArray.Length;
            Debug.Log("Saving " + tileAmount + " tiles.");
            //Load file stream for saving.
            var formatter = new BinaryWriter(stream);
            //Below are file writing process.

            //1. We write the amount of tiles.
            formatter.Write(tileAmount);
            //Write the necessary information of a tile.
            foreach (var i in tileEntityArray)
            {
                SaveTileEntity(stream, i, formatter);
            }
            //Discard the entity array.
            tileEntityArray.Dispose();


            //Now we store the monsters.
            //TODO

        }

        public static void LoadMapByPath(string path)
        {
            if (CentralSystem.Running) CentralSystem.Pause();
            //Open the file using the map name
            var stream = File.Open(path, FileMode.Open);
            LoadMap(stream);
            CentralSystem.WorldSystem.Resume();
        }

        public static void LoadMapByName(string mapName)
        {
            if (CentralSystem.Running) CentralSystem.Pause();
            //Open the file using the map name
            var stream = File.Open(_SavePath + "/" + mapName, FileMode.Open);
            LoadMap(stream);
            CentralSystem.WorldSystem.Resume();
        }

        private static void LoadMap(FileStream stream)
        {
            var formatter = new BinaryReader(stream);
            //Get the amount of tiles.
            int tileAmount = formatter.ReadInt32();
            Debug.Log("Loading " + tileAmount + " tiles.");
            //Read tile and load it into the game world.
            for (int i = 0; i < tileAmount; i++)
            {
                LoadTileEntity(stream, formatter);
            }

            //Read monsters.
            //TODO

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