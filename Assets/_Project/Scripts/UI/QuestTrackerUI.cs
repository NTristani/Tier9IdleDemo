using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text questTitleText;
    [SerializeField] private TMP_Text questObjectiveText;
    [SerializeField] private TMP_Text questStatusText;

    [Header("Button")]
    [SerializeField] private Button claimRewardButton;

    [Header("References")]
    [SerializeField] private QuestManager questManager;

    private void OnEnable()
    {
        GameEvents.QuestProgressChanged += HandleQuestProgressChanged;

        if (claimRewardButton != null)
        {
            claimRewardButton.onClick.AddListener(HandleClaimRewardClicked);
        }
    }

    private void OnDisable()
    {
        GameEvents.QuestProgressChanged -= HandleQuestProgressChanged;

        if (claimRewardButton != null)
        {
            claimRewardButton.onClick.RemoveListener(HandleClaimRewardClicked);
        }
    }

    private void HandleQuestProgressChanged(
        QuestDefinition quest,
        int currentKills,
        bool isComplete,
        bool rewardClaimed)
    {
        if (quest == null)
        {
            SetNoQuestState();
            return;
        }

        if (questTitleText != null)
        {
            questTitleText.text = quest.displayName;
        }

        if (questObjectiveText != null)
        {
            questObjectiveText.text =
                //$"Defeat {quest.targetDisplayName}: {currentKills} / {quest.requiredKills}";
                $"Status: {currentKills} / {quest.requiredKills}";
        }

        if (questStatusText != null)
        {
            if (rewardClaimed)
            {
                questStatusText.text = "Reward claimed";
            }
            else if (isComplete)
            {
                questStatusText.text = $"Complete! Reward: {quest.coinReward} Coins, {quest.xpReward} XP";
            }
            else
            {
                questStatusText.text = quest.description;
            }
        }

        if (claimRewardButton != null)
        {
            claimRewardButton.gameObject.SetActive(isComplete && !rewardClaimed);
        }
    }

    private void SetNoQuestState()
    {
        if (questTitleText != null)
        {
            questTitleText.text = "No Active Quest";
        }

        if (questObjectiveText != null)
        {
            questObjectiveText.text = "";
        }

        if (questStatusText != null)
        {
            questStatusText.text = "";
        }

        if (claimRewardButton != null)
        {
            claimRewardButton.gameObject.SetActive(false);
        }
    }

    private void HandleClaimRewardClicked()
    {
        if (questManager == null)
        {
            Debug.LogWarning("QuestTrackerUI has no QuestManager assigned.");
            return;
        }

        questManager.ClaimReward();
    }
}