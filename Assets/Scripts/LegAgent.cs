using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.MLAgentsExamples;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(JointDriveController))] // Required to set joint forces

public class LegAgent : Agent
{
    [Header("Environmental Objects")]
    [Space(10)]

    public GameObject Ball;
    Rigidbody m_BallRb;

    public GameObject Table;

    [Header("Body Parts")]
    [Space(10)]

    public Transform Upper;
    public Transform Lower;

    [Header("Joint Controller")]
    [Space(10)]

    public float upperX;
    public float lowerX;
    public float strength;

    JointDriveController m_JdController;

    //float reward = 0.0f;

    public override void Initialize()
    {
        m_BallRb = Ball.GetComponent<Rigidbody>();

        m_JdController = GetComponent<JointDriveController>();

        m_JdController.SetupBodyPart(Upper);
        m_JdController.SetupBodyPart(Lower);
    }

    public override void OnEpisodeBegin()
    {
        //Reset all of the body parts
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }

        m_BallRb.velocity = new Vector3(0f, 0f, 0f);
        m_BallRb.angularVelocity = new Vector3(0f, 0f, 0f);
        m_BallRb.transform.position = new Vector3(0f, 4f, Random.Range(-7.4f, 7.4f)) + Table.transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Upper.transform.rotation.x);
        sensor.AddObservation(Lower.transform.rotation.x);
        sensor.AddObservation(Table.transform.rotation.x);
        sensor.AddObservation(Math.Abs(Table.transform.position.z - Ball.transform.position.z));
        sensor.AddObservation(m_BallRb.velocity);
        sensor.AddObservation(Ball.transform);

        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }

    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        sensor.AddObservation(bp.rb.transform.localRotation);
        sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
    }

    //Heuristic Controls for debugging.Has not been tested, but "TestMotionScript" contains similar code that will work for testing.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var i = -1;
        var inputAction = actionsOut.ContinuousActions;

        inputAction[0] = upperX;
        inputAction[1] = lowerX;
        inputAction[2] = strength;

        var bpDict = m_JdController.bodyPartsDict;

        bpDict[Upper].SetJointTargetRotation(inputAction[++i], 0, 0);
        bpDict[Lower].SetJointTargetRotation(inputAction[++i], 0, 0);

        foreach (var bodyPart in bpDict.Keys)
        {
            bpDict[bodyPart].SetJointStrength(strength);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        var vectorAction = actionBuffers.ContinuousActions;

        var bpDict = m_JdController.bodyPartsDict;
        var i = -1;

        bpDict[Upper].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);

        bpDict[Upper].SetJointStrength(vectorAction[++i]);
        bpDict[Lower].SetJointStrength(vectorAction[++i]);

        AddReward((float)(5.0f / Math.Pow(2.0f, Math.Abs(Table.transform.position.z - Ball.transform.position.z))));
        AddReward((float)(1.0f / Math.Pow(2.0f, Math.Abs(m_BallRb.velocity.z))));
        AddReward((float)(1.0f / Math.Pow(2.0f, Math.Abs(m_BallRb.velocity.y))));
        AddReward((float)(-1 * Math.Abs(m_BallRb.velocity.x)));
        AddReward((float)(1.0f / Math.Pow(2.0f, Math.Abs(Table.transform.rotation.x))));
        AddReward((float)(1.0f / Math.Pow(2.0f, Math.Abs(Upper.transform.rotation.x))));
        AddReward((float)(1.0f / Math.Pow(2.0f, Math.Abs(Lower.transform.rotation.x))));
    }
}
