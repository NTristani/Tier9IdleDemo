using UnityEngine;
using UnityEngine.UI;

public class PortalTravelButtonUI : MonoBehaviour
{
    [SerializeField] private WorldZoneManager worldZoneManager;
    [SerializeField] private Button button;
    [SerializeField] private GameZone targetZone = GameZone.CombatField;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleClicked);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClicked);
        }
    }

    private void HandleClicked()
    {
        if (worldZoneManager == null)
        {
            Debug.LogWarning("PortalTravelButtonUI is missing a WorldZoneManager reference.");
            return;
        }

        switch (targetZone)
        {
            case GameZone.Town:
                worldZoneManager.GoToTown();
                break;

            case GameZone.CombatField:
                worldZoneManager.GoToCombatField();
                break;
        }
    }
}