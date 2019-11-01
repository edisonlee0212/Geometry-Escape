using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape
{
    public class CentralSystemSettingController : MonoBehaviour
    {
        public CentralSystem m_CentralSystem;
        // Start is called before the first frame update
        void Start()
        {
            gameObject.AddComponent<AudioSource>();
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
        }

        public void OnOffsetButtonPressed()
        {
            SceneManager.LoadScene("Offset", LoadSceneMode.Single);
        }

        public void OnCharacterButtonPressed()
        {
            SceneManager.LoadScene("Character", LoadSceneMode.Single);
        }

        public void OnBackButtonPressed()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
