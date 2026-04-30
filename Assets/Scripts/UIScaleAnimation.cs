using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIScaleAnimation : MonoBehaviour
{
    [Tooltip("How small it starts. 0.5 means half size.")]
    public Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
    public float duration = 1.0f;
    public float delay = 0f;

    private Vector3 finalScale;
    private CanvasGroup canvasGroup;
    private bool initialized = false;

    void Awake()
    {
        finalScale = transform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        initialized = true;
    }

    void OnEnable()
    {
        if (!initialized) return;
        transform.localScale = startScale;
        canvasGroup.alpha = 0f;
        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        yield return new WaitForSecondsRealtime(delay);

        float time = 0;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            // Adds a tiny "bounce" or smooth pop at the end!
            float easeOutCurve = 1f - Mathf.Pow(1f - t, 3f);

            transform.localScale = Vector3.Lerp(startScale, finalScale, easeOutCurve);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, easeOutCurve);

            yield return null;
        }

        transform.localScale = finalScale;
        canvasGroup.alpha = 1f;
    }
}