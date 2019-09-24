using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class MonsterResources : ScriptableObject
    {
        #region Public
        [SerializeField]
        private UnityEngine.Mesh _MonsterMesh;
        [SerializeField]
        private UnityEngine.Material[] _Materials;
        public Mesh MonsterMesh { get => _MonsterMesh; set => _MonsterMesh = value; }
        public Material[] Materials { get => _Materials; set => _Materials = value; }
        public int GetMaterialAmount()
        {
            Debug.Log("material length"+ Materials.Length);
            return Materials.Length;
        }
        #endregion
    }
}
