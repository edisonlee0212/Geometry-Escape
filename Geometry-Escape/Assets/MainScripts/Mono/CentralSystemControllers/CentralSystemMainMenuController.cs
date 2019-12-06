using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace GeometryEscape {
    public class CentralSystemMainMenuController : MonoBehaviour {

        public CentralSystem m_CentralSystem;
        public AudioMixer mixer;
        public Slider volumeRocker;
        public InputField usernameInput;
        public Text usernameText;

        // Start is called before the first frame update
        void Start() {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
            volumeRocker.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f) * 100f;
            usernameText.text = PlayerPrefs.GetString("UserName", "PLAYER");
            usernameInput.text = PlayerPrefs.GetString("UserName", "PLAYER");
        }

        public void OnStartButtonPressed() {
            //source.PlayOneShot(sound);
            SceneManager.LoadScene("In-Game", LoadSceneMode.Single);
        }

        public void OnOptionButtonPressed() {
            SceneManager.LoadScene("Setting", LoadSceneMode.Single);
        }

        public void OnEditorButtonPressed()
        {
            SceneManager.LoadScene("Editor", LoadSceneMode.Single);
        }

        public void OnOffsetButtonPressed() {
            SceneManager.LoadScene("Offset", LoadSceneMode.Single);
        }

        public void OnCharacterButtonPressed() {
            SceneManager.LoadScene("Character", LoadSceneMode.Single);
        }

        public void OnUsernameInputEndEdit(string username) {
            usernameText.text = username;
            PlayerPrefs.SetString("UserName", username);
        }

        public void SetVolumeLevel(float sliderValue) {
            sliderValue /= 100f;
            mixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
            PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        }
    }
}