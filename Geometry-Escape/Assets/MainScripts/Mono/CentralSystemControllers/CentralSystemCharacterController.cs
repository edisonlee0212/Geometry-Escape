using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace GeometryEscape
{
    public class CentralSystemCharacterController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnBackToMainMenu()
        {

            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

}
