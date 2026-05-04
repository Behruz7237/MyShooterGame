using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "PlayScene";

    [Header("Audio Settings")]
    [Tooltip("Drag your button click sound here")]
    public AudioClip clickSoundClip;

    private void PlayClickSoundAndExecute(System.Action onComplete)
    {
        if (clickSoundClip != null)
        {
            GameObject soundObj = new GameObject("TempClickSound");
            DontDestroyOnLoad(soundObj);
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clickSoundClip;
            source.spatialBlend = 0f;
            source.Play();
            Destroy(soundObj, clickSoundClip.length);
        }
        StartCoroutine(ExecuteAfterDelay(onComplete, 0.2f));
    }

    private IEnumerator ExecuteAfterDelay(System.Action onComplete, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        onComplete?.Invoke();
    }

    public void ClickPlay()
    {
        PlayClickSoundAndExecute(() => SceneManager.LoadScene(gameSceneName));
    }

    public void ClickQuit()
    {
        PlayClickSoundAndExecute(() => 
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }
}