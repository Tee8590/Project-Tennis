using UnityEngine;

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
    public float launchForce ;
    public int noOfPoints; // Number of points drawn
    public float timeStep; // Time between each point

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        //rb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        SpeedControl();

    }
    public void CreateBallMovement(Vector3 startPoint, Vector3 direction, float swipeTime)
    {
        direction = new Vector3(direction.x, direction.y, direction.y*2);
        rb = GetComponent<Rigidbody>();
        speed += swipeTime * 10;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode.Impulse);
        //Debug.Log($"velocity: " + rb.linearVelocity + " direction: " + direction.normalized + " swipeTime: " + swipeTime);
        SpeedControl();
    }
    
    public void SpeedControl()
    {
        Vector3 ballVel = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
       
        if(ballVel.magnitude > maxSpeed)
        {
            speed = maxSpeed;
            Vector3 limitBallVel = ballVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitBallVel.x, limitBallVel.y, limitBallVel.z);
        }

    }
    void SimulatedBallTrajectory(Vector3 startPoint, Vector3 launchDirection, float speed)
    {
          ballSpawnPoint.position = startPoint;
       if(ballSpawnPoint == null) return;

        launchForce = speed / rb.mass;
        Vector3 startPos = ballSpawnPoint.position;
        Vector3 velocity = launchDirection.normalized * launchForce;
        
        Vector3 prevPoint = startPos;
        for(int i =0; i <= noOfPoints; i++)
        {
            float t = i * timeStep;
            Vector3 gravity = Physics.gravity;

            //ProjectileMotion
            //P(t) =                V₀       ·t + ½    ·g          ·t²
            Vector3 displacement = velocity * t + 0.5f * gravity * t * t;
            Vector3 point = startPos + displacement;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}
