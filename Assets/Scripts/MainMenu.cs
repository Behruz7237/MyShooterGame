using UnityEngine;
using UnityEngine.SceneManagement; // This lets us load scenes!

public class MainMenu : MonoBehaviour
{
    // Make sure you have added your game scene to the Build Settings!
    public string gameSceneName = "PlayScene"; // Or whatever your Viking village scene is called!

    public void ClickPlay()
    {
        Debug.Log("Loading Game...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void ClickQuit()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

        // This line only works when testing in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}