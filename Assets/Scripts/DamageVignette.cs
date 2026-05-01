using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Flashes a red vignette overlay on the screen when the player takes damage.
/// Attach to a full-screen UI Image with the DamageVignette sprite.
/// </summary>
public class DamageVignette : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float flashAlpha = 0.6f;   // How bright the red flash gets
    [SerializeField] private float fadeInTime = 0.05f;   // Snap in fast (instant hit feel)
    [SerializeField] private float fadeOutTime = 0.4f;   // Smooth fade out

    private Image vignetteImage;
    private Tweener currentTween;

    // Singleton so PlayerHealth can find it easily
    public static DamageVignette Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        vignetteImage = GetComponent<Image>();

        // Start fully transparent
        if (vignetteImage != null)
        {
            Color c = vignetteImage.color;
            c.a = 0f;
            vignetteImage.color = c;
        }
    }

    /// <summary>
    /// Call this to flash the red vignette.
    /// </summary>
    public void Flash()
    {
        if (vignetteImage == null) return;

        // Kill any running fade so hits stack properly
        currentTween?.Kill();

        // Snap to peak alpha, then fade out
        Color c = vignetteImage.color;
        c.a = flashAlpha;
        vignetteImage.color = c;

        currentTween = vignetteImage.DOFade(0f, fadeOutTime)
            .SetEase(Ease.OutQuad);
    }
}
