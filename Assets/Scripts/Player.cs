using UnityEngine;

public class Player : MonoBehaviour
{
    //private int Colliders = 10;
    private GameObject ball;
    public float moveSpeed = 20.0f;
    [SerializeField]
    //private BoxCollider HItboxCollider;
    private Rigidbody ballRb;
    [SerializeField]
    private BallHitDetection ballHitDetection;
    void Start()
    {
       
    }
    void FixedUpdate()
    {
       if( ball = GameObject.FindGameObjectWithTag("Ball"))
              ballRb = ball.GetComponent<Rigidbody>();
        
        StartFollowTheBall();
        
    }
    
    void PlayerMovement()
    {
       
        if (ball != null)
        {
            Vector3 currentPosition = transform.position;
            float targetX = Mathf.Lerp(currentPosition.x, ball.transform.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(targetX, currentPosition.y, currentPosition.z);
        }
    }
    public void StartFollowTheBall()
    {
      if(ballRb != null)  
      {  //// Get velocity direction
       Vector3 ballDirection = ballRb.linearVelocity.normalized;
        //// Calculate direction FROM BALL TO PLAYER(collider)
        Vector3 directionToPlayer = (transform.position - ball.transform.position).normalized;
        ////  directions
        float dot = Vector3.Dot(ballDirection, directionToPlayer);

            ////Ball is moving toward the player
            if (dot > 0)
            {
                PlayerMovement();
                //Debug.Log("Ball is moving toward the player");
            }
            else if (dot < 0)
            {
               // Debug.Log("Ball is moving away from the player");
            }
        }
    }
    void SwingRange(Vector3 center, float radius)
    {
        int MaxColliders = 10;
        Collider[] hitColliders = new Collider[MaxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, radius, hitColliders);
        for (int i = 0; i < numColliders; i++)
        {

        }

    }

}
