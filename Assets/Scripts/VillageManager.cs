using UnityEngine;
using System.Collections.Generic;

public class VillageManager : MonoBehaviour
{
    [Header("Village Setup")]
    [Tooltip("Drag all the Vikings in this village into this list")]
    public List<VikingHealth> villageVikings;

    [Tooltip("Drag your deactivated Mutant Boss here")]
    public GameObject bossMutant;

    void Start()
    {
        // Make double sure the boss is hidden when the game starts
        if (bossMutant != null) bossMutant.SetActive(false);
    }

    // The Vikings will call this function when they die
    public void ReportDeath(VikingHealth deadViking)
    {
        if (villageVikings.Contains(deadViking))
        {
            villageVikings.Remove(deadViking); // Remove him from the living list
        }

        // Are they all dead?
        if (villageVikings.Count == 0)
        {
            Debug.Log("Village Cleared! Spawning Boss!");
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (bossMutant != null)
        {
            // This makes him visible AND starts his animation sequence!
            bossMutant.SetActive(true);
        }
    }
}