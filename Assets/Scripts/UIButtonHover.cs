using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.EventSystems; // For detecting hovers

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;

    [Header("Hover Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.9f, 0.2f); // That slight yellow!
    public float hoverSizeMultiplier = 1.1f; // Makes text 10% bigger on hover

    private Vector3 originalScale;

    void Start()
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = buttonText.transform.localScale;
        buttonText.color = normalColor;
    }

    // When the mouse enters the button (Figma Hover State)
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        buttonText.transform.localScale = originalScale * hoverSizeMultiplier;
    }

    // When the mouse leaves the button (Figma Default State)
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
        buttonText.transform.localScale = originalScale;
    }
}