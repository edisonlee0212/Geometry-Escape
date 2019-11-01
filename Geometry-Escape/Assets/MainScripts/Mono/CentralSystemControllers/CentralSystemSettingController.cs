using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape
{
    public class CentralSystemSettingController : MonoBehaviour
    {
        public AudioClip sound;
        private AudioSource source { get { return GetComponent<AudioSource>(); } }

        public CentralSystem m_CentralSystem;
        // Start is called before the first frame update
        void Start()
        {
            gameObject.AddComponent<AudioSource>();
            source.clip = sound;
            source.playOnAwake = false;
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
        }

        public void OnOffsetButtonPressed()
        {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("Offset", LoadSceneMode.Single);
        }

        public void OnCharacterButtonPressed()
        {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("Character", LoadSceneMode.Single);
        }

        public void OnBackButtonPressed()
        {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
