using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static WorldGenerator;

public class ObjectBehavior : MonoBehaviour
{
    public bool isActive = true;
    public float reactivationTime = 15.0f;
    public Renderer objectRenderer;
    public Color baseColor = Color.white;

    private float timeSinceDeactivation;
    private Subject1 subject1;

    public float value1;
    public float value2;
    public ObjectType type;

    private void Start()
    {
        // Add a NavMeshObstacle component to the object
        NavMeshObstacle obstacle = gameObject.AddComponent<NavMeshObstacle>();

        // Set the NavMeshObstacle properties
        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.size = GetComponent<BoxCollider>().size + new Vector3(0.1f, 0.1f, 0.1f);
        obstacle.center = GetComponent<BoxCollider>().center;
        obstacle.carving = true;

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer component found on this GameObject. Please add one.");
            return;
        }

        SetActiveColor();
    }

    public void PerformAction(Subject1 subject)
    {

        switch (type)
        {
            case ObjectType.Resource:

                break;
            case ObjectType.Food:
                subject.Eat(value1);
                break;
            case ObjectType.RestingPlace:
                subject.Rest(value1);
                break;
        }

        isActive = false;
        SetInactiveColor();
        timeSinceDeactivation = 0;
        StartCoroutine(ReactivateAfterTime(reactivationTime));
    }

    private IEnumerator ReactivateAfterTime(float time)
    {
        while (timeSinceDeactivation < reactivationTime)
        {
            timeSinceDeactivation += Time.deltaTime;
            float lerpValue = timeSinceDeactivation / reactivationTime;
            objectRenderer.material.color = Color.Lerp(GetInactiveColor(), GetActiveColor(), lerpValue);
            yield return null;
        }

        isActive = true;
        SetActiveColor();
    }

    private void SetActiveColor()
    {
        objectRenderer.material.color = GetActiveColor();
    }

    private void SetInactiveColor()
    {
        objectRenderer.material.color = GetInactiveColor();
    }

    private Color GetActiveColor()
    {
        return baseColor;
    }

    private Color GetInactiveColor()
    {
        // Compute the inactive color by inverting the base color
        return new Color(1 - baseColor.r, 1 - baseColor.g, 1 - baseColor.b);
    }
}