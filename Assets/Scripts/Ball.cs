using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    //[SerializeField]
    //private float speedThreshold = 10f;
    [SerializeField]
    private float speed;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        //rb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        SpeedControl();

    }
    public void CreateBallMovement(Vector3 startPoint, Vector3 direction)
    {
        direction = new Vector3(direction.x, direction.y, direction.y);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode.Impulse);
    }
    public void SpeedControl()
    {
        Vector3 ballVel = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
       
        if(ballVel.magnitude > speed)
        {
            Vector3 limitBallVel = ballVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitBallVel.x, limitBallVel.y, limitBallVel.z);
            //Debug.Log(rb.linearVelocity.magnitude);
        }
       
    }
}
