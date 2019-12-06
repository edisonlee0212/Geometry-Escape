using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeometryEscape {
    public class CentralSystemInGameController : MonoBehaviour {
        public CentralSystem m_CentralSystem;
        public Text TileTypeText;
        public Text usernameText;

        public static Text TileText;
        // Start is called before the first frame update
        private void Start() {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            m_CentralSystem.Init();
            TileText = TileTypeText;
            usernameText.text = PlayerPrefs.GetString("UserName", "PLAYER");
        }

        public static void ChangeTileTest(string text)
        {
            TileText.text = text;
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

        public void OnPopupCloseButtonPressed() {
            UISystem.popup.SetActive(false);
        }

        
    }
}