using UnityEngine;
using System.Collections;

public class UIIntroAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How far it moves from its starting point. (e.g., Y = 200 means it starts 200 pixels UP)")]
    public Vector2 startOffset = new Vector2(0, 200f);
    public float duration = 1.2f; // How long the animation lasts
    public float delay = 0f;      // Wait a moment before starting

    private RectTransform rectTransform;
    private Vector2 finalPosition;
    private CanvasGroup canvasGroup;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        finalPosition = rectTransform.anchoredPosition;

        // Auto-add a CanvasGroup to handle the fading/opacity
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Set to starting position (invisible and moved away)
        rectTransform.anchoredPosition = finalPosition + startOffset;
        canvasGroup.alpha = 0f;

        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        yield return new WaitForSeconds(delay);

        float time = 0;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // This math creates the perfect "Figma Ease Out" curve!
            // It starts fast and slows down smoothly at the end.
            float easeOutCurve = 1f - Mathf.Pow(1f - t, 3f);

            // Move and Fade
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, finalPosition, easeOutCurve);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, easeOutCurve);

            yield return null;
        }

        // Snap exactly to final position at the end
        rectTransform.anchoredPosition = finalPosition;
        canvasGroup.alpha = 1f;
    }
}