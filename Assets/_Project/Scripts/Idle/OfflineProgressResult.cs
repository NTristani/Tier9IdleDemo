using System;
using UnityEngine;

[Serializable]
public class OfflineProgressResult
{
    public double realSecondsAway;
    public double simulatedSecondsUsed;

    public int enemiesDefeated;
    public int xpGained;
    public int coinsGained;

    public ItemDefinition materialItem;
    public int materialAmount;

    public bool HasOfflineTime => simulatedSecondsUsed >= 1;

    public bool HasRewards =>
        enemiesDefeated > 0 ||
        xpGained > 0 ||
        coinsGained > 0 ||
        materialAmount > 0;
}