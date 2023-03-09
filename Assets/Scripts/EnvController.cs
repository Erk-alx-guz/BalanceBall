using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvController : MonoBehaviour
{
    [Header("Agents")]
    [Space(10)]

    public LegAgent legOne;
    public LegAgent legTwo;

    [Header("Objects")]
    [Space(10)]

    public GameObject Ball;
    public GameObject Table;

    Rigidbody ballRB;

    private int resetTimer;

    [Header("Max Steps")]
    [Space(10)]

    public int MaxEnvironmentSteps;

    // Start is called before the first frame update
    void Start()
    {
        ballRB = Ball.GetComponent<Rigidbody>();

        ballRB.velocity = new Vector3(0f, 0f, 0f);
        ballRB.angularVelocity = new Vector3(0f, 0f, 0f);
        ballRB.transform.position = new Vector3(0f, 4f, Random.Range(-7.4f, 7.4f)) + Table.transform.position;
    }

    void FixedUpdate()
    {
        resetTimer += 1;
        //Debug.Log(resetTimer.ToString());
        if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            resetTimer = 0;

            legOne.EndEpisode();
            legTwo.EndEpisode();
        }

        if (Ball.transform.position.y < gameObject.transform.position.y)
        {
            legOne.AddReward(-10f);
            legTwo.AddReward(-10f);

            legOne.EndEpisode();
            legTwo.EndEpisode();
        }
    }

    // Update is called once per frame

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            //  The ball has hit the floor give negative reward
            legOne.AddReward(-1f);
            legTwo.AddReward(-1f);

            legOne.EndEpisode();
            legTwo.EndEpisode();
        }
    }
}
