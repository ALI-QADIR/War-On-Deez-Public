using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();

    // a start game function which asynchronously loads the next scene after waiting for 2 seconds
    public void StartGame()
    {
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(2f);
        _scenesLoading.Add(SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}