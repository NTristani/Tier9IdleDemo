using UnityEngine;
using UnityEngine.UI;

public class UIPanelToggle : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup targetPanel;
    [SerializeField] private bool showOnStart = false;

    [Header("Buttons")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private Button closeButton;

    private bool isVisible;

    private void Awake()
    {
        if (toggleButton == null)
        {
            toggleButton = GetComponent<Button>();
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
        if (showOnStart)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Toggle()
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

    public void Show()
    {
        isVisible = true;

        if (targetPanel == null)
        {
            return;
        }

        targetPanel.alpha = 1f;
        targetPanel.interactable = true;
        targetPanel.blocksRaycasts = true;

        targetPanel.transform.SetAsLastSibling();
    }

    public void Hide()
    {
        isVisible = false;

        if (targetPanel == null)
        {
            return;
        }

        targetPanel.alpha = 0f;
        targetPanel.interactable = false;
        targetPanel.blocksRaycasts = false;
    }
}