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
        public static EntityQuery _MonsterEntityQuery;
        private static SerializationUtilities m_SerializationUtilities;
        public FileSystem()
        {
            _SavePath = Application.persistentDataPath;
            Debug.Log("Saving Path: " + _SavePath);
            m_SerializationUtilities = new SerializationUtilities();
            _EntityManager = World.Active.EntityManager;
            _TileEntityQuery = _EntityManager.CreateEntityQuery(typeof(TileProperties));
            _MonsterEntityQuery = _EntityManager.CreateEntityQuery(typeof(MonsterProperties));
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
            CentralSystem.WorldSystem.Resume();
        }

        private static void SaveMap(FileStream stream)
        {
            //First we use the query to get the list of the tile entities.
            var tileEntityArray = _TileEntityQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            //We calculate the tile count.
            int tileAmount = tileEntityArray.Length;
            Debug.Log("Saving " + tileAmount + " tiles.");
            //Load file stream for saving.
            var writter = new BinaryWriter(stream);
            //Below are file writing process.

            //1. We write the amount of tiles.
            writter.Write(tileAmount);
            //Write the necessary information of a tile.
            foreach (var i in tileEntityArray)
            {
                SaveTileEntity(stream, i, writter);
            }
            //Discard the entity array.
            tileEntityArray.Dispose();


            //Now we store the monsters.
            //TODO
            var monsterEntityArray = _MonsterEntityQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            int monsterAmount = monsterEntityArray.Length;
            writter.Write(monsterAmount);
            Debug.Log("monsterAmount " + monsterAmount);

            foreach (var i in monsterEntityArray)
            {
                SaveMonsterEntity(stream, i, writter);
            }
            monsterEntityArray.Dispose();
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
            if (WorldSystem.MapLoaded)
            {
                Debug.Log("Map is not loaded! Try load map first.");
                return;
            }
            var reader = new BinaryReader(stream);
            //Get the amount of tiles.
            int tileAmount = reader.ReadInt32();
            Debug.Log("Loading " + tileAmount + " tiles.");
            //Read tile and load it into the game world.
            for (int i = 0; i < tileAmount; i++)
            {
                LoadTileEntity(stream, reader);
            }

            //Read monsters.
            //TODO
            int monsterAmount = reader.ReadInt32();
            
            Debug.Log("monsterAmount "+monsterAmount);
            for (int i = 0; i < monsterAmount; i++)
            {
                LoadMonsterEntity(stream, reader);
            }
            WorldSystem.MapLoaded = true;
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
        public static void SaveMonsterEntity(FileStream stream, Entity entity, BinaryWriter writer)
        {
            var coordinate = _EntityManager.GetComponentData<Coordinate>(entity);
            SerializationUtilities.Write(writer, ref coordinate);
            var monsterProperties = _EntityManager.GetComponentData<MonsterProperties>(entity);
            SerializationUtilities.Write(writer, ref monsterProperties);
        }
        public static void LoadMonsterEntity(FileStream stream, BinaryReader reader)
        {

            var monsterLoadingInfo = new MonsterCreationInfo
            {
                Coordinate = SerializationUtilities.ReadCoordinate(reader),
                MonsterProperties = SerializationUtilities.ReadMonsterProperties(reader)
                };
            WorldSystem.AddMonster(monsterLoadingInfo);
        }
    }
}