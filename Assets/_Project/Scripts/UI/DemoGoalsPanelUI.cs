using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoGoalsPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private Button toggleButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    [Header("Settings")]
    [SerializeField] private bool showOnStart = true;

    private bool isVisible;

    private void Awake()
    {
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(Toggle);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    private void OnDisable()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(Toggle);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
        }
    }

    private void Start()
    {
        if (titleText != null)
        {
            titleText.text = "Demo Goals";
        }

        if (bodyText != null)
        {
            bodyText.text =
                "Suggested test flow:\n\n" +
                "1. Talk to the Field Guide in town.\n" +
                "2. Travel to the monster field.\n" +
                "3. Watch enemies move, take damage, and drop loot.\n" +
                "4. Complete the first quest.\n" +
                "5. Return to town and claim the reward.\n" +
                "6. Spend Coins and Green Essence on upgrades.\n" +
                "7. Save, load, and test offline combat gains.\n\n" +
                "Goal: show an early-game idle RPG loop in under 30 minutes.";
        }

        if (showOnStart)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Toggle()
    {
        if (isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        isVisible = true;

        if (panelCanvasGroup == null)
        {
            return;
        }

        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        transform.SetAsLastSibling();
    }

    private void Hide()
    {
        isVisible = false;

        if (panelCanvasGroup == null)
        {
            return;
        }

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
    }
}