using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryEscape {
    public class CentralSystemMainMenuController : MonoBehaviour
    {
        public CentralSystem m_CentralSystem;
        // Start is called before the first frame update
        void Start()
        {
            m_CentralSystem = World.Active.GetOrCreateSystem<CentralSystem>();
        }

        public void OnStartButtonPressed()
        {

            SceneManager.LoadScene("In-Game", LoadSceneMode.Single);
        }
    }
}