using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class TileMeshAndMaterials : ScriptableObject
    {
        #region Public
        public UnityEngine.Mesh TileMesh;
        public UnityEngine.Material[] Materials;
        #endregion

        public int GetMaterialAmount()
        {
            return Materials.Length;
        }
    }
}
