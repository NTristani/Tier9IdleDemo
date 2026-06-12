using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflineProgressPopupUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button closeButton;

    [Header("Debug")]
    [SerializeField] private bool logPopupEvents = true;

    private void Awake()
    {
        if (popupCanvasGroup == null)
        {
            popupCanvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        GameEvents.OfflineProgressApplied += HandleOfflineProgressApplied;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        Hide();
    }

    private void OnDisable()
    {
        GameEvents.OfflineProgressApplied -= HandleOfflineProgressApplied;

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
        }
    }

    private void HandleOfflineProgressApplied(OfflineProgressResult result)
    {
        if (logPopupEvents)
        {
            Debug.Log("OfflineProgressPopupUI received OfflineProgressApplied event.");
        }

        if (result == null || !result.HasOfflineTime)
        {
            return;
        }

        Show(result);
    }

    private void Show(OfflineProgressResult result)
    {
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 1f;
            popupCanvasGroup.interactable = true;
            popupCanvasGroup.blocksRaycasts = true;
        }

        if (titleText != null)
        {
            titleText.text = "Offline Gains";
        }

        if (bodyText != null)
        {
            string materialLine = "";

            if (result.materialItem != null && result.materialAmount > 0)
            {
                materialLine = $"\n{result.materialItem.displayName}: {result.materialAmount}";
            }

            string durationText = FormatDuration(result.simulatedSecondsUsed);

            string rewardText;

            if (result.HasRewards)
            {
                rewardText =
                    $"Enemies defeated: {result.enemiesDefeated}\n" +
                    $"XP gained: {result.xpGained}\n" +
                    $"Coins gained: {result.coinsGained}" +
                    materialLine;
            }
            else
            {
                rewardText =
                    "Not enough time passed to defeat a full enemy yet.\n" +
                    "Stay in the combat field longer to earn offline rewards.";
            }

            bodyText.text =
                $"You were away for {durationText}.\n\n" +
                rewardText;
        }

        transform.SetAsLastSibling();

        if (logPopupEvents)
        {
            Debug.Log("Offline progress popup shown.");
        }
    }

    private string FormatDuration(double totalSecondsDouble)
    {
        int totalSeconds = Mathf.Max(1, Mathf.CeilToInt((float)totalSecondsDouble));

        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

        if (time.TotalDays >= 1)
        {
            int days = Mathf.FloorToInt((float)time.TotalDays);
            return $"{days}d {time.Hours}h {time.Minutes}m {time.Seconds}s";
        }

        if (time.TotalHours >= 1)
        {
            int hours = Mathf.FloorToInt((float)time.TotalHours);
            return $"{hours}h {time.Minutes}m {time.Seconds}s";
        }

        if (time.TotalMinutes >= 1)
        {
            int minutes = Mathf.FloorToInt((float)time.TotalMinutes);
            return $"{minutes}m {time.Seconds}s";
        }

        return $"{time.Seconds}s";
    }

    private void Hide()
    {
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0f;
            popupCanvasGroup.interactable = false;
            popupCanvasGroup.blocksRaycasts = false;
        }
    }
}