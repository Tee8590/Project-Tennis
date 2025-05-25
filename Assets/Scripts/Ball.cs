using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour
{
    public static event Action<Vector3> OnBallLandingPoint;
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


    void Start()
    {
        ballSpawnPoint = gameObject.transform;
        rb = GetComponent<Rigidbody>();
        //rb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        SpeedControl();

    }
    public void CreateBallMovement(Vector3 startPoint, Vector3 direction, float swipeTime)
    {
      Vector3  ogDirection = new Vector3(direction.x, direction.y, direction.y * 2);
        rb = GetComponent<Rigidbody>();
        speed += swipeTime * 10;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(ogDirection * speed , ForceMode.Impulse);
       
        SpeedControl();
        Vector3 ballDir = new Vector3(ogDirection.x, ogDirection.y, ogDirection.z);

        Vector3 velocity = ballDir.normalized * speed;
       

        CalculateLandingPoint(startPoint, velocity, 18.24f);
        
       
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

    private Vector3 CalculateLandingPoint(Vector3 startPos, Vector3 startVelocity, float groundY)
    {
        // Decompose initial conditions
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

        Debug.Log("discriminant"+discriminant);

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
        Instantiate(trailBallPrefab, landingPos, Quaternion.identity);
        Debug.Log("landingPos"+landingPos);
        OnBallLandingPoint?.Invoke(landingPos);
        return landingPos;
    }
}
