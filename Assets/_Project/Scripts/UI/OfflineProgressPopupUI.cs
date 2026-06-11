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

        if (result == null || !result.HasRewards)
        {
            if (logPopupEvents)
            {
                Debug.Log("Offline progress result had no rewards, so popup was not shown.");
            }

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

            double minutes = result.simulatedSecondsUsed / 60.0;

            bodyText.text =
                $"You were away for {minutes:0.0} simulated minutes.\n\n" +
                $"Enemies defeated: {result.enemiesDefeated}\n" +
                $"XP gained: {result.xpGained}\n" +
                $"Coins gained: {result.coinsGained}" +
                materialLine;
        }

        if (logPopupEvents)
        {
            Debug.Log("Offline progress popup shown.");
        }
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