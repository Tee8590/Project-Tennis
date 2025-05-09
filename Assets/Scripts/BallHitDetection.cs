using System.Collections;
using UnityEngine;

public class BallHitDetection : MonoBehaviour
{
    private Rigidbody ballRb;
    private GameObject ballPrefab;
    public float slowdownFactor = 0.7f;
    public LayerMask layerMask;
   
    void Start()
    {
        //layerMask = ~LayerMask.GetMask("Ground");
       

    }
    public bool OnTriggerEnter(Collider other)
    {
        ballPrefab = GameObject.FindGameObjectWithTag("Ball");
        if (ballPrefab != null)
        { ballRb = ballPrefab.GetComponent<Rigidbody>(); }

        if (ballRb == null || !other.gameObject.CompareTag("Ball"))
        {
            Debug.Log(other.gameObject.ToString());
            return false;
        }
        else
            //// Get velocity direction
            //Vector3 ballDirection = ballRb.linearVelocity.normalized;
            //// Calculate direction FROM BALL TO PLAYER(collider)
            //Vector3 directionToPlayer = (transform.position - ballPrefab.transform.position).normalized;
            ////  directions
            //float dot = Vector3.Dot(ballDirection, directionToPlayer);


            ////Ball is moving toward the player
            //if (dot > 0)
            //{
            //   Debug.Log("Dot product: " + dot);
            //   StartCoroutine(SlowBall(ballRb, 0.5f,3f));


            //   Debug.Log("Ball is moving toward the player");

            //}
            //else
            //{
            //    Debug.Log("Ball is moving away from the player");
            //}
            Debug.Log(other.gameObject.name); return true;
    }
    public IEnumerator SlowBall(Rigidbody rb, float slowFactor, float duration)
    {
       
        Vector3 originalVelocity = rb.linearVelocity;

        //  slowdown
        rb.useGravity = false; rb.linearVelocity = originalVelocity * slowFactor;
      
        yield return new WaitForSeconds(duration);

        rb.useGravity = true; rb.linearVelocity = originalVelocity;
    }
    
}
