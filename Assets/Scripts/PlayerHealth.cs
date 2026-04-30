using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    // Made public or use SerializeField just to see it in Inspector if desired, 
    // but the prompt says "int currentHealth"
    [SerializeField] private int currentHealth;

    [Header("References")]
    [Tooltip("Drag the PlayerMovement script here (optional, auto-finds on Start)")]
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private TMP_Text _healhText;
    [SerializeField] private Image _fillImage;

    private Tweener _fillTweener;

    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Auto-find PlayerMovement component if not assigned
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
    }

    /// <summary>
    /// Call this method to deal damage to the player.
    /// </summary>
    public void TakeDamage(int damage)
    {
        // Prevent damage if already dead
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        ShowHEalth();
    }

    private void Die()
    {
        Debug.Log("Player Died!");

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void ShowHEalth()
    {
        if (currentHealth <= 0) return;
        _healhText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        _fillTweener?.Kill();
        _fillTweener = _fillImage.DOFillAmount((float)currentHealth / maxHealth, 0.3f);
    }

    private void SaveRecord()
    {
        PlayerPrefs.SetInt("MaxDiedCount", 50);
        PlayerPrefs.GetInt("MaxDiedCount", 0);
    }

}