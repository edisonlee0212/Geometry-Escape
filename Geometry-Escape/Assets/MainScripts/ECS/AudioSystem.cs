using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
namespace GeometryEscape
{
    public struct MusicRecordingInfo
    {
        public float StartTime;
        public int BeatCounter;
        public float CurrentBeatTime;
    }
    [Serializable]
    public struct MusicInfo
    {
        public float MusicStartTime;
        public float MusicBeatsTime;
    }

    [UpdateBefore(typeof(TransformSystemGroup))]
    public class AudioSystem : JobComponentSystem
    {
        #region Private
        private static MusicResources m_MusicResources;
        private static MusicRecordingInfo m_MusicRecordingInfo;
        #endregion

        #region Public
        /// <summary>
        /// The audio source for main backgroud music.
        /// </summary>
        public static AudioSource m_BeatsAudioSource;
        private static AudioSource m_MusicAudioSource;
        private static AudioSource m_KeyAudioSource;
        private static MusicResources.Music m_Music;
        private static MusicResources.Music m_Beats;
        private static MusicResources.Music m_KeySound;
        private static float _Deviation;
        public static AudioSource MusicAudioSource { get => m_MusicAudioSource; set => m_MusicAudioSource = value; }
        public static AudioSource BeatsAudioSource { get => m_BeatsAudioSource; set => m_BeatsAudioSource = value; }
        public static AudioSource KeyAudioSource { get => m_KeyAudioSource; set => m_KeyAudioSource = value; }
        
        public static float Deviation { get => _Deviation; set => _Deviation = value; }
        public static MusicResources.Music Music { get => m_Music; set => m_Music = value; }
        public static MusicResources.Music Beats { get => m_Beats; set => m_Beats = value; }



        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
        }

        public void Init()
        {
            ShutDown();
            m_MusicResources = CentralSystem.MusicResources;
            m_Music = m_MusicResources.Musics[2];
            m_Beats = m_MusicResources.Musics[1];
            m_KeySound = m_MusicResources.Musics[0];
            CreateMusic(m_Music.MusicClip);
            CreateBeats(m_Beats.MusicClip);
            CreateKeySound(m_KeySound.MusicClip);
            m_MusicAudioSource.Play();
            _Deviation = 0.1f;
            Enabled = true;

            _Deviation = m_Music.MusicInfo.MusicBeatsTime / 4;
        }

        public void ShutDown()
        {
            Enabled = false;

        }

        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods

        public static void StartRecording()
        {
            m_MusicRecordingInfo = default;
            m_MusicRecordingInfo.BeatCounter = 0;
        }

        public static void AddBeats()
        {
            if(m_MusicRecordingInfo.BeatCounter == 0)
            {
                m_MusicRecordingInfo.StartTime = m_MusicAudioSource.time;
            }
            m_MusicRecordingInfo.BeatCounter++;
            m_MusicRecordingInfo.CurrentBeatTime = m_MusicAudioSource.time;
        }

        public static void EndRecording()
        {
            float beatsTime = ((m_MusicRecordingInfo.CurrentBeatTime - m_MusicRecordingInfo.StartTime) / (m_MusicRecordingInfo.BeatCounter - 1));
            m_MusicRecordingInfo.StartTime = m_MusicRecordingInfo.StartTime % beatsTime;
            Debug.Log("[Starting Time]:" + m_MusicRecordingInfo.StartTime + " [Beat Time]: " + beatsTime);
        }

        public static void CreateMusic(AudioClip audioClip)
        {
            if (m_MusicAudioSource != null)
            {
                Debug.Log("Music AudioSource already exists! Use LoadMusic instead!");
                return;
            }
            m_MusicAudioSource = m_MusicResources.InstanciateAudioSource(audioClip);
        }

        public static void CreateBeats(AudioClip audioClip)
        {
            if (m_BeatsAudioSource != null)
            {
                Debug.Log("Music AudioSource already exists! Use LoadMusic instead!");
                return;
            }
            m_BeatsAudioSource = m_MusicResources.InstanciateAudioSource(audioClip);
        }

        public static void CreateKeySound(AudioClip audioClip)
        {
            if (m_KeyAudioSource != null)
            {
                Debug.Log("Music AudioSource already exists! Use LoadMusic instead!");
                return;
            }
            m_KeyAudioSource = m_MusicResources.InstanciateAudioSource(audioClip);
        }

        public static void LoadMusic(AudioClip audioClip)
        {
            if (m_MusicAudioSource == null)
            {
                Debug.Log("Music AudioSource does not exists! Use CreateMusic instead!");
                return;
            }
            m_MusicAudioSource.clip = audioClip;
        }

        public static void PrintMusicTime()
        {
            Debug.Log("Current play time: " + m_MusicAudioSource.time % 0.5f);
        }

        public static bool OnBeats()
        {
            float dev = Mathf.Abs((m_MusicAudioSource.time - m_Music.MusicInfo.MusicStartTime) % m_Music.MusicInfo.MusicBeatsTime);
            // Debug.Log("dev: " + dev);
            return dev <= _Deviation || dev >= m_Music.MusicInfo.MusicBeatsTime - _Deviation;
        }

        public static int CurrentBeatCounter()
        {
            return (int)((m_MusicAudioSource.time - m_Music.MusicInfo.MusicStartTime) / m_Music.MusicInfo.MusicBeatsTime);
        }

        #endregion

        #region Jobs
        #endregion

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}
