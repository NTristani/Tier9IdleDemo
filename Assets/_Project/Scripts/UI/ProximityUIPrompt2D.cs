using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProximityUIPrompt2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("UI")]
    [SerializeField] private CanvasGroup promptCanvasGroup;

    [Header("Optional")]
    [SerializeField] private CanvasGroup panelToHideOnExit;
    [SerializeField] private bool hidePanelOnExit = true;

    private int playerOverlapCount;

    private void Awake()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        HidePrompt();

        if (hidePanelOnExit && panelToHideOnExit != null)
        {
            HideCanvasGroup(panelToHideOnExit);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerOverlapCount++;
        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerOverlapCount = Mathf.Max(0, playerOverlapCount - 1);

        if (playerOverlapCount <= 0)
        {
            HidePrompt();

            if (hidePanelOnExit && panelToHideOnExit != null)
            {
                HideCanvasGroup(panelToHideOnExit);
            }
        }
    }

    private void OnDisable()
    {
        playerOverlapCount = 0;
        HidePrompt();

        if (hidePanelOnExit && panelToHideOnExit != null)
        {
            HideCanvasGroup(panelToHideOnExit);
        }
    }

    private void ShowPrompt()
    {
        if (promptCanvasGroup == null)
        {
            return;
        }

        promptCanvasGroup.alpha = 1f;
        promptCanvasGroup.interactable = true;
        promptCanvasGroup.blocksRaycasts = true;
    }

    private void HidePrompt()
    {
        if (promptCanvasGroup == null)
        {
            return;
        }

        HideCanvasGroup(promptCanvasGroup);
    }

    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}