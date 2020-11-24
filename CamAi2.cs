using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
public class CamAi2: Agent
{
    // Public Variables to be set in inspector
    public GameObject targetPosition;
    public EyeScript eyeL, eyeC, eyeR;
    public float viewDistance = 20f;

    // Script variables
    GameObject target;        
    bool seesTarget = false, goingRight = true;
    int counter;

    // Set cam rotation and target position
    public override void OnEpisodeBegin()
    {
        // Cam rotation
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        // Target position
        targetPosition.transform.localPosition = new Vector3(0, 1.5f, 0);

        counter = 0;
    }
    // Input target position and cam rotation into the neural net
    public override void CollectObservations(VectorSensor sensor)
    {
        // Send the state of the left eye ray
        sensor.AddObservation(eyeL.seesTarget);
        sensor.AddObservation(eyeL.targetDistance);

        // Send the state of the center eye ray
        sensor.AddObservation(eyeC.seesTarget);
        sensor.AddObservation(eyeC.targetDistance);

        // Send the state of the right eye ray
        sensor.AddObservation(eyeR.seesTarget);
        sensor.AddObservation(eyeR.targetDistance);
    }

    // Determines behavior actions and training model actions
    public override void OnActionReceived(float[] vectorAction)
    {

        // Action buffers are the output and determine which way the camera turns
        if (vectorAction[0] == 0)
        {
            // No rotation
        }
        if (vectorAction[0] == 1)
        {
            // Rotate the camera right
            gameObject.transform.Rotate(new Vector3(0, 0.5f, 0));
        }
        if (vectorAction[0] == 2)
        {
            // Rotate the camera right
            gameObject.transform.Rotate(new Vector3(0, -0.5f, 0));
        }

        // Training model Actions based on performance
        if (eyeL.seesTarget || eyeC.seesTarget || eyeR.seesTarget)
        {
            // The camera is on the target so add to the reward
            SetReward(0.2f);

            // Reset the counter because the camera is on target
            counter = 0;
        }
        else
        {
            // The camera is off the target, reduce from the reward
            SetReward(-0.1f);

            // Increment the counter so we know how long the camera is off target
            counter++;

            // If the camera is off target for 25 itterations reset the episode
            if (counter == 25)
            {
                // End the episode and reset
                EndEpisode();
            }
        }
    }
    
    // This is used for manual override of neural net output
    public override void Heuristic(float[] actionsOut)
    {
        /*        
        float h = Input.GetAxisRaw("Horizontal");
        Debug.Log("input: "+ h);
        if (h == 0)
            actionsOut[0] = 0;
        if (h > 0)
            actionsOut[0] = 1;
        if (h < 0)
            actionsOut[0] = 2;
        //base.Heuristic(actionsOut);
        */
    }

}
