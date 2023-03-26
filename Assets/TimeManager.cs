using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float realTimePerGameDay = 5.0f * 60; // 5 minutes in real life = 1 game day
    public int hoursPerDay = 24;
    public int daysPerWeek = 5;
    public int weeksPerMonth = 2;
    public int monthsPerYear = 4;

    [Header("UI Settings")]
    public bool showUI = true;
    public Vector2 uiPosition = new Vector2(0.85f, 0.95f);
    public float uiPadding = 10.0f;
    public TMP_Text uiTextPrefab;

    private int currentYear = 850;
    private int currentMonth = 1;
    private int currentWeek = 1;
    private int currentDay = 1;
    private float currentHour = 0.0f;

    private TMP_Text uiText;

    private void Start()
    {
        if (showUI)
        {
            CreateUI();
        }
    }

    private void Update()
    {
        // Advance time based on real time
        currentHour += Time.deltaTime / (realTimePerGameDay / hoursPerDay);

        // If a day has passed, update the date
        if (currentHour >= hoursPerDay)
        {
            currentHour = 0.0f;
            currentDay++;

            // If a week has passed, update the week
            if (currentDay > daysPerWeek)
            {
                currentDay = 1;
                currentWeek++;

                // If a month has passed, update the month
                if (currentWeek > weeksPerMonth)
                {
                    currentWeek = 1;
                    currentMonth++;

                    // If a year has passed, update the year
                    if (currentMonth > monthsPerYear)
                    {
                        currentMonth = 1;
                        currentYear++;
                    }
                }
            }
        }

        if (showUI)
        {
            UpdateUI();
        }
    }

    private void CreateUI()
    {
        // Create a new canvas
        GameObject canvasGO = new GameObject("TimeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create a new TextMeshProUGUI instance
        GameObject textGO = new GameObject("TimeText");
        textGO.transform.SetParent(canvasGO.transform);
        uiText = textGO.AddComponent<TextMeshProUGUI>();
        uiText.fontSize = 24;
        uiText.fontStyle = FontStyles.Bold;
        uiText.color = Color.white;

        // Set the position of the TextMeshProUGUI instance
        uiText.rectTransform.anchorMin = uiPosition;
        uiText.rectTransform.anchorMax = uiPosition;
        uiText.rectTransform.pivot = uiPosition;
        uiText.rectTransform.anchoredPosition = new Vector2(uiPadding, -uiPadding);
    }

    private void UpdateUI()
    {
        uiText.rectTransform.anchorMin = uiPosition;
        uiText.rectTransform.anchorMax = uiPosition;
        uiText.rectTransform.pivot = uiPosition;
        uiText.rectTransform.anchoredPosition = new Vector2(uiPadding, -uiPadding);

        uiText.text = $"{currentHour:00}:{Mathf.Floor((currentHour % 1.0f) * 60):00} {currentDay:00}/{currentMonth:00}/{currentYear:000}";
    }
}