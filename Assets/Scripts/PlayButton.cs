using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [Header("Settings")]
    public string sceneName = ""; // Leave empty to use scene index
    public int sceneIndex = 1;
    public bool useSceneIndex = true;

    [Header("Debug")]
    public bool debugMode = true;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            // Add listener via code (more reliable than Inspector)
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnPlayClicked);

            if (debugMode)
            {
                Debug.Log("PlayButton initialized on: " + gameObject.name);
                CheckIfSceneExists();
            }
        }
    }

    void CheckIfSceneExists()
    {
        if (useSceneIndex)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            if (sceneIndex >= 0 && sceneIndex < sceneCount)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
                string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"Scene {sceneIndex} exists: '{name}'");
            }
            else
            {
                Debug.LogError($"Scene index {sceneIndex} is out of range! Build has {sceneCount} scenes.");
            }
        }
        else if (!string.IsNullOrEmpty(sceneName))
        {
            // Check if scene exists by name
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (name == sceneName)
                {
                    sceneExists = true;
                    Debug.Log($"Scene '{sceneName}' found at index {i}");
                    break;
                }
            }

            if (!sceneExists)
            {
                Debug.LogError($"Scene '{sceneName}' not found in Build Settings!");
            }
        }
    }

    public void OnPlayClicked()
    {
        Debug.Log("Play button clicked!");

        if (useSceneIndex)
        {
            LoadByIndex();
        }
        else
        {
            LoadByName();
        }
    }

    void LoadByIndex()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneIndex < 0 || sceneIndex >= sceneCount)
        {
            Debug.LogError($"Cannot load scene index {sceneIndex}. Build has only {sceneCount} scenes.");
            Debug.Log("Available scenes:");
            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"  [{i}] {name}");
            }
            return;
        }

        Debug.Log($"Loading scene at index {sceneIndex}...");
        SceneManager.LoadScene(sceneIndex);
    }

    void LoadByName()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty! Set sceneName or use sceneIndex instead.");
            return;
        }

        Debug.Log($"Loading scene: {sceneName}");

        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load scene '{sceneName}': {e.Message}");
            Debug.Log("Available scenes:");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"  [{i}] {name}");
            }
        }
    }

    // Also keep the Inspector OnClick event for redundancy
    public void LoadGameScene()
    {
        OnPlayClicked();
    }
}