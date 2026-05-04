using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class FigmaButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Components")]
    public TextMeshProUGUI buttonText;
    public Image buttonBackground;
    public Outline buttonOutline;

    [Header("Default State")]
    public Color defaultTextColor = Color.white;
    public Color defaultBackgroundColor = new Color(0f, 0f, 0f, 0.4f);
    public Color defaultOutlineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Hover State")]
    public Color hoverTextColor = new Color(0.85f, 0.65f, 0.35f, 1f);
    public Color hoverBackgroundColor = new Color(0.85f, 0.65f, 0.35f, 0.1f);
    public Color hoverOutlineColor = new Color(0.85f, 0.65f, 0.35f, 1f);

    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound for UI
    }

    void OnEnable()
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonBackground == null) buttonBackground = GetComponent<Image>();
        if (buttonOutline == null) buttonOutline = GetComponent<Outline>();

        ApplyState(defaultTextColor, defaultBackgroundColor, defaultOutlineColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyState(hoverTextColor, hoverBackgroundColor, hoverOutlineColor);
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyState(defaultTextColor, defaultBackgroundColor, defaultOutlineColor);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void ApplyState(Color txt, Color bg, Color outline)
    {
        if (buttonText != null) buttonText.color = txt;
        if (buttonBackground != null) buttonBackground.color = bg;
        if (buttonOutline != null) buttonOutline.effectColor = outline;
    }
}