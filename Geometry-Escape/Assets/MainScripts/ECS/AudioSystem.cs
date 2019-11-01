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
        private static AudioResources m_MusicResources;
        private static MusicRecordingInfo m_MusicRecordingInfo;
        #endregion

        #region Public
        /// <summary>
        /// The audio source for main backgroud music.
        /// </summary>
        //private static AudioSource m_BeatsAudioSource;
        private static AudioSource[] m_SoundEffectAudioSources;
        private static AudioSource m_MusicAudioSource;
        //private static AudioSource m_KeyAudioSource;
        private static AudioResources.Music m_Music;
        //private static MusicResources.Music m_Beats;
        //private static MusicResources.Music m_KeySound;
        private static float _Deviation;
        public static AudioSource MusicAudioSource { get => m_MusicAudioSource; set => m_MusicAudioSource = value; }
        //public static AudioSource BeatsAudioSource { get => m_BeatsAudioSource; set => m_BeatsAudioSource = value; }
        //public static AudioSource KeyAudioSource { get => m_KeyAudioSource; set => m_KeyAudioSource = value; }


        public static float Deviation { get => _Deviation; set => _Deviation = value; }
        public static AudioResources.Music Music { get => m_Music; set => m_Music = value; }
        //public static MusicResources.Music Beats { get => m_Beats; set => m_Beats = value; }
        public static AudioSource[] SoundEffectAudioSources { get => m_SoundEffectAudioSources; set => m_SoundEffectAudioSources = value; }



        #endregion

        #region Managers
        protected override void OnCreate()
        {
            Enabled = false;
        }

        public void Init()
        {
            ShutDown();
            m_MusicResources = CentralSystem.AudioResources;
            m_Music = m_MusicResources.Musics[0];
            m_SoundEffectAudioSources = new AudioSource[m_MusicResources.SoundEffects.Length];
            CreateMusic(m_Music.MusicClip);
            CreateSoundEffects(m_MusicResources.SoundEffects);
            m_MusicAudioSource.Play();
            _Deviation = 0.1f;
            Enabled = true;

            _Deviation = m_Music.MusicInfo.MusicBeatsTime / 4;
            UISystem._devision = m_Music.MusicInfo.MusicBeatsTime;
        }

        public void Pause()
        {
            Debug.Log("Audio System Paused!");
            if (m_MusicAudioSource != null) m_MusicAudioSource.Pause();
            Enabled = false;
        }


        public void Resume()
        {
            if (m_MusicAudioSource != null) m_MusicAudioSource.UnPause();
            Enabled = true;
        }



        public void ShutDown()
        {
            Enabled = false;
            if (m_MusicAudioSource != null) m_MusicAudioSource.Stop();
        }

        protected override void OnDestroy()
        {
            ShutDown();
        }
        #endregion

        #region Methods

        #region Beats Editor
        public static void StartRecording()
        {
            m_MusicRecordingInfo = default;
            m_MusicRecordingInfo.BeatCounter = 0;
        }

        public static void AddBeats()
        {
            if (m_MusicRecordingInfo.BeatCounter == 0)
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
        #endregion

        #region Create Audio Sources
        public static void CreateMusic(AudioClip audioClip)
        {
            if (m_MusicAudioSource != null)
            {
                Debug.Log("Music AudioSource already exists! Use LoadMusic instead!");
                return;
            }
            m_MusicAudioSource = m_MusicResources.InstanciateAudioSource(audioClip);
        }

        /*public static void CreateBeats(AudioClip audioClip)
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
        }*/

        public static void CreateSoundEffects(AudioClip[] audioClips)
        {
            int amount = audioClips.Length;
            for (int i = 0; i < amount; i++)
            {
                m_SoundEffectAudioSources[i] = m_MusicResources.InstanciateAudioSource(audioClips[i]);
            }
        }
        #endregion

        #region Reload Audio Sources
        public static void ReloadMusic(AudioClip audioClip)
        {
            if (m_MusicAudioSource == null)
            {
                Debug.Log("Music AudioSource doesn't exists! Use CreateMusic instead!");
                return;
            }
            m_MusicAudioSource.clip = audioClip;
        }
        #endregion

        #region Play Sound Effects
        public static void PlayKeySound()
        {
            m_SoundEffectAudioSources[0].Play();
        }

        public static void PlayBeatSound()
        {
            m_SoundEffectAudioSources[1].Play();
        }
        public static void PlayTrapSound()
        {
            m_SoundEffectAudioSources[2].Play();
        }

        public static void StopTrapSound()
        {
            m_SoundEffectAudioSources[2].Stop();
        }
        #endregion

        public static void PrintMusicTime()
        {
            Debug.Log("Current play time: " + m_MusicAudioSource.time % 0.5f);
        }

        public static bool OnBeats()
        {
            float dev = Mathf.Abs((m_MusicAudioSource.time - m_Music.MusicInfo.MusicStartTime) % m_Music.MusicInfo.MusicBeatsTime);

            Debug.Log("dev: " + dev);
            Debug.Log("offset: " + CentralSystemOffsetController._offset);
            return dev <= _Deviation || dev >= m_Music.MusicInfo.MusicBeatsTime - _Deviation;
        }

        public static int CalOffset()
        {
            int offset = (int)(100 * (float)((m_MusicAudioSource.time - m_Music.MusicInfo.MusicStartTime) % m_Music.MusicInfo.MusicBeatsTime));
            return offset;
        }

        public static int CurrentBeatCounter()
        {
            return (int)((m_MusicAudioSource.time - m_Music.MusicInfo.MusicStartTime) / m_Music.MusicInfo.MusicBeatsTime);
        }

        private static float _AcclerateTimer;
        private static bool _Acclerating;
        public static void AcclerateMusic(float time, float speed)
        {
            _AcclerateTimer = time;
            m_MusicAudioSource.pitch = speed;
            _Acclerating = true;
        }

        #endregion

        #region Jobs
        #endregion
        public JobHandle OnBeatUpdate(ref JobHandle inputDeps, int beatCounter)
        {
            PlayBeatSound();
            //Schedule your job for every beat here.
            return inputDeps;
        }

        public JobHandle OnFixedUpdate(ref JobHandle inputDeps, int counter)
        {
            //Schedule your job for every time step here. Time step is defined in central system.
            return inputDeps;
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_Acclerating)
            {
                _AcclerateTimer -= Time.deltaTime;
                if (_AcclerateTimer <= 0)
                {
                    _Acclerating = false;
                    m_MusicAudioSource.pitch = 1;
                }
            }

            return inputDeps;
        }
    }
}
