using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Kinematic
{
    FollowPath patrolMoveType;
    Pursue pursueMoveType;
    Seek myMoveType;
    Face myRotateType;
    public Material pursueMaterial;
    public GameObject[] path;
    public GameObject player; 
    private Light spotLight;
    public Color pursueSpotlightColor = Color.red;

    public float detectionDistance = 5f;
    public float detectionAngle = 45f;
    public float delay = 3f;
    bool isChasingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        patrolMoveType = new FollowPath();
        patrolMoveType.character = this;
        patrolMoveType.path = path;
        patrolMoveType.delay = delay;

        pursueMoveType = new Pursue();
        pursueMoveType.character = this;
        pursueMoveType.target = player;

        myMoveType = patrolMoveType; // Start with patrol behavior

        myMoveType.getSteering(); // Set target to first waypoint

        myRotateType = new Face();
        myRotateType.character = this;
        myRotateType.target = myMoveType.target;

        spotLight = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        myRotateType.target = myMoveType.target;

        // Calculate direction and distance to player
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Calculate angle to player
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        // Player detected
        if (!isChasingPlayer && distanceToPlayer <= detectionDistance && angleToPlayer <= detectionAngle)
        {
            // Switch to chase behavior
            myMoveType = pursueMoveType;
            pursueMoveType.target = player; // Update chase target every frame in case player moved
            myRotateType.target = pursueMoveType.target; // Face the player

            this.GetComponent<MeshRenderer>().material = pursueMaterial;
            spotLight.color = pursueSpotlightColor;

            isChasingPlayer = true;
        }

        steeringUpdate = new SteeringOutput();
        steeringUpdate.linear = myMoveType.getSteering().linear;
        steeringUpdate.angular = myRotateType.getSteering().angular;
        base.Update();
    }
}