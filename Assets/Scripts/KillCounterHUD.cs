using TMPro;
using UnityEngine;

public class KillCounterHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text killCountText;

    void Update()
    {
        if (killCountText != null)
        {
            killCountText.text = GameOverManager.killCount.ToString();
        }
    }
}
