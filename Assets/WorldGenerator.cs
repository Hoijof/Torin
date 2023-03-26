using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{
    public enum ObjectType
    {
        Resource,
        Food,
        RestingPlace
    }

    [System.Serializable]
    public class ObjectProperties
    {
        public string name;
        public GameObject prefab;

        public ObjectType type;

        public float value1;
        public float value2;

        public float reactivationTime = 15.0f;

        public Color baseColor = Color.white;
    }

    [Header("Object Settings")]
    public List<ObjectProperties> objectsToSpawn;
    public int numberOfObjects = 10;
    public float spawnRadius = 10.0f;

    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;

    [Header("Object Prefabs")]
    public GameObject BasePrefab;
    public GameObject FoodPrefab;
    public GameObject CampfirePrefab;
    public GameObject WoodPrefab;
    public GameObject StonePrefab;
    public GameObject MetalPrefab;

    private void Start()
    {
        objectsToSpawn = GenerateObjectList();

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            ObjectProperties objectProperties = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
            GameObject objectToSpawn = objectProperties.prefab ? objectProperties.prefab : BasePrefab;

            Vector3 spawnPosition = GetRandomSpawnPosition();
            Quaternion spawnRotation = Quaternion.identity;

            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            spawnedObject.name = objectProperties.name;
            spawnedObject.tag = "Object";

            ObjectBehavior objectBehavior = spawnedObject.AddComponent<ObjectBehavior>();

            objectBehavior.type = objectProperties.type;
            objectBehavior.name = objectProperties.name;
            objectBehavior.value1 = objectProperties.value1;
            objectBehavior.value2 = objectProperties.value2;
            objectBehavior.baseColor = objectProperties.baseColor;
            objectBehavior.reactivationTime = objectProperties.reactivationTime;

            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(spawnPosition, out navMeshHit, 1.0f, NavMesh.AllAreas))
            {
                spawnedObject.transform.position = navMeshHit.position;
            }
            else
            {
                Debug.LogError($"Failed to place object {objectToSpawn.name} on NavMesh surface!");
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, spawnRadius, NavMesh.AllAreas);
        return navHit.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    private List<ObjectProperties> GenerateObjectList()
    {
        List<ObjectProperties> objectList = new List<ObjectProperties>();

        // Add food objects
        objectList.Add(new ObjectProperties
        {
            name = "Apple",
            prefab = FoodPrefab,
            type = ObjectType.Food,
            value1 = 20.0f, // Saciation value
            value2 = 5.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.red
        });
        objectList.Add(new ObjectProperties
        {
            name = "Banana",
            prefab = FoodPrefab,
            type = ObjectType.Food,
            value1 = 30.0f, // Saciation value
            value2 = 7.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.yellow
        });
        objectList.Add(new ObjectProperties
        {
            name = "Meat",
            prefab = FoodPrefab,
            type = ObjectType.Food,
            value1 = 50.0f, // Saciation value
            value2 = 10.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.cyan
        });

        // Add resource objects
        objectList.Add(new ObjectProperties
        {
            name = "Wood",
            prefab = WoodPrefab,
            type = ObjectType.Resource,
            value1 = 10.0f, // Resource value
            value2 = 0.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.gray
        });
        objectList.Add(new ObjectProperties
        {
            name = "Stone",
            prefab = StonePrefab,
            type = ObjectType.Resource,
            value1 = 20.0f, // Resource value
            value2 = 0.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.gray
        });
        objectList.Add(new ObjectProperties
        {
            name = "Metal",
            prefab = MetalPrefab,
            type = ObjectType.Resource,
            value1 = 30.0f, // Resource value
            value2 = 0.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.gray
        });

        // Add resting place object
        objectList.Add(new ObjectProperties
        {
            name = "Campfire",
            prefab = CampfirePrefab,
            type = ObjectType.RestingPlace,
            value1 = 50.0f, // Energy recovered
            value2 = 10.0f, // Cooldown time in seconds
            reactivationTime = 60f,
            baseColor = Color.magenta
        });

        return objectList;
    }
}

