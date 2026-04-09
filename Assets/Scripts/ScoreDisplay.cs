using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Score Settings")]
    public int baseScore = 10;
    public float maxMultiplier = 3f;
    public float minMultiplier = 1f;
    public float maxDistance = 10f;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public TMP_Text streakText;

    [Header("Display Settings")]
    public Vector2 scorePosition = new Vector2(20, -20);
    public Vector2 multiplierPosition = new Vector2(20, -50);
    public Vector2 streakPosition = new Vector2(20, -80);
    public int scoreFontSize = 36;
    public int multiplierFontSize = 24;
    public int streakFontSize = 20;

    private int totalScore = 0;
    private float currentMultiplier = 1f;
    private int killStreak = 0;
    private float streakTimer = 0f;
    private float streakTimeout = 3f;

    void Start()
    {
        InitializeUI();
        UpdateDisplay();
    }

    void Update()
    {
        // Update streak timer
        if (killStreak > 0)
        {
            streakTimer += Time.deltaTime;
            if (streakTimer >= streakTimeout)
            {
                ResetStreak();
            }
        }
    }

    void InitializeUI()
    {
        // Create Canvas if needed
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Create Score Text if not assigned
        if (scoreText == null)
        {
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(transform);
            scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            SetupText(scoreText, scorePosition, scoreFontSize, Color.white);
        }

        // Create Multiplier Text if not assigned
        if (multiplierText == null)
        {
            GameObject multiObj = new GameObject("MultiplierText");
            multiObj.transform.SetParent(transform);
            multiplierText = multiObj.AddComponent<TextMeshProUGUI>();
            SetupText(multiplierText, multiplierPosition, multiplierFontSize, Color.yellow);
        }

        // Create Streak Text if not assigned
        if (streakText == null)
        {
            GameObject streakObj = new GameObject("StreakText");
            streakObj.transform.SetParent(transform);
            streakText = streakObj.AddComponent<TextMeshProUGUI>();
            SetupText(streakText, streakPosition, streakFontSize, Color.cyan);
        }
    }

    void SetupText(TMP_Text text, Vector2 position, int fontSize, Color color)
    {
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.TopLeft;

        RectTransform rt = text.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 50);
    }

    // Call this when an enemy is killed
    public void AddEnemyKill(Vector3 enemyPosition)
    {
        // Calculate distance to player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(enemyPosition, player.transform.position);

        // Calculate distance multiplier
        float distanceMultiplier = CalculateDistanceMultiplier(distance);

        // Update streak
        killStreak++;
        streakTimer = 0f;

        // Calculate total multiplier
        float streakBonus = 1f + (killStreak * 0.1f);
        currentMultiplier = distanceMultiplier * streakBonus;
        currentMultiplier = Mathf.Clamp(currentMultiplier, minMultiplier, maxMultiplier);

        // Calculate score
        int scoreEarned = Mathf.RoundToInt(baseScore * currentMultiplier);
        totalScore += scoreEarned;

        // Create floating text at enemy position
        CreateFloatingText(enemyPosition, scoreEarned, distanceMultiplier);

        // Update display
        UpdateDisplay();

        Debug.Log($"Score: +{scoreEarned} (Distance: {distance:F1}m, Multiplier: {currentMultiplier:F1}x)");
    }

    float CalculateDistanceMultiplier(float distance)
    {
        if (distance >= maxDistance)
            return minMultiplier;

        float normalizedDistance = distance / maxDistance;
        return Mathf.Lerp(maxMultiplier, minMultiplier, normalizedDistance);
    }

    void CreateFloatingText(Vector3 position, int score, float multiplier)
    {
        // Create floating score text in world space
        GameObject floatObj = new GameObject("FloatingScore");
        floatObj.transform.position = position + Vector3.up * 0.5f;

        TextMeshPro tmp = floatObj.AddComponent<TextMeshPro>();
        tmp.text = $"+{score}";
        tmp.fontSize = 20;
        tmp.color = multiplier >= 2f ? Color.yellow : Color.green;
        tmp.alignment = TextAlignmentOptions.Center;

        // Add fade out effect
        StartCoroutine(FadeOutText(tmp, 1f));

        Destroy(floatObj, 1.5f);
    }

    System.Collections.IEnumerator FadeOutText(TMP_Text text, float duration)
    {
        float timer = 0f;
        Color startColor = text.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            text.transform.position += Vector3.up * Time.deltaTime * 0.5f;
            yield return null;
        }
    }

    void UpdateDisplay()
    {
        // Update Score Text
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {totalScore}";
        }

        // Update Multiplier Text
        if (multiplierText != null)
        {
            multiplierText.text = $"MULTIPLIER: {currentMultiplier:F1}x";
            multiplierText.color = currentMultiplier >= 2f ? Color.yellow :
                                  currentMultiplier >= 1.5f ? new Color(1f, 0.5f, 0f) : // Orange
                                  Color.white;
        }

        // Update Streak Text
        if (streakText != null)
        {
            streakText.text = $"STREAK: {killStreak}";
            streakText.color = killStreak >= 5 ? Color.red :
                              killStreak >= 3 ? Color.yellow :
                              Color.cyan;
        }
    }

    void ResetStreak()
    {
        killStreak = 0;
        currentMultiplier = 1f;
        UpdateDisplay();
    }

    // Public methods
    public int GetTotalScore() => totalScore;
    public float GetCurrentMultiplier() => currentMultiplier;
    public int GetKillStreak() => killStreak;

    public void ResetAll()
    {
        totalScore = 0;
        ResetStreak();
    }
}