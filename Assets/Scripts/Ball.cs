using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    //public float slowdownFactor = 0.7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        
    }
    //void OnCollisionEnter(Collision collision)
    //{
    //    // Calculate direction from velocity
    //    if (rb.linearVelocity != Vector3.zero)
    //    {
    //        Vector2 direction = rb.linearVelocity.normalized;
    //        Debug.Log("Direction: " + direction);
    //    }
    //    // Reduce velocity
    //    //rb.linearVelocity *= slowdownFactor;
    //    // Optional: Reduce angular velocity (spin)
    //    //rb.angularVelocity *= slowdownFactor;
    //}
    // Update is called once per frame
    void Update()
    {
        //rb.AddForce(-Vector3.forward * 5f * Time.deltaTime, ForceMode.Impulse);
    }
}
