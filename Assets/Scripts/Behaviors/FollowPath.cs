﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : Seek
{
    // list of waypoints
    public GameObject[] path;
    public int pathIndex = 0;
    public float targetThreshold = .1f;
    public float delay = 0f;
    private bool isDelaying = false;
    private float delayTimer = 0f;

    public override SteeringOutput getSteering()
    {
        // initialize target to first waypoint
        if (target == null)
        {
            target = path[pathIndex];
        }

        if (isDelaying)
        {
            character.linearVelocity = Vector3.zero;
            delayTimer += Time.deltaTime;
            if (delayTimer >= delay)
            {
                isDelaying = false;
                delayTimer = 0f;
            }
            SteeringOutput result = new SteeringOutput();
            result.linear = Vector3.zero; // Stop moving during delay
            result.angular = 0;
            return result; // Return zero linear velocity during delay
        }

        // increment path index when waypoint is reached
        if ((target.transform.position - character.transform.position).magnitude < targetThreshold)
        {
            isDelaying = true;
            delayTimer = 0f;
            pathIndex = (pathIndex + 1) % path.Length;
            target = path[pathIndex];
        }

        return base.getSteering();
    }

}