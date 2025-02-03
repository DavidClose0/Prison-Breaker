using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector3 linearVelocity;
    public float angularVelocity;
    public float maxSpeed = 10.0f;
    public float maxAngularVelocity = 45.0f; // degrees

    public GameObject myTarget;
    protected SteeringOutput steeringUpdate;

    // OverlapBox Configuration
    public Vector3 overlapBoxHalfExtents = Vector3.one * 0.5f;
    public LayerMask collisionLayerMask = Physics.DefaultRaycastLayers;

    // Collision Resolution Parameters
    public float collisionResolutionForce = 10f;

    void Start()
    {
        steeringUpdate = new SteeringOutput();
    }

    protected virtual void Update()
    {
        if (float.IsNaN(angularVelocity))
        {
            angularVelocity = 0.0f;
        }

        Vector3 totalResolutionVector = Vector3.zero;

        Collider[] allColliders = Physics.OverlapBox(transform.position, overlapBoxHalfExtents, transform.rotation, collisionLayerMask, QueryTriggerInteraction.UseGlobal);
        List<Collider> externalColliders = new List<Collider>(); // List to store colliders that are NOT on this GameObject

        // Filter out colliders attached to this GameObject
        foreach (Collider collider in allColliders)
        {
            if (collider.gameObject != this.gameObject && collider.tag != "End")
            {
                externalColliders.Add(collider); // Add to the list of external colliders
            }
        }

        if (externalColliders.Count > 0) // Use the filtered list for collision resolution
        {
            foreach (Collider otherCollider in externalColliders)
            {
                if (otherCollider.gameObject.GetComponent<Player>() != null)
                {
                    GameManager gameManager = FindFirstObjectByType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.EndGame();
                        return;
                    }
                }

                Vector3 closestPoint = otherCollider.ClosestPoint(transform.position);
                Vector3 overlapDirection = transform.position - closestPoint;
                float overlapMagnitude = overlapDirection.magnitude;

                if (overlapMagnitude < overlapBoxHalfExtents.magnitude * 2f)
                {
                    if (overlapMagnitude > 0)
                    {
                        Vector3 resolutionVector = overlapDirection.normalized * collisionResolutionForce * Time.deltaTime;
                        totalResolutionVector += resolutionVector;
                    }
                    else
                    {
                        totalResolutionVector += Vector3.up * collisionResolutionForce * Time.deltaTime;
                    }
                }
            }
        }

        transform.position += totalResolutionVector;


        this.transform.position += linearVelocity * Time.deltaTime;
        if (Mathf.Abs(angularVelocity) > 0.01f)
        {
            Vector3 v = new Vector3(0, angularVelocity, 0);
            this.transform.eulerAngles += v * Time.deltaTime;
        }


        if (steeringUpdate != null)
        {
            linearVelocity += steeringUpdate.linear * Time.deltaTime;
            angularVelocity += steeringUpdate.angular * Time.deltaTime;
        }

        if (linearVelocity.magnitude > maxSpeed)
        {
            linearVelocity.Normalize();
            linearVelocity *= maxSpeed;
        }
        if (Mathf.Abs(angularVelocity) > maxAngularVelocity)
        {
            angularVelocity = maxAngularVelocity * (angularVelocity / Mathf.Abs(angularVelocity));
        }
    }
}