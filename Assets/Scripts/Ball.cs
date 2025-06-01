using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    //[SerializeField]
    //private float speedThreshold = 10f;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    public Transform ballSpawnPoint;
    public Vector3 launchDirection = Vector3.forward;
    public float launchForce;
    //public int noOfPoints; // Number of points drawn
    //public float timeStep; // Time between each point
    public GameObject trailBallPrefab; // Prefab with Rigidbody and Trail Renderer
    public int noOfPoints = 50;
    public float timeStep = 0.1f;
    public static event Action<Rigidbody, Vector3, Vector3> BallStartAndEndpositions;
    void Start()
    {
        ballSpawnPoint = gameObject.transform;
        rb = GetComponent<Rigidbody>();
        trailBallPrefab = Instantiate(trailBallPrefab, new Vector3(999,999,999), Quaternion.AngleAxis(-90, new Vector3(-90, 0, 0)));

        //rb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        SpeedControl();

    }
    public Vector3 CreateBallVelocity(Vector3 startPoint, Vector3 direction, float swipeTime)
    {
        Debug.Log("directionvelocity " + direction);
        Vector3  ogDirection = new Vector3(direction.x, direction.y, direction.y * 2);
        rb = GetComponent<Rigidbody>();
        speed += swipeTime * 10;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //SimulatedBallTrajectory(startPoint, direction, speed);

        //Debug.Log($"velocity: " + rb.linearVelocity + " direction: " + direction.normalized + " swipeTime: " + swipeTime);
      
        Vector3 ballDir = new Vector3(ogDirection.x, ogDirection.y, ogDirection.z);
        Vector3 velocity = ogDirection.normalized * speed ;

        //CalculateLandingPoint(startPoint, velocity, 18.24f);
        /*rb.AddForce(velocity, ForceMode.Impulse);*/
      /*  Debug.Log("velocity " + velocity);*/
       // BallStartAndEndpositions?.Invoke(rb, startPoint, CalculateLandingPoint(startPoint, velocity, 18.24f));
        //SpeedControl();
        return velocity;
    }

    public void SpeedControl()
    {
        Vector3 ballVel = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);

        if (ballVel.magnitude > maxSpeed)
        {
            speed = maxSpeed;
            Vector3 limitBallVel = ballVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitBallVel.x, limitBallVel.y, limitBallVel.z);
        }

    }

    

    public Vector3 CalculateLandingPoint(Vector3 startPos, Vector3 startVelocity, float groundY)
    {
        float y0 = startPos.y;                  // start height
        float vy = startVelocity.y;             // initial vertical velocity
        Vector3 vHorizontal = new Vector3(startVelocity.x, 0, startVelocity.z);

        // Use Physics.gravity for g (negative value, e.g. -9.81)
        float g = Physics.gravity.y;
        float a = 0.5f * g;
        float b = vy;
        float c = y0 - groundY;

        // Solve quadratic a*t^2 + b*t + c = 0 for time t
        float discriminant = Mathf.Abs(b * b - 4f * a * c);
        if (discriminant < 0f)
        {
            // No real solution: does not hit the plane
            return Vector3.zero;
        }
        float sqrtDisc = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);
        // Choose the positive (future) time
        float t = Mathf.Max(t1, t2);
        if (t < 0f)
        {
            // Both times negative: impact is in the past
            return Vector3.zero;
        }

        // Calculate impact position using P = start + v*t + 0.5*g*t^2
        Vector3 landingPos = startPos
                            + startVelocity * t
                            + 0.5f * Physics.gravity * t * t;
        // Ensure Y is exactly groundY (prevent tiny float errors)
        landingPos.y = groundY;
        BallLandingPositionMarker(landingPos);
        Debug.Log("landingPos"+landingPos);

        return landingPos;
    }
    public Vector3 BallLandingPositionMarker(Vector3 lp)
    {
        if(lp !=  Vector3.zero)
        {
            Vector3 landingPosSpot = new Vector3(lp.x, lp.y + .02f, lp.z);
           // Instantiate(trailBallPrefab, landingPosSpot, Quaternion.AngleAxis(-90, new Vector3(-90, 0, 0)));
           trailBallPrefab.transform.position = landingPosSpot;

            return lp;
        }
        else
        {
            Debug.LogError("Ball landingPos is " + lp);
            return Vector3.zero;
        }
    }
}
