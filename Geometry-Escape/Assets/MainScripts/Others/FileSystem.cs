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

        public static void SaveNewMap(string mapName)
        {
            //If the central system is currently running, we return because other systems may alter the map.
            if (CentralSystem.Running) CentralSystem.Pause();
            //First we use the query to get the list of the tile entities.
            var tileEntityArray = _TileEntityQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            //We calculate the tile count.
            int tileAmount = tileEntityArray.Length;
            Debug.Log("Saving " + tileAmount + " tiles.");
            //Load file stream for saving.
            var stream = File.Open(_SavePath + "/" + mapName, FileMode.OpenOrCreate);
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

            CentralSystem.Resume();
        }

        public static void LoadMap(string mapName)
        {
            if (CentralSystem.Running) CentralSystem.Pause();
            //Open the file using the map name
            var stream = File.Open(_SavePath + "/" + mapName, FileMode.Open);
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