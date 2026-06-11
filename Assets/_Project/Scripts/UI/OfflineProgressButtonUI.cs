using UnityEngine;
using UnityEngine.UI;

public class OfflineProgressButtonUI : MonoBehaviour
{
    [SerializeField] private OfflineProgressManager offlineProgressManager;
    [SerializeField] private Button simulateButton;

    private void OnEnable()
    {
        if (simulateButton != null)
        {
            simulateButton.onClick.AddListener(HandleSimulateClicked);
        }
    }

    private void OnDisable()
    {
        if (simulateButton != null)
        {
            simulateButton.onClick.RemoveListener(HandleSimulateClicked);
        }
    }

    private void HandleSimulateClicked()
    {
        if (offlineProgressManager == null)
        {
            Debug.LogWarning("OfflineProgressButtonUI is missing OfflineProgressManager.");
            return;
        }

        offlineProgressManager.SimulateDemoOfflineProgress();
    }
}