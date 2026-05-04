using UnityEngine;
using System.Collections;

public class SceneAudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    
    [Tooltip("How long the fade in should take in seconds")]
    public float fadeDuration = 1.5f; // We will match the UI to this!
    
    private float maxVolume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Remember the volume you set in the Inspector
        maxVolume = audioSource.volume; 
        
        // Start at 0 volume for the fade-in
        audioSource.volume = 0f; 
        
        // Make sure it plays, then start fading in
        audioSource.Play();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Gradually increase volume
            audioSource.volume = Mathf.Lerp(0f, maxVolume, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = maxVolume; // Ensure it finishes exactly at max volume
    }
}