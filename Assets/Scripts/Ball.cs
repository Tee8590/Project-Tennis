using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * 5f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
