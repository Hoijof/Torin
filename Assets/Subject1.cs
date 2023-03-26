using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static WorldGenerator;

public class Subject1 : MonoBehaviour
{
    public enum State
    {
        Idle,
        Searching,
        GoingToTarget,
        PerformingAction
    }

    [Header("Agent and Searching Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private SphereCollider searchCollider;
    [SerializeField] private float searchCooldown = 1.0f;
    private float nextSearchTime;

    [Header("Target Object")]
    [SerializeField] private ObjectBehavior targetObject;

    [Header("State and Pathfinding")]
    public State currentState;
    public bool hasPath = false;
    public ObjectType desiredObjectType;

    [Header("Hunger and Energy")]
    private float maxHunger = 100.0f;
    private float maxEnergy = 100.0f;
    [SerializeField] private float hungerDecayRate = 1.0f;
    [SerializeField] private float energyDecayRate = 0.5f;

    [Header("Current Values")]
    [SerializeField] private float currentHunger;
    [SerializeField] private float currentEnergy;

    public float CurrentHunger { get => currentHunger; }
    public float CurrentEnergy { get => currentEnergy; }
    public float MaxHunger { get => maxHunger; }
    public float MaxEnergy { get => maxEnergy; }

    private Vector3 lastRandomDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        searchCollider = GetComponent<SphereCollider>();
        if (searchCollider == null)
        {
            Debug.LogError("No SphereCollider component found on this GameObject. Please add one.");
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        nextSearchTime = Time.time + searchCooldown;
        currentHunger = 51;
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        CheckStatus();
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Searching:
                Search();
                break;
            case State.GoingToTarget:
                GoToTarget();
                break;
            case State.PerformingAction:
                // coroutine handles this state
                break;
        }
        DecreaseHungerAndEnergy();
    }

    private void CheckStatus()
    {
        if (currentHunger <= 0 || currentEnergy <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Subject1 has died.");
        Destroy(gameObject);
    }

    private void Idle()
    {
        if (currentHunger <= maxHunger * 0.5f)
        {
            desiredObjectType = ObjectType.Food;
            ChangeState(State.Searching);

            return;
        }

        if (currentEnergy <= maxEnergy * 0.5f)
        {
            desiredObjectType = ObjectType.RestingPlace;
            ChangeState(State.Searching);

            return;
        }

        if (!agent.hasPath)
        {
            SetRandomDestination();
        }
    }

    private void Search()
    {
        hasPath = agent.hasPath;

        if (Time.time > nextSearchTime)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchCollider.radius);

            foreach (Collider hitCollider in hitColliders)
            {
                ObjectBehavior objectBehavior = hitCollider.GetComponent<ObjectBehavior>();
                if (objectBehavior != null && objectBehavior.isActive && objectBehavior.type == desiredObjectType)
                {
                    targetObject = objectBehavior;

                    Transform targetPosition = targetObject.transform.Find("PickupPoint");

                    if (!targetPosition) Debug.Log("No PickupPoint found on target object.");
                    targetPosition = targetPosition == null ? targetObject.transform : targetPosition;

                    agent.SetDestination(targetPosition.position);
                    ChangeState(State.GoingToTarget);
                    break;
                }
            }

            nextSearchTime = Time.time + searchCooldown;
            if (!agent.hasPath && !targetObject)
            {
                SetRandomDestination();
            }
        }
    }

    private void GoToTarget()
    {
        if (targetObject == null)
        {
            ChangeState(State.Idle);
            return;
        }

        Transform targetPosition = targetObject.transform.Find("PickupPoint");
        targetPosition = targetPosition == null ? targetObject.transform : targetPosition;

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            ChangeState(State.Searching);
            return;
        }

        if (Vector3.Distance(transform.position, targetPosition.position) <= 1)
        {
            ChangeState(State.PerformingAction);
            StartCoroutine(WaitAndPerformAction());
        }

        drawPath(targetPosition);
    }

    private IEnumerator WaitAndChangeState(State newState, float duration)
    {
        yield return new WaitForSeconds(duration);

        ChangeState(newState);
    }

    private IEnumerator WaitAndPerformAction()
    {
        transform.GetComponent<SmoothLookAt>().SetTargetToLookAt(targetObject.transform.gameObject);

        yield return new WaitForSeconds(2.0f);

        targetObject.GetComponent<ObjectBehavior>().PerformAction(this);

        targetObject = null;
        ChangeState(State.Idle);

        transform.GetComponent<SmoothLookAt>().ClearTargetToLookAt();
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * searchCollider.radius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, searchCollider.radius, -1);

        agent.SetDestination(navHit.position);

        lastRandomDestination = navHit.position;
    }

    private void DecreaseHungerAndEnergy()
    {
        currentHunger -= hungerDecayRate * Time.deltaTime;
        currentEnergy -= energyDecayRate * Time.deltaTime;

        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    private void RefillHungerAndEnergy()
    {
        currentHunger = maxHunger;
        currentEnergy = maxEnergy;
    }

    private void ChangeState(State newState)
    {
        State oldState = currentState;
        currentState = newState;
        Debug.Log($"STATE CHANGE: {oldState} => {newState}");
    }

    private void OnDrawGizmosSelected()
    {
        if (agent.hasPath == true)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(agent.destination, 0.5f);
        }
    }

    public void Eat(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
    }

    public void Rest(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    private void drawPath(Transform targetObject)
    {
        // Get the current path of the NavMeshAgent
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetObject.position, path);

        // Debug.Log($"Path status: {path.status}");

        if (path.status.Equals(NavMeshPathStatus.PathComplete))
        {
            // Draw the path using a LineRenderer component
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.material.color = Color.red;
                lineRenderer.positionCount = path.corners.Length;
            }
            else
            {
                lineRenderer.enabled = true;
            }

            Vector3[] corners = path.corners;
            lineRenderer.SetPositions(corners);
        }
    }
}