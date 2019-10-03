using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape
{
    public class CentralSystemOffsetController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_HealthStatusText;

        public CentralSystem m_CentralSystem;
        private static int test_time;
        public static int _offset;
        public static TextMeshProUGUI OffsetText;
        // Start is called before the first frame update
        void Start()
        {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            m_CentralSystem.OffsetInit();
            test_time = 0;
            _offset = 0;
            OffsetText = m_HealthStatusText;
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

        public static void updateOffset(int offset)
        {
            Debug.Log(_offset);
            Debug.Log(offset);
            test_time += 1;
            _offset = (_offset * (test_time - 1) + offset) / test_time;
            OffsetText.text = "Offset " + _offset;
        }

        public void resetTest()
        {
            test_time = 0;
            _offset = 0;
            OffsetText.text = "Offset " + _offset;
        }


    }
}
