using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveManager saveManager;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button clearButton;

    [Header("Text")]
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(HandleSaveClicked);
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(HandleLoadClicked);
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(HandleClearClicked);
        }

        SetStatus("Save system ready");
    }

    private void OnDisable()
    {
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(HandleSaveClicked);
        }

        if (loadButton != null)
        {
            loadButton.onClick.RemoveListener(HandleLoadClicked);
        }

        if (clearButton != null)
        {
            clearButton.onClick.RemoveListener(HandleClearClicked);
        }
    }

    private void HandleSaveClicked()
    {
        if (saveManager == null)
        {
            SetStatus("Missing SaveManager");
            return;
        }

        saveManager.SaveGame();
        SetStatus("Game saved");
    }

    private void HandleLoadClicked()
    {
        if (saveManager == null)
        {
            SetStatus("Missing SaveManager");
            return;
        }

        if (!saveManager.HasSave())
        {
            SetStatus("No save found");
            return;
        }

        saveManager.LoadGame();
        SetStatus("Game loaded");
    }

    private void HandleClearClicked()
    {
        if (saveManager == null)
        {
            SetStatus("Missing SaveManager");
            return;
        }

        saveManager.ClearSave();
        SetStatus("Save cleared");
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}