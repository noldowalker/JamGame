using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] public Image LoadingBar;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return new WaitForSeconds(2f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progressValue = operation.progress;

            LoadingBar.fillAmount = progressValue - 0.2f;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        LoadingBar.fillAmount = 1f;
        
        yield return new WaitForSeconds(2f);
        Destroy(this);
    }
}
