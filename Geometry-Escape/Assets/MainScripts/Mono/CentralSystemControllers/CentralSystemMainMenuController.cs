using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape {
    public class CentralSystemMainMenuController : MonoBehaviour {
        public AudioClip sound;
        private AudioSource source { get { return GetComponent<AudioSource>(); } }

        public CentralSystem m_CentralSystem;
        // Start is called before the first frame update
        void Start() {
            gameObject.AddComponent<AudioSource>();
            source.clip = sound;
            source.playOnAwake = false;
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
        }

        public void OnStartButtonPressed() {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("In-Game", LoadSceneMode.Single);
        }

        public void OnExitButtonPressed() {
            source.PlayOneShot(sound);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OnOptionButtonPressed() {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("Offset", LoadSceneMode.Single);
        }

        public void OnEditorButtonPressed()
        {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("Editor", LoadSceneMode.Single);
        }
    }
}