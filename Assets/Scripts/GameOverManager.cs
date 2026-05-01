using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // Drag your GameOverCanvas here

    [Header("Game Over Stats UI")]
    [SerializeField] private TMP_Text soulText;  // Drag your SoulText here
    [SerializeField] private TMP_Text timeText;  // Drag your TimeText here

    // --- Static kill counter (accessible from anywhere) ---
    public static int killCount = 0;

    // --- Survival timer ---
    private float startTime;

    void Start()
    {
        // Hide the Game Over screen when the game starts
        if (gameOverUI != null) gameOverUI.SetActive(false);

        // Reset kill count every time the scene loads
        killCount = 0;

        // Start the survival timer
        startTime = Time.time;
    }

    /// <summary>
    /// Call from anywhere to add a kill: GameOverManager.killCount++;
    /// </summary>
    public static void AddKill()
    {
        killCount++;
    }

    public void TriggerGameOver()
    {
        if (gameOverUI != null) gameOverUI.SetActive(true);

        // Auto-find text objects by name if references are missing
        if (soulText == null && gameOverUI != null)
        {
            Transform found = gameOverUI.transform.Find("SoulsCount");
            if (found != null) soulText = found.GetComponent<TMP_Text>();
        }
        if (timeText == null && gameOverUI != null)
        {
            Transform found = gameOverUI.transform.Find("TimeCount");
            if (found != null) timeText = found.GetComponent<TMP_Text>();
        }

        // If still not found, search ALL children (nested objects)
        if (soulText == null && gameOverUI != null)
        {
            foreach (TMP_Text t in gameOverUI.GetComponentsInChildren<TMP_Text>(true))
            {
                if (t.gameObject.name == "SoulsCount") { soulText = t; break; }
            }
        }
        if (timeText == null && gameOverUI != null)
        {
            foreach (TMP_Text t in gameOverUI.GetComponentsInChildren<TMP_Text>(true))
            {
                if (t.gameObject.name == "TimeCount") { timeText = t; break; }
            }
        }

        // Calculate survival time
        float survivalTime = Time.time - startTime;
        int minutes = Mathf.FloorToInt(survivalTime / 60f);
        int seconds = Mathf.FloorToInt(survivalTime % 60f);

        // Update the Soul Text (kill count)
        if (soulText != null)
            soulText.text = killCount.ToString();
        else
            Debug.LogWarning("GameOverManager: Could not find SoulsCount text!");

        // Update the Time Text (survival time)
        if (timeText != null)
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        else
            Debug.LogWarning("GameOverManager: Could not find TimeCount text!");

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