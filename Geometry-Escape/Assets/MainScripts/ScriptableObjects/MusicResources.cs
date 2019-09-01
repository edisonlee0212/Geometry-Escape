using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GeometryEscape
{
    [CreateAssetMenu]
    public class MusicResources : ScriptableObject
    {
        #region Public
        [SerializeField]
        private Music[] m_Musics;
        [SerializeField]
        private AudioSource m_AudioSourcePrefab;
        public AudioSource AudioSourcePrefab { get => m_AudioSourcePrefab; set => m_AudioSourcePrefab = value; }
        public Music[] Musics { get => m_Musics; set => m_Musics = value; }
        #endregion
        
        [Serializable]
        public class Music
        {
            public AudioClip MusicClip;
            public MusicInfo MusicInfo;
        }

        #region Methods
        public AudioSource InstanciateAudioSource(AudioClip audioClip)
        {
            var source = Instantiate(m_AudioSourcePrefab);
            source.clip = audioClip;
            return source;
        }
        
        #endregion


    }
}
