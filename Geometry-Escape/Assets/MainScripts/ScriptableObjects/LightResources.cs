using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class LightResources : ScriptableObject
    {
        #region Public
        [SerializeField]
        private GameObject m_ViewLightPrefab = null;
        #endregion

        private GameObject m_ViewLight;

        public GameObject ViewLight { get
            {
                if (m_ViewLight == null) Init();
                return m_ViewLight;
            }
        }

        public void Init()
        {
            m_ViewLight = Instantiate(m_ViewLightPrefab, new Vector3(0, 0, -5), Quaternion.identity);
        }
    }
}