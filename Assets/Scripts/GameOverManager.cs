using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // Drag your GameOverCanvas here

    void Start()
    {
        // Hide the Game Over screen when the game starts
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (gameOverUI != null) gameOverUI.SetActive(true);

        // Unlock the mouse so the player can click the buttons!
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Optional: Freeze the game time
        // Time.timeScale = 0f; 
    }

    public void ClickResurrect()
    {
        // Reloads the current level
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ClickReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your menu scene name!
    }
}