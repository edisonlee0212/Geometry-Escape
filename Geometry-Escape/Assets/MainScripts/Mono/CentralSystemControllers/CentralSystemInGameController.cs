﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape
{
    public class CentralSystemInGameController : MonoBehaviour
    {
        public CentralSystem m_CentralSystem;
        // Start is called before the first frame update
        private void Start()
        {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            m_CentralSystem.Init();
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
    }
}