using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            ResourceManager.CheckAndCreatePath();
            SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        }
    }
}
