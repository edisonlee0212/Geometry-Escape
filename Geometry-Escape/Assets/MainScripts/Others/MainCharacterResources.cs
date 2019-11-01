using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class MainCharacterResources : ScriptableObject
    {
        [Serializable]
        public class MainCharacter
        {
            public GameObject Character;
            public Sprite Icon;
        }

        #region Private
        [SerializeField]
        private MainCharacter[] m_MainCharacters;
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
            if (m_MainCharacterController != null) Destroy(m_MainCharacterController);
            m_MainCharacterController = Instantiate(m_MainCharacters[0].Character, new Vector3(0, 0, -3), Quaternion.identity).GetComponent<CharacterController>();
        }

        public CharacterController Init(int controllerIndex)
        {
            Debug.Assert(controllerIndex < m_MainCharacters.Length);
            if (m_MainCharacterController != null) Destroy(m_MainCharacterController);
            m_MainCharacterController = Instantiate(m_MainCharacters[controllerIndex].Character, new Vector3(0, 0, -3), Quaternion.identity).GetComponent<CharacterController>();
            return m_MainCharacterController;
        }



        #endregion
    }
}
