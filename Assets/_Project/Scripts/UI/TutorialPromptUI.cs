using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPromptUI : MonoBehaviour
{
    private const string TutorialDismissedKey = "IdleAdventurer_TutorialDismissed";

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button dismissButton;
    [SerializeField] private TMP_Text tutorialText;

    [Header("Settings")]
    [SerializeField] private bool rememberDismissedState = false;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(Dismiss);
        }
    }

    private void OnDisable()
    {
        if (dismissButton != null)
        {
            dismissButton.onClick.RemoveListener(Dismiss);
        }
    }

    private void Start()
    {
        if (tutorialText != null)
        {
            tutorialText.text =
                "Welcome! Talk to the Field Guide, travel to the monster field, defeat enemies, " +
                "collect Green Essence, buy upgrades, and test offline gains.";
        }

        if (rememberDismissedState && PlayerPrefs.GetInt(TutorialDismissedKey, 0) == 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Dismiss()
    {
        if (rememberDismissedState)
        {
            PlayerPrefs.SetInt(TutorialDismissedKey, 1);
            PlayerPrefs.Save();
        }

        Hide();
    }

    private void Show()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}