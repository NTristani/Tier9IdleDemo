using UnityEngine;

[CreateAssetMenu(
    fileName = "ItemDefinition",
    menuName = "Idle Adventurer/Economy/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    public string itemId = "green_essence";
    public string displayName = "Green Essence";

    [Header("Visuals")]
    public Sprite icon;
}