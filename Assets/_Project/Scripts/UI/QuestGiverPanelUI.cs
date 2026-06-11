using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private WorldZoneManager worldZoneManager;

    [Header("Panel")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text questProgressText;

    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button claimRewardButton;
    [SerializeField] private Button goToFieldButton;

    private QuestDefinition currentQuest;
    private int currentKills;
    private bool isQuestComplete;
    private bool rewardClaimed;

    private void Awake()
    {
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        GameEvents.QuestProgressChanged += HandleQuestProgressChanged;
        GameEvents.ZoneChanged += HandleZoneChanged;

        if (openButton != null)
        {
            openButton.onClick.AddListener(Show);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        if (claimRewardButton != null)
        {
            claimRewardButton.onClick.AddListener(HandleClaimRewardClicked);
        }

        if (goToFieldButton != null)
        {
            goToFieldButton.onClick.AddListener(HandleGoToFieldClicked);
        }

        Hide();
        Refresh();
    }

    private void OnDisable()
    {
        GameEvents.QuestProgressChanged -= HandleQuestProgressChanged;

        if (openButton != null)
        {
            openButton.onClick.RemoveListener(Show);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
        }

        if (claimRewardButton != null)
        {
            claimRewardButton.onClick.RemoveListener(HandleClaimRewardClicked);
        }

        if (goToFieldButton != null)
        {
            goToFieldButton.onClick.RemoveListener(HandleGoToFieldClicked);
        }
    }

    private void HandleZoneChanged(GameZone newZone)
    {
        if (newZone != GameZone.Town)
        {
            Hide();
        }
    }

    private void HandleQuestProgressChanged(
        QuestDefinition quest,
        int kills,
        bool complete,
        bool claimed)
    {
        currentQuest = quest;
        currentKills = kills;
        isQuestComplete = complete;
        rewardClaimed = claimed;

        Refresh();
    }

    private void Refresh()
    {
        if (npcNameText != null)
        {
            npcNameText.text = "Field Guide";
        }

        if (currentQuest == null)
        {
            if (dialogueText != null)
            {
                dialogueText.text = "I don't have anything for you right now.";
            }

            if (questProgressText != null)
            {
                questProgressText.text = "";
            }

            SetButtonStates(false, false);
            return;
        }

        if (dialogueText != null)
        {
            if (rewardClaimed)
            {
                dialogueText.text = "Nice work out there. Spend your loot and keep training!";
            }
            else if (isQuestComplete)
            {
                dialogueText.text = "Great job! You cleared enough enemies. Come claim your reward.";
            }
            else
            {
                dialogueText.text = "The field is crawling with monsters. Defeat a few and bring back proof.";
            }
        }

        if (questProgressText != null)
        {
            questProgressText.text =
                $"{currentQuest.displayName}\n" +
                $"Defeat {currentQuest.targetDisplayName}: {currentKills} / {currentQuest.requiredKills}\n" +
                $"Reward: {currentQuest.coinReward} Coins, {currentQuest.xpReward} XP";
        }

        SetButtonStates(isQuestComplete && !rewardClaimed, !isQuestComplete);
    }

    private void SetButtonStates(bool canClaim, bool canGoToField)
    {
        if (claimRewardButton != null)
        {
            claimRewardButton.gameObject.SetActive(canClaim);
        }

        if (goToFieldButton != null)
        {
            goToFieldButton.gameObject.SetActive(canGoToField);
        }
    }

    private void HandleClaimRewardClicked()
    {
        if (questManager == null)
        {
            Debug.LogWarning("QuestGiverPanelUI is missing QuestManager.");
            return;
        }

        questManager.ClaimReward();
        Refresh();
    }

    private void HandleGoToFieldClicked()
    {
        if (worldZoneManager != null)
        {
            worldZoneManager.GoToCombatField();
            Hide();
        }
    }

    private void Show()
    {
        if (panelCanvasGroup == null)
        {
            return;
        }

        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        Refresh();
    }

    private void Hide()
    {
        if (panelCanvasGroup == null)
        {
            return;
        }

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
    }
}