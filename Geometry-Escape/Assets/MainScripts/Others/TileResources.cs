using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class TileResources : ScriptableObject
    {
        #region Public
        [SerializeField]
        private Tile[] _Tiles;
        public int GetTileAmount()
        {
            return _Tiles.Length;
        }

        public Tile GetTile(int index)
        {
            return _Tiles[index];
        }

        #endregion
        [Serializable]
        public struct Tile
        {
            public TileType TileType;
            public RenderContent RenderContent;
            public ushort MaxIndex;
        }
    }
}
