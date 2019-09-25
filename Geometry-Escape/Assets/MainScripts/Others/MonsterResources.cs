using System;
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
        private Monster[] _Monsters;
        public int GetMonsterAmount()
        {
            return _Monsters.Length;
        }

        public Monster GetMonster(int index)
        {
            return _Monsters[index];
        }

        #endregion
        [Serializable]
        public struct Monster
        {
            public int MonsterIndex;
            public MonsterType MonsterType;
            public RenderContent RenderContent;
            public TextureMaxIndex TextureMaxIndex;
        }
    }

}
