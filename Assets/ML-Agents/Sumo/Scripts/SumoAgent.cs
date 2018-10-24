using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
}

public class SumoAgent : Agent
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public GameObject cart;
    public GameObject pole;
    private Rigidbody cartRb;
    private Rigidbody poleRb;

    public override void AgentAction (float[] vectorAction, string textAction)
    {
        float motor = maxMotorTorque * Mathf.Clamp(vectorAction[0], -0.5f, 0.5f);

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }

        Vector3 targetDirection = pole.transform.position - cart.transform.position;

        float angle = Vector3.Angle(
            targetDirection,
            transform.right);

        // check boundaries
        if (cart.transform.position.x < -20 || cart.transform.position.x > 20)
        {
            AgentReset();
        }

        // negative reinforcement
        if (angle < 70 || angle > 110)
        {
            Done();
            AddReward(-1);
        }

        // positive reinforcement
        AddReward(0.1f);
    }

    public override void AgentReset ()
    {
        // pole.transform.position = new Vector3(0f, 4f, 0f);
        // pole.transform.rotation = Quaternion.identity;
        // poleRb.velocity = Vector3.zero;
        // poleRb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations ()
    {
        AddVectorObs(cart.transform.position);
        AddVectorObs(pole.transform.position);
    }

    public override void InitializeAgent ()
    {
        cartRb = cart.GetComponent<Rigidbody>();
        poleRb = pole.GetComponent<Rigidbody>();
    }

}
