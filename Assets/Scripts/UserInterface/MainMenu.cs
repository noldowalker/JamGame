using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] public GameObject AuthorsPanel;
        public void OnStartGame()
        {
            UIService.Current.LoadSceneWithScreen("SampleScene");
        }

        private void Awake()
        {
            OnAuthorsClose();
        }

        public void OnAuthors()
        {
            AuthorsPanel.SetActive(true);
        }
        
        public void OnAuthorsClose()
        {
            AuthorsPanel.SetActive(false);
        }
    }
}