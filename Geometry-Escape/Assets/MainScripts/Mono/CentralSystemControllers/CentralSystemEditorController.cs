using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeometryEscape
{
    public class CentralSystemEditorController : MonoBehaviour
    {
        public CentralSystem m_CentralSystem;
        [SerializeField]
        private InputField m_InputField;
        public Text usernameText;

        // Start is called before the first frame update
        private void Start()
        {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            m_CentralSystem.Init(ControlMode.MapEditor);
            usernameText.text = PlayerPrefs.GetString("UserName", "PLAYER");
        }

        public void OnBackToMainMenu()
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

        public void OnPopupCloseButtonPressed()
        {
            UISystem.popup.SetActive(false);
        }

        public void OnLoadMap()
        {
            
            string name = m_InputField.text;
            if (name != "" && CentralSystem.Running)
            {
                WorldSystem.UnloadMap();
                FileSystem.LoadMapByName(name);
            }
        }

        public void OnSaveMap()
        {
            string name = m_InputField.text;
            if (name != "" && CentralSystem.Running)
            {
                FileSystem.SaveMapByName(name);
            }
        }

        public void OnUseMap()
        {
            string name = m_InputField.text;
            if (name != "" && CentralSystem.Running)
            {
                CentralSystem.MapName = name;
                CentralSystem.UseMap = true;
            }
            
        }
    }
}