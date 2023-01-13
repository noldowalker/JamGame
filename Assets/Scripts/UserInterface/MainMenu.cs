using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        public void OnStartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}