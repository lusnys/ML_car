using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class CarAgent : Agent
{
    Rigidbody rigidBody;
    Vector3 initialPosition;
    Quaternion initialRotation;
    SimpleCarController carController;
    List<WheelCollider> carWheels;
    public List<Sensor> sensors;

    float distanceToTarget;
    float newDistanceToTarget;
    bool respawned;

    private float distancePrimary;
    private float distanceSecondary;
    private float distanceLongRange;
    private float angle_z;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        initialPosition = GetComponent<Transform>().localPosition;
        initialRotation = GetComponent<Transform>().localRotation;
        carController = GetComponent<SimpleCarController>();
        carWheels = new List<WheelCollider>() {
            carController.axleInfos[0].rightWheel,
            carController.axleInfos[0].leftWheel,
            carController.axleInfos[1].rightWheel,
            carController.axleInfos[1].leftWheel
        };
        respawned = true;
    }

    public Transform Target;
    public override void AgentReset()
    {
        respawned = true;
        this.transform.localPosition = initialPosition;
        this.transform.localRotation = initialRotation;
        this.rigidBody.angularVelocity = Vector3.zero;
        this.rigidBody.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (respawned)
        {
            foreach (WheelCollider wc in carWheels)
            {
                wc.brakeTorque = Mathf.Infinity;
            }
            this.rigidBody.isKinematic = true;
            respawned = false;
        }
        else
        {
            this.rigidBody.isKinematic = false;
            foreach (WheelCollider wc in carWheels)
            {
                wc.brakeTorque = 0;
            }
    }
}

    public override void CollectObservations()
    {
        // Agent position
        AddVectorObs((this.transform.localPosition.x - (-25.0f)) / (160.0f - (-25.0f)));
        AddVectorObs((this.transform.localPosition.z - (-25.0f)) / (290.0f - (-25.0f)));

        // Target position
        AddVectorObs((Target.transform.localPosition.x - 0.0f) / (140.0f - 0.0f));
        AddVectorObs((Target.transform.localPosition.z - (-20.0f)) / (280.0f - (-20.0f)));

        // Agent velocity
        AddVectorObs((rigidBody.velocity.x - (-50.0f)) / (50.0f - (-50.0f)));
        AddVectorObs((rigidBody.velocity.z - (-50.0f)) / (50.0f - (-50.0f)));

        // Agent stability (rotation z)
        AddVectorObs(this.transform.localEulerAngles.z / 360.0f);

        // Sensor data
        foreach (Sensor s in sensors)
        {
            AddVectorObs(s.SensorFlashLongRange() / 100.0f);
            AddVectorObs(s.SensorFlashSide() / 100.0f);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // --- ACTIONS ---
        carController.UpdateMovement(vectorAction[0], vectorAction[1]);

        if (this.transform.localPosition.y < 0)
        {
            Done();
        }

        newDistanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        if (newDistanceToTarget < distanceToTarget)
        {
            SetReward(0.02f);
        }

        distanceToTarget = newDistanceToTarget;

        // --- REWARDS ---
        // Car has reached target
        if (distanceToTarget < 10f)
        {
            Done();
            SetReward(1.0f);
        }

        // Car is unstable
        angle_z = this.transform.localEulerAngles.z > 180 ? 360 - this.transform.localEulerAngles.z : this.transform.localEulerAngles.z;
        if (Mathf.Abs(angle_z) > 10)
        {
            Done();
            SetReward(-0.5f);
        }

        // Car is driving too slow or too fast
        if (rigidBody.velocity.magnitude < 2.5f || rigidBody.velocity.magnitude > 25.0f)
        {
            AddReward(-0.01f);
        }
        else
        if (rigidBody.velocity.magnitude >= 3.0f && rigidBody.velocity.magnitude <= 20.0f)
        {
            AddReward(0.01f);
        }

        // Car is too close to the obstacles
        foreach (Sensor s in sensors)
        {
            distancePrimary = s.SensorFlashPrimary();
            distanceSecondary = s.SensorFlashSecondary();

            if (distancePrimary < 2.5f || distanceSecondary < 1.5f)
            {
                AddReward(-0.01f);
            }

            if (distancePrimary < 0.5f || distanceSecondary < 0.5f)
            {
                Done();
                SetReward(-1.0f);
            }
        }
    }
}