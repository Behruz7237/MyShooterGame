using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // Drag your GameOverCanvas here

    [Header("Game Over Stats UI")]
    [SerializeField] private TMP_Text soulText;  // Drag your SoulText here
    [SerializeField] private TMP_Text timeText;  // Drag your TimeText here

    [Header("Audio Settings")]
    [Tooltip("Drag the AudioSource playing the Viking background song here so it doesn't get muted.")]
    [SerializeField] private AudioSource backgroundSongSource;
    [Tooltip("Drag your Game Over/Victory sound effect here.")]
    [SerializeField] private AudioClip gameOverSound;

    private AudioSource audioSource;

    // --- Static kill counter (accessible from anywhere) ---
    public static int killCount = 0;

    // --- Survival timer ---
    private float startTime;

    void Awake()
    {
        // Get or add an AudioSource to play the GameOver sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // Ensure it plays 2D (not affected by distance)
        }
    }

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

        HandleGameOverAudio();
    }

    public void TriggerVictory(string victoryWord = "RAGNAROK")
    {
        TriggerGameOver();
        
        if (gameOverUI != null)
        {
            foreach (TMP_Text t in gameOverUI.GetComponentsInChildren<TMP_Text>(true))
            {
                if (t.gameObject.name == "TitleText") 
                { 
                    t.text = victoryWord;
                    t.color = Color.red; // RAGNAROK in red!
                    
                    // Animate from zero scale with a fade
                    t.transform.localScale = Vector3.zero;
                    t.alpha = 0f;
                    t.DOFade(1f, 1.2f).SetUpdate(true);
                    t.transform.DOScale(Vector3.one, 1.2f).SetEase(Ease.OutBack).SetUpdate(true);
                }
                else if (t.gameObject.name == "SubtitleText")
                {
                    t.text = "YOU HAVE AVERTED";
                    
                    // Animate subtitle with a delay
                    t.transform.localScale = Vector3.zero;
                    t.alpha = 0f;
                    t.DOFade(1f, 1.2f).SetDelay(0.8f).SetUpdate(true);
                    t.transform.DOScale(Vector3.one, 1.2f).SetEase(Ease.OutBack).SetDelay(0.8f).SetUpdate(true);
                }
            }
        }
    }

    private void HandleGameOverAudio()
    {
        // 1. Find every AudioSource in the game
        // Using FindObjectsOfType to ensure backwards compatibility with all Unity versions
        AudioSource[] allAudioSources = Object.FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            // Don't mute the background song, our own AudioSource, or UI click sounds
            if (source == backgroundSongSource || source == audioSource)
                continue;

            // Pause or mute them
            source.Pause();
        }

        // 2. Play the Game Over/Victory Sound Effect
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
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
