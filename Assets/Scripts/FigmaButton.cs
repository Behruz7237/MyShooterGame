using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FigmaButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    public TextMeshProUGUI buttonText;
    public Image buttonBackground;
    public Outline buttonOutline;

    [Header("Default State (Image 2)")]
    public Color defaultTextColor = Color.white;
    public Color defaultBackgroundColor = new Color(0f, 0f, 0f, 0.4f); // Dark/Transparent
    public Color defaultOutlineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Faint Grey

    [Header("Hover State (Image 1)")]
    public Color hoverTextColor = new Color(0.85f, 0.65f, 0.35f, 1f); // Bronze/Gold
    public Color hoverBackgroundColor = new Color(0.85f, 0.65f, 0.35f, 0.1f); // Faint Gold Fill
    public Color hoverOutlineColor = new Color(0.85f, 0.65f, 0.35f, 1f); // Solid Gold Border

    void OnEnable()
    {
        // Auto-find the components
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonBackground == null) buttonBackground = GetComponent<Image>();
        if (buttonOutline == null) buttonOutline = GetComponent<Outline>();

        // Set to default immediately
        ApplyState(defaultTextColor, defaultBackgroundColor, defaultOutlineColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyState(hoverTextColor, hoverBackgroundColor, hoverOutlineColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyState(defaultTextColor, defaultBackgroundColor, defaultOutlineColor);
    }

    private void ApplyState(Color txt, Color bg, Color outline)
    {
        if (buttonText != null) buttonText.color = txt;
        if (buttonBackground != null) buttonBackground.color = bg;
        if (buttonOutline != null) buttonOutline.effectColor = outline;
    }
}