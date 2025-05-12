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
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode.Impulse);
    }
    public void SpeedControl()
    {
        Vector3 ballVel = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
       
        if(ballVel.magnitude > maxSpeed)
        {
            speed = maxSpeed;
            Vector3 limitBallVel = ballVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitBallVel.x, limitBallVel.y, limitBallVel.z);
            Debug.Log(rb.linearVelocity.magnitude);
        }

    }
}
