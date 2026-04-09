using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    [Header("Settings")]
    public float quitDelay = 0.1f; // Small delay for visual feedback

    [Header("Visual Feedback")]
    public Color pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private Button button;
    private Color originalColor;
    private Image buttonImage;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }

        if (button != null)
        {
            // Add listener via code
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnQuitClicked);

            Debug.Log("QuitButton initialized on: " + gameObject.name);
        }
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit button clicked! Starting quit process...");

        // Visual feedback
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }

        // Disable button to prevent double-click
        if (button != null)
        {
            button.interactable = false;
        }

        // Quit after a small delay
        Invoke("ActuallyQuit", quitDelay);
    }

    void ActuallyQuit()
    {
        Debug.Log("Quitting application...");

#if UNITY_EDITOR
        Debug.Log("In Unity Editor - stopping Play Mode");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // Also keep the Inspector OnClick event for redundancy
    public void QuitGame()
    {
        OnQuitClicked();
    }
}