using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UISlideAnimation : MonoBehaviour
{
    public Vector2 startOffset = new Vector2(0, -150f);
    public float duration = 1.0f;
    public float delay = 0.2f;

    private RectTransform rectTransform;
    private Vector2 finalPosition;
    private CanvasGroup canvasGroup;
    private bool initialized = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        finalPosition = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        initialized = true;
    }

    void OnEnable()
    {
        if (!initialized) return;
        rectTransform.anchoredPosition = finalPosition + startOffset;
        canvasGroup.alpha = 0f;
        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        yield return new WaitForSecondsRealtime(delay); // Works even if game is paused!

        float time = 0;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float easeOutCurve = 1f - Mathf.Pow(1f - t, 3f);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, finalPosition, easeOutCurve);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, easeOutCurve);

            yield return null;
        }

        rectTransform.anchoredPosition = finalPosition;
        canvasGroup.alpha = 1f;
    }
}