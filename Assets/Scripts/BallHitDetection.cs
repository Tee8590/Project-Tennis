using UnityEngine;

public class BallHitDetection : MonoBehaviour
{
    private Rigidbody ballRb;
    private GameObject ballPrefab;
    public float slowdownFactor = 0.7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballPrefab =  GameObject.FindGameObjectWithTag("Ball");
      if(ballPrefab !=null)
        { ballRb = ballPrefab.GetComponent<Rigidbody>(); }


    }
    void OnTriggerEnter(Collider other)
    {
        if (ballRb == null || transform.position == null) return;

        // Get velocity direction (3D)
        Vector3 ballDirection = ballRb.linearVelocity.normalized;

        // Calculate direction FROM BALL TO PLAYER(collider)
        Vector3 directionToPlayer = (transform.position - ballPrefab.transform.position).normalized;

        //  directions
        float dot = Vector3.Dot(ballDirection, directionToPlayer);
        
        if (other.gameObject.CompareTag("Ball"))
        { Debug.Log("Dot product: " + dot); }

        if (dot > 0)
        {
            Debug.Log("Ball is moving toward the player");
        }
        else
        {
            Debug.Log("Ball is moving away from the player");
        }
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
