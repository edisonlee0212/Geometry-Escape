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
        private UnityEngine.Mesh _TileMesh;
        [SerializeField]
        private UnityEngine.Material[] _Materials;
        public Mesh TileMesh { get => _TileMesh; set => _TileMesh = value; }
        public Material[] Materials { get => _Materials; set => _Materials = value; }
        public int GetMaterialAmount()
        {
            return Materials.Length;
        }
        #endregion
    }
}
