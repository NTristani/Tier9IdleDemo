using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text coinsText;

    [Header("Bars")]
    [SerializeField] private Slider xpSlider;

    private void OnEnable()
    {
        GameEvents.PlayerStatsChanged += HandlePlayerStatsChanged;
        GameEvents.CoinsChanged += HandleCoinsChanged;
    }

    private void OnDisable()
    {
        GameEvents.PlayerStatsChanged -= HandlePlayerStatsChanged;
        GameEvents.CoinsChanged -= HandleCoinsChanged;
    }

    private void HandlePlayerStatsChanged(int level, int currentXp, int requiredXp)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv. {level}";
        }

        if (xpText != null)
        {
            xpText.text = $"{currentXp} / {requiredXp} XP";
        }

        if (xpSlider != null)
        {
            xpSlider.maxValue = requiredXp;
            xpSlider.value = currentXp;
        }
    }

    private void HandleCoinsChanged(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = $"{coins} Coins";
        }
    }
}