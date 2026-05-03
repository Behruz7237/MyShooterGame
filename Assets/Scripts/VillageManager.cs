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

            SlamWord("RAGNAROK");
            yield return new WaitForSeconds(1f);

            SlamWord("BEGINS");
            yield return new WaitForSeconds(1f);

            SlamWord("IN...");
            yield return new WaitForSeconds(1f);

            SlamWord("3");
            yield return new WaitForSeconds(1f);

            SlamWord("2");
            yield return new WaitForSeconds(1f);

            SlamWord("1");
            yield return new WaitForSeconds(1f);

            cinematicText.text = "";
            cinematicText.gameObject.SetActive(false);
        }

        // 4. SPAWN THE BOSS
        if (bossMutant != null)
        {
            bossMutant.SetActive(true);
        }
    }

    private void SlamWord(string word)
    {
        cinematicText.text = word;

        cinematicText.transform.localScale = Vector3.one * 5f;
        cinematicText.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBounce);
    }
}