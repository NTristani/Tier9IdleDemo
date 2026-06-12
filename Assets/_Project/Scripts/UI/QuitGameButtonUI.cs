using UnityEngine;
using UnityEngine.UI;

public class QuitGameButtonUI : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (quitButton == null)
        {
            quitButton = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void OnDisable()
    {
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit button pressed. Application.Quit only closes built players.");
#else
        Application.Quit();
#endif
    }
}