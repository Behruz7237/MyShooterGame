using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class VillageManager : MonoBehaviour
{
    [Header("Village Setup")]
    public List<VikingHealth> villageVikings;
    public GameObject bossMutant;

    [Header("Cinematic Settings")]
    public TMP_Text cinematicText;
    public Material bossSkybox;

    [Header("Lighting Changes")]
    [Tooltip("Drag your 'Directional light' from the Hierarchy here")]
    public Light mainSun;
    public Color bossSunColor = new Color(0.8f, 0.2f, 0.1f); // Spooky Dark Red/Orange
    public float bossSunIntensity = 0.3f; // Dim the sun!
    public Color bossAmbientColor = new Color(0.1f, 0.05f, 0.1f); // Dark purple/black shadows

    void Start()
    {
        if (bossMutant != null) bossMutant.SetActive(false);
        if (cinematicText != null) cinematicText.gameObject.SetActive(false);
    }

    public void ReportDeath(VikingHealth deadViking)
    {
        if (villageVikings.Contains(deadViking))
        {
            villageVikings.Remove(deadViking);
        }

        if (villageVikings.Count == 0)
        {
            StartCoroutine(CinematicRoutine());
        }
    }

    private IEnumerator CinematicRoutine()
    {
        // 1. CHANGE THE SKYBOX
        if (bossSkybox != null)
        {
            RenderSettings.skybox = bossSkybox;
        }

        // 2. PLUNGE THE WORLD INTO DARKNESS
        // Force the ambient shadow light to be our dark color
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = bossAmbientColor;

        // Dim the actual Sun and make it look evil
        if (mainSun != null)
        {
            mainSun.color = bossSunColor;
            mainSun.intensity = bossSunIntensity;
            mainSun.shadowStrength = 1f; // Make shadows super harsh!
        }

        // Tell Unity to update the reflections to match the new sky!
        DynamicGI.UpdateEnvironment();

        // 3. PLAY THE TEXT CINEMATIC
        if (cinematicText != null)
        {
            cinematicText.gameObject.SetActive(true);

            float interval = 1.5f;

            SlamWord("RAGNAROK", interval);
            yield return new WaitForSeconds(interval);

            SlamWord("BEGINS", interval);
            yield return new WaitForSeconds(interval);

            SlamWord("IN...", interval);
            yield return new WaitForSeconds(interval);

            SlamWord("3", interval);
            yield return new WaitForSeconds(interval);

            SlamWord("2", interval);
            yield return new WaitForSeconds(interval);

            SlamWord("1", interval);
            yield return new WaitForSeconds(interval);

            cinematicText.text = "";
            cinematicText.gameObject.SetActive(false);
        }

        // 4. SPAWN THE BOSS
        if (bossMutant != null)
        {
            bossMutant.SetActive(true);
        }
    }

    private void SlamWord(string word, float duration = 1f)
    {
        cinematicText.text = word;

        // Force the text to be perfectly centered on the screen
        cinematicText.alignment = TextAlignmentOptions.Center;
        RectTransform rect = cinematicText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }

        cinematicText.transform.DOKill();
        cinematicText.transform.localPosition = Vector3.zero; // Reset position
        cinematicText.transform.localEulerAngles = Vector3.zero;

        // Ominous heavy slam: starts huge, smashes down, then slowly creeps forward like a giant approaching
        cinematicText.transform.localScale = Vector3.one * 8f;
        
        cinematicText.transform.DOScale(Vector3.one * 1.5f, 0.15f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            // Simulate a massive, heavy footstep shaking the screen
            cinematicText.transform.DOPunchPosition(new Vector3(0, -30f, 0), 0.4f, 10, 0.5f);
            
            // Slowly creep closer to the camera to build tension before the next word
            cinematicText.transform.DOScale(Vector3.one * 2.2f, duration - 0.15f).SetEase(Ease.Linear);
        });
    }
}