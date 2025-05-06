using System.Collections;
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

        // Get velocity direction
        Vector3 ballDirection = ballRb.linearVelocity.normalized;
        // Calculate direction FROM BALL TO PLAYER(collider)
        Vector3 directionToPlayer = (transform.position - ballPrefab.transform.position).normalized;
        //  directions
        float dot = Vector3.Dot(ballDirection, directionToPlayer);


        //Ball is moving toward the player
        if (dot > 0)
        {
            if (other.gameObject.CompareTag("Ball"))
            { Debug.Log("Dot product: " + dot);
                StartCoroutine(SlowBall(ballRb, 0.1f, 4f));
            }
            Debug.Log("Ball is moving toward the player");

        }
        else
        {
            Debug.Log("Ball is moving away from the player");
        }
    }
    private IEnumerator SlowBall(Rigidbody rb, float slowFactor, float duration)
    {
        // Remember the original velocity
        Vector3 originalVelocity = rb.linearVelocity;

        // Apply slowdown
        rb.linearVelocity = originalVelocity * slowFactor;

        // Wait
        yield return new WaitForSeconds(duration);

        // Restore original speed (preserving direction)
        rb.linearVelocity = originalVelocity;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
