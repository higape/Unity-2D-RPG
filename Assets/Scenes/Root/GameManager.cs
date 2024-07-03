using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        }
    }
}
