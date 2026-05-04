using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;

    [Header("Hover Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.9f, 0.2f);
    public float hoverSizeMultiplier = 1.1f;

    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    private Vector3 originalScale;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    void Start()
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = buttonText.transform.localScale;
        buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        buttonText.transform.localScale = originalScale * hoverSizeMultiplier;
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
        buttonText.transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}