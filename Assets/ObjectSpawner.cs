using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectSpawner : MonoBehaviour
{
    public int numberOfBoxes = 10;
    public GameObject boxPrefab;
    public float spawnRadius = 40.0f;
    public LayerMask navMeshLayer;

    private NavMeshSurface navMeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        // Find the NavMeshSurface component in the scene
        navMeshSurface = FindObjectOfType<NavMeshSurface>();

        if (boxPrefab == null)
        {
            Debug.LogError("No box prefab assigned to the ObjectSpawner script.");
            return;
        }

        for (int i = 0; i < numberOfBoxes; i++)
        {
            SpawnBox();
        }


        // Update the NavMesh
        navMeshSurface.BuildNavMesh();
    }

    private void SpawnBox()
    {
        Vector3 randomPosition = GetRandomPointOnNavMesh();

        if (randomPosition != Vector3.zero)
        {
            GameObject spawnedBox = Instantiate(boxPrefab, randomPosition, Quaternion.identity);
            spawnedBox.AddComponent<ObjectBehavior>();
            spawnedBox.tag = "Object";
            spawnedBox.transform.parent = transform;
        }
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection += transform.position;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, spawnRadius, navMeshLayer))
            {
                return navHit.position;
            }
        }

        Debug.LogWarning("Could not find a valid NavMesh position. Increase the spawnRadius or adjust the NavMesh.");
        return Vector3.zero;
    }
}