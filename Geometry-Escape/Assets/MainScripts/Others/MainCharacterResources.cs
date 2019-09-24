using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class MainCharacterResources : ScriptableObject
    {
        #region Private
        [SerializeField]
        private GameObject[] m_CharacterControllers = null;
        #endregion

        #region Public
        private CharacterController m_MainCharacterController;
        public CharacterController MainCharacterController {
            get
            {
                if (m_MainCharacterController == null) Init();
                return m_MainCharacterController;
            }
        }
        #endregion

        #region Methods
        public void Init()
        {
            m_MainCharacterController = Instantiate(m_CharacterControllers[0], new Vector3(0, 0, -3), Quaternion.identity).GetComponent<CharacterController>();
        }

        public void Init(int controllerIndex)
        {
            Debug.Assert(controllerIndex < m_CharacterControllers.Length);
            m_MainCharacterController = Instantiate(m_CharacterControllers[controllerIndex], new Vector3(0, 0, -3), Quaternion.identity).GetComponent<CharacterController>();
        }
        #endregion
    }
}
