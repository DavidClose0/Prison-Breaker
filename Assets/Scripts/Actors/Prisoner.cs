using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prisoner : Kinematic
{
    Face myRotateType;

    public float detectionDistance = 10f;

    // Start is called before the first frame update
    void Start()
    {
        myRotateType = new Face();
        myRotateType.character = this;
        myRotateType.target = myTarget;
    }

    // Update is called once per frame
    protected override void Update()
    {
        steeringUpdate = new SteeringOutput();
        steeringUpdate.linear = Vector3.zero;

        float distanceToTarget = (myTarget.transform.position - transform.position).magnitude;
        if (distanceToTarget <= detectionDistance)
        {
            steeringUpdate.angular = myRotateType.getSteering().angular;
        }
        else
        {
            steeringUpdate.angular = 0;
        }
        base.Update();
    }
}
