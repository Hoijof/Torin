using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubjectUI : MonoBehaviour
{
    public Subject1 subject1;
    public float yOffset = 2.0f;

    private Canvas canvas;
    private Image hungerBar;
    private Image energyBar;

    private RectTransform hungerBarRectTransform;
    private RectTransform energyBarRectTransform;

    private void Start()
    {
        CreateCanvas();
        CreateHungerBar();
        CreateEnergyBar();

        hungerBarRectTransform = hungerBar.GetComponent<RectTransform>();
        energyBarRectTransform = energyBar.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (subject1 != null)
        {
            UpdateUIPosition();
            UpdateUIValues();
            FaceCamera();
        }
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject("SubjectCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.transform.localPosition = new Vector3(0, 0, 0);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2.5f, 1);
    }

    private void CreateHungerBar()
    {
        GameObject hungerBarObject = new GameObject("HungerBar");
        hungerBarObject.transform.SetParent(canvas.transform);
        hungerBar = hungerBarObject.AddComponent<Image>();
        hungerBar.color = Color.green;
        RectTransform hungerBarRect = hungerBarObject.GetComponent<RectTransform>();
        hungerBarRect.anchorMin = new Vector2(0, 0.5f);
        hungerBarRect.anchorMax = new Vector2(0, 0.5f);
        hungerBarRect.pivot = new Vector2(0, 0.5f);
        hungerBarRect.sizeDelta = new Vector2(5, 0.5f);
        hungerBarRect.localPosition = new Vector3(0, 0, 0);
    }

    private void CreateEnergyBar()
    {
        GameObject energyBarObject = new GameObject("EnergyBar");
        energyBarObject.transform.SetParent(canvas.transform);
        energyBar = energyBarObject.AddComponent<Image>();
        energyBar.color = Color.blue;
        RectTransform energyBarRect = energyBarObject.GetComponent<RectTransform>();
        energyBarRect.anchorMin = new Vector2(0, 0.5f);
        energyBarRect.anchorMax = new Vector2(0, 0.5f);
        energyBarRect.pivot = new Vector2(0, 0.5f);
        energyBarRect.sizeDelta = new Vector2(5, 0.5f);
        energyBarRect.localPosition = new Vector3(0, 0.5f, 0);
    }

    private void UpdateUIPosition()
    {
        Vector3 uiPosition = subject1.transform.position;
        uiPosition.y += yOffset;
        transform.position = uiPosition;
    }

    private void UpdateUIValues()
    {
        float hungerPercent = subject1.CurrentHunger / subject1.MaxHunger;
        float energyPercent = subject1.CurrentEnergy / subject1.MaxEnergy;

        hungerBarRectTransform.localScale = new Vector3(hungerPercent, 1, 1);
        energyBarRectTransform.localScale = new Vector3(energyPercent, 1, 1);

        hungerBar.color = GetColorFromPercent(hungerPercent, "health");
        energyBar.color = GetColorFromPercent(energyPercent, "energy");
    }

    private void FaceCamera()
    {
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private Color GetColorFromPercent(float percent, string barType)
    {
        Dictionary<string, Color[]> colorRanges = new Dictionary<string, Color[]>()
        {
            {"health", new Color[] {Color.red, Color.yellow, Color.green}},
            {"energy", new Color[] {Color.blue, Color.cyan, Color.white}}
        };

        int index = Mathf.FloorToInt((colorRanges[barType].Length - 1) * percent);
        return colorRanges[barType][index];
    }
}