using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition activeQuest;

    [Header("References")]
    [SerializeField] private CurrencyWallet currencyWallet;
    [SerializeField] private PlayerStats playerStats;

    [Header("Runtime")]
    [SerializeField] private int currentKills;
    [SerializeField] private bool isQuestComplete;
    [SerializeField] private bool rewardClaimed;

    public QuestDefinition ActiveQuest => activeQuest;
    public int CurrentKills => currentKills;
    public bool IsQuestComplete => isQuestComplete;
    public bool RewardClaimed => rewardClaimed;

    private void OnEnable()
    {
        GameEvents.EnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= HandleEnemyKilled;
    }

    private void Start()
    {
        BroadcastQuestProgress();
    }

    private void HandleEnemyKilled(string enemyId, int xpReward, int coinReward)
    {
        AddEnemyKills(enemyId, 1);
    }

    public void AddEnemyKills(string enemyId, int killAmount)
    {
        if (activeQuest == null || isQuestComplete)
        {
            return;
        }

        if (enemyId != activeQuest.targetEnemyId)
        {
            return;
        }

        if (killAmount <= 0)
        {
            return;
        }

        currentKills += killAmount;

        if (currentKills >= activeQuest.requiredKills)
        {
            currentKills = activeQuest.requiredKills;
            isQuestComplete = true;
        }

        BroadcastQuestProgress();
    }

    public void ClaimReward()
    {
        if (activeQuest == null)
        {
            Debug.LogWarning("No active quest assigned.");
            return;
        }

        if (!isQuestComplete)
        {
            Debug.Log("Quest is not complete yet.");
            return;
        }

        if (rewardClaimed)
        {
            Debug.Log("Quest reward already claimed.");
            return;
        }

        rewardClaimed = true;

        if (currencyWallet != null)
        {
            currencyWallet.AddCoins(activeQuest.coinReward);
        }

        if (playerStats != null)
        {
            playerStats.AddExperience(activeQuest.xpReward);
        }

        BroadcastQuestProgress();
    }

    public void LoadProgress(int savedCurrentKills, bool savedIsComplete, bool savedRewardClaimed)
    {
        currentKills = Mathf.Max(0, savedCurrentKills);

        if (activeQuest != null)
        {
            currentKills = Mathf.Min(currentKills, activeQuest.requiredKills);
        }

        isQuestComplete = savedIsComplete;
        rewardClaimed = savedRewardClaimed;

        if (activeQuest != null && currentKills >= activeQuest.requiredKills)
        {
            isQuestComplete = true;
        }

        BroadcastQuestProgress();
    }

    public void ResetQuestProgress()
    {
        currentKills = 0;
        isQuestComplete = false;
        rewardClaimed = false;
        BroadcastQuestProgress();
    }

    private void BroadcastQuestProgress()
    {
        GameEvents.RaiseQuestProgressChanged(
            activeQuest,
            currentKills,
            isQuestComplete,
            rewardClaimed
        );
    }
}