using TMPro;
using UnityEngine;

public class CombatStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text combatStatsText;

    private void OnEnable()
    {
        GameEvents.CombatStatsChanged += HandleCombatStatsChanged;
    }

    private void OnDisable()
    {
        GameEvents.CombatStatsChanged -= HandleCombatStatsChanged;
    }

    private void HandleCombatStatsChanged(int damage, float attacksPerSecond)
    {
        if (combatStatsText == null)
        {
            return;
        }

        combatStatsText.text = $"DMG: {damage}  SPD: {attacksPerSecond:0.00}/s";
    }
}