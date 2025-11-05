using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Loads a new scene by name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Optional - exits game (will only work in build, not editor)
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
