using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;
using Random = UnityEngine.Random;
using System;

[RequireComponent(typeof(JointDriveController))] // Required to set joint forces

public class TestMotinon : MonoBehaviour
{

    [Header("Environmental Objects")]
    [Space(10)]

    public GameObject Table;

    public GameObject Ball;
    Rigidbody m_BallRb;


    [Header("Body Parts")]
    [Space(10)]

    public Transform UpperOne;
    public Transform LowerOne;    
    public Transform UpperTwo;
    public Transform LowerTwo;

    [Header("Joint Controllers")]
    [Space(10)]

    public float upperOneX;
    public float lowerOneX;
    public float upperTwoX;
    public float lowerTwoX;
    public float strength;

    JointDriveController m_JdController;

    private int m_Index = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_BallRb = Ball.GetComponent<Rigidbody>();

        m_JdController = GetComponent<JointDriveController>();

        m_JdController.SetupBodyPart(UpperOne);
        m_JdController.SetupBodyPart(LowerOne);        
        m_JdController.SetupBodyPart(UpperTwo);
        m_JdController.SetupBodyPart(LowerTwo);
    }

    // Update is called once per frame
    void Update()
    {
        var bpDict = m_JdController.bodyPartsDict;

        bpDict[UpperOne].SetJointTargetRotation(upperOneX, 0, 0);
        bpDict[LowerOne].SetJointTargetRotation(lowerOneX, 0, 0);
        bpDict[UpperTwo].SetJointTargetRotation(upperTwoX, 0, 0);
        bpDict[LowerTwo].SetJointTargetRotation(lowerTwoX, 0, 0);

        bpDict[UpperOne].SetJointStrength(strength);
        bpDict[LowerOne].SetJointStrength(strength);        
        bpDict[UpperTwo].SetJointStrength(strength);
        bpDict[LowerTwo].SetJointStrength(strength);

        m_Index++;

       if (m_Index >= 1000)
       {
            m_Index = 0;
            m_BallRb.velocity = new Vector3(0f, 0f, 0f);
            m_BallRb.angularVelocity = new Vector3(0f, 0f, 0f);
            Ball.transform.position = new Vector3(0f, 12.28f, Random.Range(-6.97f, 7.10f)) + gameObject.transform.position;
        }

       if (Ball.transform.position.y < gameObject.transform.position.y)
        {
            Debug.Log("Fell of the ground");
        }

    }
}
