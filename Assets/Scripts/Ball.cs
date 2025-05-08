using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        //rb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
    }
    public void CreateBallMovement(Vector3 startPoint, Vector3 direction)
    {
        direction = new Vector3(direction.x, direction.y, direction.y);
         rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * 10f * Time.deltaTime, ForceMode.Impulse);
    }
    void Update()
    {
        
    }

}
