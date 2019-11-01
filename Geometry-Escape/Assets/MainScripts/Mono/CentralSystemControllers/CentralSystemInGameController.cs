using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape {
    public class CentralSystemInGameController : MonoBehaviour {
        public CentralSystem m_CentralSystem;
        private bool is_volume_rocker_visible;

        // Start is called before the first frame update
        private void Start() {
            is_volume_rocker_visible = false;
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            m_CentralSystem.Init();
        }

        public void OnBackToMainMenu() {
            m_CentralSystem.ShutDown();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        public void OnPause() {
            CentralSystem.Pause();
        }

        public void OnResume() {
            CentralSystem.Resume();
        }

        public void OnVolumeButtonPressed() {
            is_volume_rocker_visible = !is_volume_rocker_visible;

            UISystem.volume_rocker.SetActive(is_volume_rocker_visible);
        }

        public void SetVolumeLevel(float sliderValue) {

        }

        public void OnPopupCloseButtonPressed() {
            UISystem.popup.SetActive(false);
        }
    }
}