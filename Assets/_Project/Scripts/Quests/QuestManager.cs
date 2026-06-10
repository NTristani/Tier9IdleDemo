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
        if (activeQuest == null || isQuestComplete)
        {
            return;
        }

        if (enemyId != activeQuest.targetEnemyId)
        {
            return;
        }

        currentKills++;

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