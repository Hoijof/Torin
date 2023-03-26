using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothLookAt : MonoBehaviour
{
    [SerializeField] private GameObject targetToLookAt;
    private Quaternion targetRotation;
    [SerializeField] private float turnSpeed = 2.0f;

    void Update()
    {
        if (targetToLookAt != null)
        {
            // Smoothly turn towards the target for 1 second
            Vector3 targetDirection = targetToLookAt.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    public void SetTargetToLookAt(GameObject target)
    {
        targetToLookAt = target;
    }

    public void ClearTargetToLookAt()
    {
        targetToLookAt = null;
    }
}