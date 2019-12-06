using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeometryEscape
{
    public class CentralSystemOffsetController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_HealthStatusText;

        public Button BackToMenu, ResetOffset, ChangeMode, StartRecording, EndRecording;
        //public Text usernameText;

        public CentralSystem m_CentralSystem;
        private static int test_time;
        private static int[] offsets;
        public static int _offset;
        public static TextMeshProUGUI OffsetText;
        public static bool isRecordingBeat;
        // Start is called before the first frame update
        void Start()
        {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            m_CentralSystem.OffsetInit();
            test_time = 0;
            _offset = 0;
            OffsetText = m_HealthStatusText;
            offsets = new int[20];
            //usernameText.text = PlayerPrefs.GetString("UserName", "PLAYER");
        }

        public void OnBackMainMenu()
        {
            m_CentralSystem.ShutDown();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        public void OnPause()
        {
            CentralSystem.Pause();
        }

        public void OnResume()
        {
            CentralSystem.Resume();
        }

        public void OnChangeMode()
        {
            isRecordingBeat = !isRecordingBeat;
            if (isRecordingBeat)
            {
                AudioSystem.MusicAudioSource.Stop();
                ResetOffset.interactable = false;
                StartRecording.interactable = true;
                EndRecording.interactable = false;
            }
            else
            {
                ResetOffset.interactable = true;
                StartRecording.interactable = false;
                EndRecording.interactable = false;
            }
        }

        public void OnStartRecording()
        {
            StartRecording.interactable = false;
            EndRecording.interactable = true;
            ChangeMode.interactable = false;
            AudioSystem.StartRecording();
        }

        public void OnEndRecording()
        {
            StartRecording.interactable = true;
            EndRecording.interactable = false;
            ChangeMode.interactable = true;
            AudioSystem.EndRecording();
        }


        public static void updateOffset(int offset)
        {
            if (!isRecordingBeat)
            {
                Debug.Log(_offset);
                Debug.Log(offset);
                offsets[test_time % 20] = offset;
                test_time += 1;
                _offset = test_time <= 20 ? sumArray(offsets) / test_time : sumArray(offsets) / 20;
                OffsetText.text = "Offset " + _offset + "ms";
            }
            else
            {
                AudioSystem.RecordBeat();
            }
        }

        public static int sumArray(int[] array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i ++)
            {
                sum += array[i];
            }
            return sum;
        }

        public void resetTest()
        {
            test_time = 0;
            _offset = 0;
            offsets = new int[20];
            OffsetText.text = "Offset " + _offset + "ms";
        }


    }
}
