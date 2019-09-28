using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class LightResources : ScriptableObject
    {
        #region Private
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

        public void TrapColor()
        {
            m_ViewLight.GetComponent<Light>().color = Color.red;
        }
        public void StopTrapColor()
        {
            m_ViewLight.GetComponent<Light>().color = Color.white;
        }
        public void Init()
        {
            m_ViewLight = Instantiate(m_ViewLightPrefab);
        }
    }
}